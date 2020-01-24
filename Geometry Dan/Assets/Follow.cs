using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
	[SerializeField] private GameObject toFollow;
	[SerializeField] private Vector2 offset;

	private Transform currentTransform;
	private GameObject toFollowReference;
	private Transform toFollowReferenceTransform;
	private ConstantVelocity toFollowReferenceConstantVelocity;
	private int direction;
	private Vector3 directionBasedOffset;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();
		this.toFollowReference = GameObject.Find(this.toFollow.name);
		this.toFollowReferenceTransform = this.toFollowReference.GetComponent<Transform>();
		this.toFollowReferenceConstantVelocity = this.toFollowReference.GetComponent<ConstantVelocity>();
		this.direction = this.toFollowReferenceConstantVelocity.GetDirection();
		this.directionBasedOffset = new Vector2(this.offset.x * this.direction, this.offset.y);
	}

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
	    var toFollowReferenceTransformPosition = this.toFollowReferenceTransform.position;
	    toFollowReferenceTransformPosition.z = this.currentTransform.position.z;
	    this.currentTransform.position = toFollowReferenceTransformPosition - this.directionBasedOffset;
    }
}
