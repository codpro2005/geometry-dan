using System;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum State
{
	Run,
	Fly
}

//public class Active
//{
//    public bool isActive;
//}

//public class Active<T>
//{
//    public bool isActive;
//    public T Value;
//}

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Vector2 jumpForce;
	[SerializeField] private Vector2 flyForce;
	[SerializeField] private bool enableActionOnTouch;
	[SerializeField] private bool enableActionOnVolume;
	[SerializeField] [ConditionalField("enableActionOnVolume")] private float jumpVolumeThreshold;
	[SerializeField] [ConditionalField("enableActionOnVolume")] private float volumeMultiplier;
	[SerializeField] private bool enableActionOnTilt;
	[SerializeField] [ConditionalField("enableActionOnTilt")] private float jumpTiltThreshold;
	[SerializeField] [ConditionalField("enableActionOnTilt")] private float tiltMultiplier;
	[SerializeField] private GameObject jumpResetOnCollideWith;
	[SerializeField] private GameObject killOnCollideWith;
	[SerializeField] private GameObject triggerFlyOnCollideWith;

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
	private bool touchesGround;
	private Handler handler;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentRigidbody2D = this.GetComponent<Rigidbody2D>();
		this.jumpResetOnCollideWithReference = GameObject.Find(this.jumpResetOnCollideWith.name);
		this.killOnCollideWithReference = GameObject.Find(this.killOnCollideWith.name);
		this.triggerFlyOnCollideWithReference = GameObject.Find(this.triggerFlyOnCollideWith.name);
		this.handler = GameObject.Find("Handler").GetComponent<Handler>();
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
		var currentMicPosition = Microphone.GetPosition(null);
		if (microphoneExists && currentMicPosition > 0)
		{
			const int dec = 128;
			var waveData = new float[dec];
			this.microphoneInput.GetData(waveData, currentMicPosition - (dec + 1));
			this.volume = Mathf.Sqrt(Mathf.Sqrt(waveData.Select(single => Mathf.Pow(single, 2)).Max()));
		}

		var validTouch = this.enableActionOnTouch && Input.touches.Any(touch =>
			                 touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary ||
			                 touch.phase == TouchPhase.Moved);
		var validVolume = this.enableActionOnVolume;
		switch (this.state)
		{
			case State.Run:
				if (this.touchesGround && this.currentRigidbody2D.velocity.y <= 0 &&
				    (validTouch || validVolume && this.volume > this.jumpVolumeThreshold))
				{
					this.jump = true;
					this.force = this.jumpForce;
				}

				break;
			case State.Fly:
				if (validTouch || validVolume)
				{
					this.fly = true;
					var totalForce = Vector2.zero;
					if (validTouch)
					{
						totalForce += this.flyForce;
					}

					if (validVolume)
					{
						totalForce += this.flyForce * this.volume;
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

	private static void ActionOnConditionForFixedUpdate(ref bool condition, Action action)
	{
		if (!condition) return;
		action();
		condition = false;
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
			this.handler.ReloadScene();
		}
	}

	private void OnTriggerEnter2D(Collider2D collider2D)
	{
		var collider2DTransformTag = collider2D.transform.tag;
		if (collider2DTransformTag == this.triggerFlyOnCollideWithReference.tag)
		{
			this.state = this.state == State.Fly ? State.Run : State.Fly;
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
}
