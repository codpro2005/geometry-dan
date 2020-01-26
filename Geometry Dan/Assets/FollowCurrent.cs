using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Yo, this shit deprecated
public class FollowCurrent : MonoBehaviour
{
	[SerializeField] private GameObject camera;
	[SerializeField] private Vector2 offset;

	private Transform currentTransform;
	private ConstantVelocity currentConstantVelocity;
	private GameObject cameraReference;
	private Transform cameraReferenceTransform;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();
		this.currentConstantVelocity = this.GetComponent<ConstantVelocity>();
		this.cameraReference = GameObject.Find(this.camera.name);
		this.cameraReferenceTransform = this.cameraReference.GetComponent<Transform>();
	}
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
	    var currentTransformPosition = this.currentTransform.position;
	    currentTransformPosition.z = this.cameraReferenceTransform.position.z;
	    this.cameraReferenceTransform.position = currentTransformPosition - new Vector3(this.offset.x * this.currentConstantVelocity.GetDirection(), this.offset.y);
    }
}
