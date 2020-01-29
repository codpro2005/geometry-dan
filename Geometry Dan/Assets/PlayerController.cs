using System;
using System.Linq;
using MyProperties;
using MyUnityExtensions;
using TMPro;
using UnityEngine;

public enum State
{
	Jump, // First State is default state
	Fly,
	Glide,
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
	[SerializeField] private GameObject triggerGlideOnCollideWith;
	[SerializeField] private GameObject triggerReverseGravityOnCollideWith;
	[SerializeField] private bool resetVelocityOnSwitch;
	[SerializeField] private GameObject progressionPercentage;

	private Transform currentTransform;
	private Rigidbody2D currentRigidbody2D;
	private ConstantVelocity currentConstantVelocity;
	private GameObject progressionPercentageReference;
	private State state;
	private GameObject jumpResetOnCollideWithReference;
	private GameObject killOnCollideWithReference;
	private GameObject triggerFlyOnCollideWithReference;
	private GameObject triggerGlideOnCollideWithReference;
	private GameObject triggerReverseGravityOnCollideWithReference;
	private AudioClip microphoneInput;
	private bool microphoneExists;
	private bool jump;
	private bool fly;
	private bool glide;
	private Vector2 force;
	private float volume;
	private Vector2 tilt;
	private bool touchesGround;
	private float gravityStrength;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();
		this.currentRigidbody2D = this.GetComponent<Rigidbody2D>();
		this.currentConstantVelocity = this.GetComponent<ConstantVelocity>();
		this.progressionPercentageReference = GameObject.Find(this.progressionPercentage.transform.name);
		this.jumpResetOnCollideWithReference = GameObject.Find(this.jumpResetOnCollideWith.name);
		this.killOnCollideWithReference = GameObject.Find(this.killOnCollideWith.name);
		this.triggerFlyOnCollideWithReference = GameObject.Find(this.triggerFlyOnCollideWith.name);
		this.triggerGlideOnCollideWithReference = GameObject.Find(this.triggerGlideOnCollideWith.name);
		this.triggerReverseGravityOnCollideWithReference = GameObject.Find(this.triggerReverseGravityOnCollideWith.name);
		var gravityScale = this.currentRigidbody2D.gravityScale;
		this.gravityStrength = gravityScale > 0 ? 1 : gravityScale < 0 ? -1 : 0;
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
		var anyThresholdReached = touchThresholdReached || volumeThresholdReached || tiltThresholdReached;
		var possibleAction = touchThresholdReached || this.enableActionOnVolume || this.enableActionOnTilt;
		switch (this.state)
		{
			case State.Jump:
				if (this.touchesGround && this.currentRigidbody2D.velocity.y * this.gravityStrength <= 0 && anyThresholdReached)
				{
					this.jump = true;
					this.force = this.jumpForce;
				}

				break;
			case State.Fly:
				if (possibleAction)
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
				else
				{
					this.fly = false;
				}

				break;
			case State.Glide:
				this.glide = anyThresholdReached;

				break;
			default:
				break;
		}
	}

	private void FixedUpdate()
	{
		void AddForce() => this.currentRigidbody2D.AddForce(new Vector2(this.force.x, this.force.y * this.gravityStrength));
		void EnableRigidbody2D(bool enable) => this.currentRigidbody2D.isKinematic = !enable;

		PlayerController.ActionOnCondition(ref this.jump, AddForce, true);
		PlayerController.ActionOnCondition(ref this.fly, AddForce);
		PlayerController.ActionOnCondition(ref this.glide, () => EnableRigidbody2D(false), false,
			() =>
			{
				EnableRigidbody2D(true);
				this.state = 0;
			});

		this.force = Vector2.zero;
		this.RespawnOnFrontalTouch();
	}

	private void OnCollisionEnter2D(Collision2D collision2D)
	{
		var collision2DTransfromTag = collision2D.transform.tag;
		if (collision2DTransfromTag == this.jumpResetOnCollideWithReference.tag)
		{
			this.touchesGround = true;
		}
		else
		if (collision2DTransfromTag == this.killOnCollideWithReference.tag)
		{
			this.Respawn();
		}
	}

	private void OnTriggerEnter2D(Collider2D collider2D)
	{
		var collider2DTransformTag = collider2D.transform.tag;
		var tagIsTriggerFly = collider2DTransformTag == this.triggerFlyOnCollideWithReference.tag;
		var tagIsGlide = collider2DTransformTag == this.triggerGlideOnCollideWithReference.tag;
		var tagisReverseGravity = collider2DTransformTag == this.triggerReverseGravityOnCollideWithReference.tag;
		if (!tagIsTriggerFly && !tagIsGlide && !tagisReverseGravity) return;
		if (tagIsTriggerFly)
		{
			this.state = this.state == State.Fly ? 0 : State.Fly;
		} else 
		if (tagIsGlide)
		{
			this.state = State.Glide;
		} else
		if (tagisReverseGravity)
		{
			this.currentRigidbody2D.gravityScale *= Handler.Reverse;
			this.gravityStrength *= Handler.Reverse;
		}
		if (resetVelocityOnSwitch)
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

	private static void ActionOnCondition(ref bool condition, Action actionOnTrue, bool reset = false, Action actionOnFalse = null)
	{
		if (condition)
		{
			actionOnTrue();
			if (reset)
			{
				condition = false;
			}
		} else
		{
			actionOnFalse?.Invoke();
		}
	}

	private void RespawnOnFrontalTouch()
	{
		var currentTransformPosition = (Vector2)this.currentTransform.position;
		var currentTransformLocalScale = (Vector2)this.transform.localScale;
		var currentTransformRotationZ = this.currentTransform.rotation.z;
		var localScaleOffset = -new Vector2(0.15f, 0.15f);

		var isGlitchingThrough = Physics2D
			.OverlapBoxAll(currentTransformPosition, currentTransformLocalScale + localScaleOffset,
			currentTransformRotationZ)
			.Any(collider => collider.name != this.currentTransform.name && !collider.isTrigger);
		var touchesRight = Physics2D
			.OverlapBoxAll(new Vector2(currentTransformPosition.x + (currentTransformLocalScale.x / 2 + 0.01f) * this.currentConstantVelocity.GetDirection(), currentTransformPosition.y), new Vector2(0, currentTransformLocalScale.y + localScaleOffset.y),
				currentTransformRotationZ)
			.Any(collider =>
				collider.name != this.currentTransform.name && !collider.isTrigger);
		if (!isGlitchingThrough && touchesRight)
		{
			this.Respawn();
		}
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

	private void Respawn()
	{
		Handheld.Vibrate();
		this.progressionPercentageReference.GetComponent<TextMeshProUGUI>().enabled = true;
		Handler.ReloadScene();
	}

	public State GetState()
	{
		return this.state;
	}

	public bool TouchesGround()
	{
		return this.touchesGround;
	}
}
