using System.Collections;
using System.Collections.Generic;
using MyProperties;
using UnityEngine;

public class Follow : MonoBehaviour
{
	[SerializeField] private GameObject toFollow;
	[SerializeField] private bool differentOffsetForStates;
	[SerializeField] [ConditionalField("differentOffsetForStates", true)] private Vector2 offset;
	[SerializeField] [ConditionalField("differentOffsetForStates")] private float smoothSpeed;
	[SerializeField] [ConditionalField("differentOffsetForStates")] private Vector2 jumpOffset;
	[SerializeField] [ConditionalField("differentOffsetForStates")] private Vector2 flyOffset;

	private Transform currentTransform;
	private GameObject toFollowReference;
	private Transform toFollowReferenceTransform;
	private ConstantVelocity toFollowReferenceConstantVelocity;
	private PlayerController toFollowReferencePlayerController;
	private State latestState;
	private float latestStateSwitchAt;
	private Vector2 latestStateSwitchOffset;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();
		this.toFollowReference = GameObject.Find(this.toFollow.name);
		this.toFollowReferenceTransform = this.toFollowReference.GetComponent<Transform>();
		this.toFollowReferenceConstantVelocity = this.toFollowReference.GetComponent<ConstantVelocity>();
		if (differentOffsetForStates)
		{
			this.toFollowReferencePlayerController = this.toFollowReference.GetComponent<PlayerController>();
			latestState = this.toFollowReferencePlayerController.GetState();
		}
	}

	// Start is called before the first frame update
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{
		if (this.differentOffsetForStates)
		{
			var currentTime = Time.time;
			var state = this.toFollowReferencePlayerController.GetState();
			if (this.latestState != state)
			{
				this.latestState = state;
				this.latestStateSwitchAt = currentTime;
				this.latestStateSwitchOffset = this.offset;
			}
			// C# 8 (and therefore switch expressions) aren't supported yet by Unity :(
			//var destinationOffset = this.toFollowReferencePlayerController.GetState() switch
			//{
			//	State.Jump => this.jumpOffset,
			//	State.Fly => this.flyOffset,
			//	_ => Vector2.zero
			//};
			Vector2 destinationOffset;
			switch (state)
			{
				case State.Jump:
					destinationOffset = this.jumpOffset;
					break;
				case State.Fly:
					destinationOffset = this.flyOffset;
					break;
				default:
					destinationOffset = Vector2.zero;
					break;
			}

			this.offset = this.latestStateSwitchOffset - (this.latestStateSwitchOffset - destinationOffset) * (currentTime - this.smoothSpeed < this.latestStateSwitchAt ? (currentTime - this.latestStateSwitchAt) / this.smoothSpeed : 1);
		}

		var toFollowReferenceTransformPosition = this.toFollowReferenceTransform.position;
		toFollowReferenceTransformPosition.z = this.currentTransform.position.z;
		this.currentTransform.position = toFollowReferenceTransformPosition - new Vector3(this.offset.x * this.toFollowReferenceConstantVelocity.GetDirection(), this.offset.y);
	}
}
