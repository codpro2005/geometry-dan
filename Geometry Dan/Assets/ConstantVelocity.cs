using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantVelocity : MonoBehaviour
{
	[SerializeField]
	private Vector2 velocity;

	private Rigidbody2D currentRigidbody2D;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentRigidbody2D = this.GetComponent<Rigidbody2D>();
	}

	// Start is called before the first frame update
	private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void FixedUpdate()
    {
	    this.currentRigidbody2D.position += this.velocity;
    }
}
