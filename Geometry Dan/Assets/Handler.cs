using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Handler : MonoBehaviour
{
    public static int Reverse => -1;
	public static Stack<int> LoadedScenes { get; set; }
	public static bool TouchEnabled { get; set; }
	public static bool VolumeEnabled { get; set; }
	public static bool TiltEnabled { get; set; }
	private static bool previousSceneLoaded { get; set; }

	static Handler()
	{
		Handler.LoadedScenes = new Stack<int>();
		Handler.TouchEnabled = true;
		Handler.VolumeEnabled = false;
		Handler.TiltEnabled = false;
		Handler.previousSceneLoaded = false;
	}

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{

	}

	// Start is called before the first frame update
	private void Start()
	{
		DontDestroyOnLoad(this);
	}

	// Update is called once per frame
	private void Update()
	{
        //Debug.Log(Handler.LoadedScenes.Aggregate("", (start, extendBy) => start + " " + extendBy));
		var totalTouches = Input.touches.Length;
		if (totalTouches == 0)
		{
			Handler.previousSceneLoaded = false;
		} else
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

	public static void LoadNextScene(Action onLast)
	{
        var nextSceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextSceneBuildIndex)
        {
            Handler.PushCurrentSceneInPool();
            Handler.LoadScene(nextSceneBuildIndex);
            return;
        }
        onLast();
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
