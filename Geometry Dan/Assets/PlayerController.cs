using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum State
{
    Run,
    Fly
}

//public class Active
//{
//    public bool isActive;
//}

//public class Active<T>
//{
//    public bool isActive;
//    public T Value;
//}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Vector2 xForce;
    [SerializeField] private bool enableActionOnTouch;
    [SerializeField] private bool enableActionOnScream;
    [SerializeField] [ConditionalField("enableJumpOnScream")] private float jumpThreshold;
    [SerializeField] private GameObject jumpResetOnCollideWith;
    [SerializeField] private GameObject killOnCollideWith;
    [SerializeField] private GameObject triggerFlyOnCollideWith;

    private Rigidbody2D currentRigidbody2D;

    private State state;
    private GameObject jumpResetOnCollideWithReference;
    private GameObject killOnCollideWithReference;
    private GameObject triggerFlyOnCollideWithReference;
    private AudioClip microphoneInput;
    private bool microphoneExists;
    private bool touchPhaseBegan;
    private float volume;
    private bool touchesGround;
    private Handler handler;

    // Awake is called before Start and should be used as the constructor
    private void Awake()
    {
        this.currentRigidbody2D = this.GetComponent<Rigidbody2D>();
        this.jumpResetOnCollideWithReference = GameObject.Find(this.jumpResetOnCollideWith.name);
        this.killOnCollideWithReference = GameObject.Find(this.killOnCollideWith.name);
        this.triggerFlyOnCollideWithReference = GameObject.Find(this.triggerFlyOnCollideWith.name);
        this.handler = GameObject.Find("Handler").GetComponent<Handler>();
        if (Microphone.devices.Length > 0)
        {
            this.microphoneExists = true;
            this.microphoneInput = Microphone.Start(Microphone.devices[0], true, 999, 44100);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        var currentMicPosition = Microphone.GetPosition(null);
        if (microphoneExists && currentMicPosition > 0)
        {
            int dec = 128;
            float[] waveData = new float[dec];
            int micPosition = currentMicPosition - (dec + 1);
            this.microphoneInput.GetData(waveData, micPosition);
            this.volume = Mathf.Sqrt(Mathf.Sqrt(waveData.Select(single => Mathf.Pow(single, 2)).Max()));
        }

        if (Input.touches.Any(touch => touch.phase == TouchPhase.Began))
	    {
		    this.touchPhaseBegan = true;
	    }

    }

    private void FixedUpdate()
    {
        switch (this.state)
        {
            case State.Run:
                if (this.touchesGround && ((this.enableActionOnTouch && this.touchPhaseBegan) || (this.enableActionOnScream && this.volume > this.jumpThreshold)))
                {
                    this.currentRigidbody2D.AddForce(this.xForce);
                    this.touchPhaseBegan = false;
                    this.volume = 0;
                }
                break;
            case State.Fly:
                if (this.enableActionOnScream && this.volume > this.jumpThreshold)
                {
                    Debug.Log(this.xForce * this.volume - new Vector2(0, 0.5f));
                    this.currentRigidbody2D.AddForce(this.xForce * this.volume - new Vector2(0, 0.5f));
                    this.volume = 0;
                }
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision2D)
    {
	    var collision2DTransfromTag = collision2D.transform.tag;
	    if (collision2DTransfromTag == this.jumpResetOnCollideWithReference.tag)
	    {
		    this.touchesGround = true;
	    } else
        if (collision2DTransfromTag == this.killOnCollideWithReference.tag)
        {
            this.handler.ReloadScene();
        } else
        if (collision2DTransfromTag == this.triggerFlyOnCollideWithReference.tag)
        {
            this.state = this.state == State.Fly ? State.Run : State.Fly;
        }
    }

    private void OnCollisionExit2D(Collision2D collision2D)
    {
	    var collision2DTransfromTag = collision2D.transform.tag;
	    if (collision2DTransfromTag == this.jumpResetOnCollideWithReference.tag)
	    {
		    this.touchesGround = false;
	    }
    }
}
