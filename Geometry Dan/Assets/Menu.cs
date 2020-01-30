using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
	public void Load(string sceneName)
	{
		Handler.LoadSceneByName(sceneName);
	}

	public void Quit()
	{
		Handler.Quit();
	}
}
