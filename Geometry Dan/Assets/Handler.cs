using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Handler : MonoBehaviour
{
	public static int Reverse => -1;
	public static Stack<int> LoadedScenes { get; set; }
	private static bool previousSceneLoaded;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		Handler.LoadedScenes = new Stack<int>();
	}

	// Start is called before the first frame update
	private void Start()
	{
		DontDestroyOnLoad(this);
	}

	// Update is called once per frame
	private void Update()
	{
		var haha = Handler.LoadedScenes.Aggregate(string.Empty, (current, loadedScene) => current + loadedScene);
		Debug.Log(haha);

		var totalTouches = Input.touches.Length;
		if (totalTouches == 0)
		{
			Handler.previousSceneLoaded = false;
		}
		else
		if (totalTouches > 1 && !previousSceneLoaded)
		{
			Handler.previousSceneLoaded = true;
			Handler.LoadAndPopSceneInPool();
		}
	}

	private static void ToDefault()
	{
		Time.timeScale = 1;
		Physics2D.gravity = new Vector2(0, -9.81f);
	}

	private static void LoadScene(int buildIndex)
	{
		Handler.ToDefault();
		SceneManager.LoadScene(buildIndex);
	}

	private static void LoadScene(string name)
	{
		Handler.ToDefault();
		SceneManager.LoadScene(name);
	}

	private static void PushCurrentSceneInPool()
	{
		Handler.LoadedScenes.Push(SceneManager.GetActiveScene().buildIndex);
	}

	private static void LoadAndPopSceneInPool()
	{
		if (Handler.LoadedScenes.Count > 0)
		{
			Handler.LoadScene(Handler.LoadedScenes.Pop());
		}
	}

	public static void ReloadScene()
	{
		Handler.LoadScene(SceneManager.GetActiveScene().name);
	}

	public static void LoadNextScene()
	{
		Handler.PushCurrentSceneInPool();
		Handler.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public static void LoadSceneByName(string name)
	{
		Handler.PushCurrentSceneInPool();
		Handler.LoadScene(name);
	}

	public static void Quit()
	{
		Application.Quit();
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
