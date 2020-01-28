using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class ProgressionBar
{
	public GameObject gameObject;
	public GameObject xDeterminedBy;
	public GameObject endXDeterminedby;
	public Vector2 offset;
	public Color inactive;
	public Color active;
}

public class UI : MonoBehaviour
{
	[SerializeField] private ProgressionBar progressionBar;

	private Transform currentTransform;

	private Transform xDeterminedByReferenceTransform;
	private float startX;
	private float endX;
	private GameObject inactiveProgression;
	private GameObject activeProgression;
	private float progression;

	private static string Name => "UI";

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();

		this.xDeterminedByReferenceTransform = GameObject.Find(this.progressionBar.xDeterminedBy.transform.name).transform;
		this.startX = this.xDeterminedByReferenceTransform.position.x;
		this.endX = GameObject.Find(this.progressionBar.endXDeterminedby.transform.name).GetComponent<SceneHandler>()
			.GetFinishLineX();

		var progressionBarObject = this.progressionBar.gameObject;
		var progressionBarPosition = (Vector2)currentTransform.position + this.progressionBar.offset;

		this.inactiveProgression =
			UI.CreateUiGameObject(progressionBarObject, progressionBarPosition, Quaternion.identity);
		var inactiveProgressionSpriteRenderer = this.inactiveProgression.GetComponent<SpriteRenderer>();
		inactiveProgressionSpriteRenderer.color = this.progressionBar.inactive;
		inactiveProgressionSpriteRenderer.sortingOrder = 0;

		this.activeProgression =
			UI.CreateUiGameObject(progressionBarObject, progressionBarPosition, Quaternion.identity);
		var activeProgressionSpriteRenderer = this.activeProgression.GetComponent<SpriteRenderer>();
		activeProgressionSpriteRenderer.color = this.progressionBar.active;
		activeProgressionSpriteRenderer.sortingOrder = inactiveProgressionSpriteRenderer.sortingOrder + 1;
	}

	// Start is called before the first frame update
	private void Start()
	{

	}

	// Update is called once per frame
	private void LateUpdate()
	{
		var totalProgression =
			(this.xDeterminedByReferenceTransform.position.x - this.startX) / (this.endX - this.startX);
		this.progression = totalProgression < 0 ? 0 : totalProgression < 1 ? totalProgression : 1;

		var inactiveProgressionTransform = this.inactiveProgression.transform;
		inactiveProgressionTransform.position = (Vector2)this.currentTransform.position + this.progressionBar.offset;

		var inactiveProgressionBarPosition = inactiveProgressionTransform.position;
		var inactiveProgressionBarLocalScaleX = inactiveProgressionTransform.localScale.x;
		var activeProgressionTransform = this.activeProgression.transform;
		activeProgressionTransform.position = new Vector2(inactiveProgressionBarPosition.x - (inactiveProgressionBarLocalScaleX - activeProgressionTransform.localScale.x) / 2, inactiveProgressionBarPosition.y);
		activeProgressionTransform.localScale = new Vector2(inactiveProgressionBarLocalScaleX * this.progression, activeProgressionTransform.localScale.y);
	}

	private static GameObject CreateUiGameObject(GameObject original, Vector2 position, Quaternion rotation)
	{
		var uiGameObject = Instantiate(original, position, rotation);
		uiGameObject.GetComponent<SpriteRenderer>().sortingLayerName = UI.Name;
		return uiGameObject;
	}
}
