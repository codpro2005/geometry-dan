using System;
using System.Linq;
using MyProperties;
using MyUnityExtensions;
using UnityEngine;
 
public enum State
{
	Run,
	Fly
}

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Vector2 jumpForce;
	[SerializeField] private Vector2 flyForce;
	[SerializeField] private bool enableActionOnTouch;
	[SerializeField] [ConditionalField("enableActionOnTouch")] private float touchMultiplier;
	[SerializeField] private bool enableActionOnVolume;
	[SerializeField] [ConditionalField("enableActionOnVolume")] private float singleActionVolumeThreshold;
	[SerializeField] [ConditionalField("enableActionOnVolume")] private float volumeMultiplier;
	[SerializeField] private bool enableActionOnTilt;
	[SerializeField] [ConditionalField("enableActionOnTilt")] private Vector2 singleActionTiltThreshold;
	[SerializeField] [ConditionalField("enableActionOnTilt")] private float tiltMultiplier;
	[SerializeField] private GameObject jumpResetOnCollideWith;
	[SerializeField] private GameObject killOnCollideWith;
	[SerializeField] private GameObject triggerFlyOnCollideWith;
	[SerializeField] private bool resetVelocityOnStateSwitch;

	private Rigidbody2D currentRigidbody2D;
	private State state;
	private GameObject jumpResetOnCollideWithReference;
	private GameObject killOnCollideWithReference;
	private GameObject triggerFlyOnCollideWithReference;
	private AudioClip microphoneInput;
	private bool microphoneExists;
	private bool jump;
	private bool fly;
	private Vector2 force;
	private float volume;
	private Vector2 tilt;
	private bool touchesGround;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentRigidbody2D = this.GetComponent<Rigidbody2D>();
		this.jumpResetOnCollideWithReference = GameObject.Find(this.jumpResetOnCollideWith.name);
		this.killOnCollideWithReference = GameObject.Find(this.killOnCollideWith.name);
		this.triggerFlyOnCollideWithReference = GameObject.Find(this.triggerFlyOnCollideWith.name);
		if (Microphone.devices.Length > 0)
		{
			this.microphoneExists = true;
			this.microphoneInput = Microphone.Start(Microphone.devices[0], true, 999, 44100);
		}
	}

	// Start is called before the first frame update
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{
		this.SetVolume();
		this.SetTilt();

		var touchThresholdReached = this.enableActionOnTouch && Input.touches.Any(touch =>
			                 touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary ||
			                 touch.phase == TouchPhase.Moved);
		;
		var volumeThresholdReached = this.enableActionOnVolume && this.volume >= this.singleActionVolumeThreshold;
		var tiltThresholdReached = this.enableActionOnTilt && this.tilt.BiggerOrEqualThan(this.singleActionTiltThreshold);
		switch (this.state)
		{
			case State.Run:
				if (this.touchesGround && this.currentRigidbody2D.velocity.y <= 0 &&
				    (touchThresholdReached || volumeThresholdReached || tiltThresholdReached))
				{
					this.jump = true;
					this.force = this.jumpForce;
				}

				break;
			case State.Fly:
				if (touchThresholdReached || this.enableActionOnVolume || this.enableActionOnTilt)
				{
					this.fly = true;
					var totalForce = Vector2.zero;
					if (touchThresholdReached)
					{
						totalForce += this.flyForce * this.touchMultiplier;
					}

					if (this.enableActionOnVolume)
					{
						totalForce += this.flyForce * this.volume * this.volumeMultiplier;
					}

					if (this.enableActionOnTilt)
					{
						totalForce += this.flyForce * this.tilt * this.tiltMultiplier;
					}

					this.force = totalForce;
				}

				break;
			default:
				break;
		}
	}

	private void FixedUpdate()
	{
		void AddForce() => this.currentRigidbody2D.AddForce(this.force);
		PlayerController.ActionOnConditionForFixedUpdate(ref this.jump, AddForce);
		PlayerController.ActionOnConditionForFixedUpdate(ref this.fly, AddForce);
	}

	private void OnCollisionEnter2D(Collision2D collision2D)
	{
		var collision2DTransfromTag = collision2D.transform.tag;
		if (collision2DTransfromTag == this.jumpResetOnCollideWithReference.tag)
		{
			this.touchesGround = true;
		} else
		if (collision2DTransfromTag == this.killOnCollideWithReference.tag)
		{
			Handheld.Vibrate();
			Handler.ReloadScene();
		}
	}

	private void OnTriggerEnter2D(Collider2D collider2D)
	{
		if (collider2D.transform.tag != this.triggerFlyOnCollideWithReference.tag) return;
		this.state = this.state == State.Fly ? State.Run : State.Fly;
		Debug.Log(this.state);
		if (resetVelocityOnStateSwitch)
		{
			this.currentRigidbody2D.velocity = Vector2.zero;
		}
	}

	private void OnCollisionExit2D(Collision2D collision2D)
	{
		var collision2DTransfromTag = collision2D.transform.tag;
		if (collision2DTransfromTag == this.jumpResetOnCollideWithReference.tag)
		{
			this.touchesGround = false;
		}
	}

	private static void ActionOnConditionForFixedUpdate(ref bool condition, Action action)
	{
		if (!condition) return;
		action();
		condition = false;
	}

	private void SetVolume()
	{
		var currentMicPosition = Microphone.GetPosition(null);
		if (!microphoneExists || currentMicPosition <= 0) return;
		const int dec = 128;
		var waveData = new float[dec];
		this.microphoneInput.GetData(waveData, currentMicPosition - (dec + 1));
		this.volume = Mathf.Sqrt(Mathf.Sqrt(waveData.Select(single => Mathf.Pow(single, 2)).Max()));
	}

	private void SetTilt()
	{
		var acceleration = Input.acceleration;
		this.tilt = new Vector2(acceleration.x > 0 ? acceleration.x : 0, acceleration.y > 0 ? acceleration.y : 0);
	}

	public State GetState()
	{
		return this.state;
	}
}
