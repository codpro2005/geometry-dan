using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Vector2 xForce;

    private Rigidbody2D playerRigidbody2D;
    private bool triggerForce;

    // Awake is called before Start and should be used as the constructor
    private void Awake()
    {
        this.playerRigidbody2D = this.GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (new List<Touch>(Input.touches).Any(touch => touch.phase == TouchPhase.Began))
        {
            this.triggerForce = true;
        }
    }

    private void FixedUpdate()
    {
        if (this.triggerForce)
        {
            this.playerRigidbody2D.AddForce(this.xForce);
            this.triggerForce = false;
        }
    }
}
