using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneHandler : MonoBehaviour
{
	[SerializeField] private float gameSpeed = 1;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		Time.timeScale = this.gameSpeed;
	}

	// Start is called before the first frame update
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{

	}
}