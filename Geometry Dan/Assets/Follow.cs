using System.Collections;
using System.Collections.Generic;
using MyProperties;
using UnityEngine;

public class Follow : MonoBehaviour
{
	[SerializeField] private GameObject toFollow;
	[SerializeField] private bool differentSettingsForStates;
	[SerializeField] [ConditionalField("differentSettingsForStates", true)] private Vector2 offset;
	[SerializeField] [ConditionalField("differentSettingsForStates", true)] private float sizeMultiplier = 1;
	[SerializeField] [ConditionalField("differentSettingsForStates")] private float smoothSpeed;
	[SerializeField] [ConditionalField("differentSettingsForStates")] private Vector2 jumpOffset;
	[SerializeField] [ConditionalField("differentSettingsForStates")] private float jumpSizeMultiplier = 1;
	[SerializeField] [ConditionalField("differentSettingsForStates")] private Vector2 flyOffset;
	[SerializeField] [ConditionalField("differentSettingsForStates")] private float flySizeMultiplier = 1;
	[SerializeField] [ConditionalField("differentSettingsForStates")] private Vector2 glideOffset;
	[SerializeField] [ConditionalField("differentSettingsForStates")] private float glideSizeMultiplier = 1;

	private Transform currentTransform;
	private Camera currentCamera;
	private float cameraStartSize;
	private GameObject toFollowReference;
	private Transform toFollowReferenceTransform;
	private ConstantVelocity toFollowReferenceConstantVelocity;
	private PlayerController toFollowReferencePlayerController;
	private State latestState;
	private float latestStateSwitchAt;
	private Vector2 latestStateSwitchOffset;
	private float latestSizeMultiplier;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();
		this.currentCamera = this.GetComponent<Camera>();
		this.cameraStartSize = this.currentCamera.orthographicSize;
		this.toFollowReference = GameObject.Find(this.toFollow.name);
		this.toFollowReferenceTransform = this.toFollowReference.GetComponent<Transform>();
		this.toFollowReferenceConstantVelocity = this.toFollowReference.GetComponent<ConstantVelocity>();
		if (!differentSettingsForStates) return;
		this.toFollowReferencePlayerController = this.toFollowReference.GetComponent<PlayerController>();
		this.latestState = this.toFollowReferencePlayerController.GetState();
		this.SetSettingsDependingOnStates(this.latestState, out this.offset, out this.sizeMultiplier);
		this.latestStateSwitchOffset = this.offset;
		this.latestSizeMultiplier = this.sizeMultiplier;
	}

	// Start is called before the first frame update
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{
		if (this.differentSettingsForStates)
		{
			var currentTime = Time.time;
			var state = this.toFollowReferencePlayerController.GetState();
			if (this.latestState != state)
			{
				this.latestState = state;
				this.latestStateSwitchAt = currentTime;
				this.latestStateSwitchOffset = this.offset;
				this.latestSizeMultiplier = this.sizeMultiplier;
			}
			// C# 8 (and therefore switch expressions) aren't supported yet by Unity :(
			//var destinationOffset = this.toFollowReferencePlayerController.GetState() switch
			//{
			//	State.Jump => this.jumpOffset,
			//	State.Fly => this.flyOffset,
			//	_ => Vector2.zero
			//};

			this.SetSettingsDependingOnStates(state, out var destinationOffset, out var destinationSizeMultiplier);

			var timeMultiplier = currentTime - this.smoothSpeed < this.latestStateSwitchAt
				? (currentTime - this.latestStateSwitchAt) / this.smoothSpeed
				: 1;

			this.offset = this.latestStateSwitchOffset - (this.latestStateSwitchOffset - destinationOffset) * timeMultiplier;
			this.sizeMultiplier = this.latestSizeMultiplier - (this.latestSizeMultiplier - destinationSizeMultiplier) * timeMultiplier;
		}

		var toFollowReferenceTransformPosition = this.toFollowReferenceTransform.position;
		toFollowReferenceTransformPosition.z = this.currentTransform.position.z;
		this.currentTransform.position = toFollowReferenceTransformPosition - new Vector3(this.offset.x * this.toFollowReferenceConstantVelocity.GetDirection(), this.offset.y);
		this.currentCamera.orthographicSize = this.cameraStartSize * this.sizeMultiplier;
	}

	private void SetSettingsDependingOnStates(State state, out Vector2 offset, out float sizeMultiplier)
	{
		switch (state)
		{
			case State.Jump:
				offset = this.jumpOffset;
				sizeMultiplier = this.jumpSizeMultiplier;
				break;
			case State.Fly:
				offset = this.flyOffset;
				sizeMultiplier = this.flySizeMultiplier;
				break;
			case State.Glide:
				offset = this.glideOffset;
				sizeMultiplier = this.glideSizeMultiplier;
				break;
			default:
				offset = Vector2.zero;
				sizeMultiplier = 1;
				break;
		}
	}
}
