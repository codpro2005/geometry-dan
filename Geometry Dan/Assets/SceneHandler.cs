using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHandler : MonoBehaviour
{
	[SerializeField] private float positionX;
	[SerializeField] private GameObject loadNextSceneOnPass;

	private GameObject loadNextSceneOnPassReference;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.loadNextSceneOnPassReference = GameObject.Find(this.loadNextSceneOnPass.name);
	}

	// Start is called before the first frame update
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{
		if (this.loadNextSceneOnPassReference.transform.position.x >= this.positionX)
		{
			Handler.LoadNextScene();
		}
	}

	public float GetFinishLineX()
	{
		return positionX;
	}
}