using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTeleportExit : MonoBehaviour
{
	[SerializeField] private GameObject teleportExit;
	[SerializeField] private Vector2 position;
    [SerializeField] private bool teleportAtStart;

	private GameObject teleportExitReference;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.teleportExitReference = Instantiate(teleportExit, this.position, Quaternion.identity);
	}

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
	    var collider2DTransform = collider2D.transform;
        var teleportExitReferenceTransformPosition = teleportExitReference.transform.position;
        collider2DTransform.position = new Vector2(teleportExitReferenceTransformPosition.x - (collider2DTransform.localScale.x / 2) * (this.teleportAtStart ? 1 : -1), teleportExitReferenceTransformPosition.y);
    }
}
