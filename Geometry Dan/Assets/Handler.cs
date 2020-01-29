using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Handler : MonoBehaviour
{
	public static int Reverse => -1;
	public static Stack<Scene> LoadedScenes { get; set; }

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		Handler.LoadedScenes = new Stack<Scene>();
	}

	// Start is called before the first frame update
	private void Start()
	{
		DontDestroyOnLoad(this);
	}

	// Update is called once per frame
	private void Update()
	{
		if (Input.touches.Length > 1)
		{
			Handler.LoadAndPopSceneInPool();
		}
	}

	private static void PushCurrentSceneInPool()
	{
		Handler.LoadedScenes.Push(SceneManager.GetActiveScene());
	}

	private static void LoadAndPopSceneInPool()
	{
		if (Handler.LoadedScenes.Count > 0)
		{
			SceneManager.LoadScene(Handler.LoadedScenes.Pop().name);
		}
	}

    public static void ReloadScene()
    {
	    Physics2D.gravity = new Vector2(0, -9.81f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void LoadNextScene()
    {
		Handler.PushCurrentSceneInPool();
	    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
