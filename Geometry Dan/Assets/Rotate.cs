using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	[SerializeField] private float speed;

	private Transform currentTransform;
	private GameObject parent;
	private PlayerController parentPlayerController;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();
		this.parent = this.currentTransform.parent.gameObject;
		this.parentPlayerController = this.parent.GetComponent<PlayerController>();
	}

	// Start is called before the first frame update
	private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
	    if (this.parentPlayerController.isPaused()) return;
	    if (this.parentPlayerController.TouchesGround() || this.parentPlayerController.GetState() != State.Jump)
	    {
		    this.currentTransform.rotation = Quaternion.identity;
		    return;
		}
	    this.currentTransform.Rotate(Vector3.back * this.speed * this.parent.GetComponent<ConstantVelocity>().GetDirection());
    }
}
