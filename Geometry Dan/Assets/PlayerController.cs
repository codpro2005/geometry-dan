using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Vector2 xForce;
    [SerializeField] private GameObject jumpResetOnCollideWith;

    private Rigidbody2D currentRigidbody2D;
    private GameObject jumpResetOnCollideWithReference;
    private bool triggerForce;
    private bool jumpReset;

    // Awake is called before Start and should be used as the constructor
    private void Awake()
    {
        this.currentRigidbody2D = this.GetComponent<Rigidbody2D>();
        this.jumpResetOnCollideWithReference = GameObject.Find(this.jumpResetOnCollideWith.name);
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
	    if (Input.touches.Any(touch => touch.phase == TouchPhase.Began))
	    {
		    this.triggerForce = true;
	    }

    }

    private void FixedUpdate()
    {
        if (this.triggerForce && this.jumpReset)
        {
            this.currentRigidbody2D.AddForce(this.xForce);
            this.triggerForce = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
	    var collision2DTransfromTag = collision2D.transform.tag;
	    if (collision2DTransfromTag == this.jumpResetOnCollideWithReference.tag)
	    {
		    this.jumpReset = true;
	    }
    }

    private void OnCollisionExit2D(Collision2D collision2D)
    {
	    var collision2DTransfromTag = collision2D.transform.tag;
	    if (collision2DTransfromTag == this.jumpResetOnCollideWithReference.tag)
	    {
		    this.jumpReset = false;
	    }
    }
}
