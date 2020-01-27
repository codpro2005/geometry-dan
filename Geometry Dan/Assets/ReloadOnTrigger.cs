using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Yo, this shit deprecated before it was even finished lol
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
		// Doesn't always work either because lowest ground sometimes isn't the generated ground.
		var allColliding = Physics2D.OverlapBoxAll(this.currentTransform.position,
			new Vector2(0, this.transform.localScale.x), this.transform.rotation.z);

		if (allColliding.Length == 0 || !allColliding.Any(collider2D =>
			!(collider2D.transform.name == this.parent.transform.name || collider2D.transform.name == this.currentTransform.name || collider2D.isTrigger ||
			this.parent.transform.position.y < -0.486))) return;
		Handheld.Vibrate();
		Handler.ReloadScene();
	}

	private void OnTriggerEnter2D(Collider2D collider2D)
	{
		// Can not do this because if player falls exactly on the ground when an object is in front of him the last condition will be true (this.parent.transform.position lower) and since this method won't be called again until next collision, game wouldn't restart.
		//if (collider2D.transform.name == this.parent.transform.name || collider2D.isTrigger || this.parent.transform.position.y < -0.486) return;
		//Handheld.Vibrate();
		//Handler.ReloadScene();
	}
}
