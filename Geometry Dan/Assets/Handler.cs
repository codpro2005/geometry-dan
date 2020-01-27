using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Handler : MonoBehaviour
{
	public static int Reverse => -1;
	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{

	}

	// Start is called before the first frame update
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{

	}

    public static void ReloadScene()
    {
	    Physics2D.gravity = new Vector2(0, -9.81f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void ReverseGravity()
    {
	    var gravity = Physics2D.gravity;
		Physics2D.gravity = new Vector2(gravity.x, -gravity.y);
    }

    public static int GetGravityMultiplier()
    {
	    var gravityY = Physics2D.gravity.y;
	    return gravityY < 0 ? 1 : gravityY > 0 ? -1 : 0;
    }
}
