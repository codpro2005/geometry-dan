using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToCamera : MonoBehaviour
{
	[SerializeField]
	private GameObject camera;

	private Transform currentTransform;
	private GameObject cameraReference;
	private Camera cameraReferenceCamera;

    // Awake is called before Start and should be used as the constructor
    private void Awake()
    {
	    this.currentTransform = this.GetComponent<Transform>();
        this.cameraReference = GameObject.Find(camera.name);
        this.cameraReferenceCamera = this.cameraReference.GetComponent<Camera>();
    }
    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
	    this.currentTransform.position = (Vector2)this.cameraReference.transform.position;
	    var height = 2f * this.cameraReferenceCamera.orthographicSize;
	    this.currentTransform.localScale = new Vector2(height * this.cameraReferenceCamera.aspect, height);
    }
}
