using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadOnTrigger : MonoBehaviour
{
	private Transform currentTransform;
	private GameObject parent;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();
		this.parent = this.currentTransform.parent.gameObject;
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
	    if (collider2D.transform.name == this.parent.transform.name || collider2D.isTrigger) return;
        Handler.ReloadScene();
    }
}
