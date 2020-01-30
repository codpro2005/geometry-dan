using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

[Serializable]
class ProgressionBar
{
	public GameObject gameObject;
	public GameObject xDeterminedBy;
	public GameObject endXDeterminedby;
	public GameObject valueToModify;
	public Vector2 offset;
	public Color inactive;
	public Color active;
}

public class UI : MonoBehaviour
{
	[SerializeField] private ProgressionBar progressionBar;

	private Transform currentTransform;
	private Camera currentCamera;
	private GameObject valueToModifyReference;
	private Transform xDeterminedByReferenceTransform;
	private float startX;
	private float endX;
	private float cameraStartSize;
	private GameObject inactiveProgression;
	private Vector2 startProgressionScale;
	private GameObject activeProgression;
	private float progression;
	private float cameraSizeMultiplier => this.currentCamera.orthographicSize / this.cameraStartSize;
	private static string Name => "UI";

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();
		this.currentCamera = this.GetComponent<Camera>();
		this.valueToModifyReference = GameObject.Find(this.progressionBar.valueToModify.transform.name);
		this.xDeterminedByReferenceTransform = GameObject.Find(this.progressionBar.xDeterminedBy.transform.name).transform;
		this.startX = this.xDeterminedByReferenceTransform.position.x;
		this.endX = GameObject.Find(this.progressionBar.endXDeterminedby.transform.name).GetComponent<PlayerController>()
			.GetFinishLineX();
		this.cameraStartSize = this.currentCamera.orthographicSize;
	}

	// Start is called before the first frame update
	private void Start()
	{
		var progressionBarObject = this.progressionBar.gameObject;
		var progressionBarPosition = (Vector2)currentTransform.position + this.progressionBar.offset;
		var identity = Quaternion.identity;

		UI.CreateProgressionGameObject(out this.inactiveProgression, progressionBarObject, progressionBarPosition, identity, this.progressionBar.inactive, 0);
		this.startProgressionScale = this.inactiveProgression.transform.localScale;
		UI.CreateProgressionGameObject(out this.activeProgression, progressionBarObject, progressionBarPosition, identity, this.progressionBar.active, inactiveProgression.GetComponent<SpriteRenderer>().sortingOrder + 1);
	}

	// Update is called once per frame
	private void LateUpdate()
	{
		var totalProgression =
			(this.xDeterminedByReferenceTransform.position.x - this.startX) / (this.endX - this.startX);
		this.progression = totalProgression < 0 ? 0 : totalProgression < 1 ? totalProgression : 1;

		var inactiveProgressionTransform = this.inactiveProgression.transform;
		inactiveProgressionTransform.position = (Vector2)this.currentTransform.position + this.progressionBar.offset * this.cameraSizeMultiplier;
		inactiveProgressionTransform.localScale = this.startProgressionScale * this.cameraSizeMultiplier;

		var inactiveProgressionBarPosition = inactiveProgressionTransform.position;
		var inactiveProgressionBarLocalScaleX = inactiveProgressionTransform.localScale.x;
		var activeProgressionTransform = this.activeProgression.transform;
		activeProgressionTransform.position = new Vector2(inactiveProgressionBarPosition.x - (inactiveProgressionBarLocalScaleX - activeProgressionTransform.localScale.x) / 2, inactiveProgressionBarPosition.y);
		activeProgressionTransform.localScale = new Vector2(inactiveProgressionBarLocalScaleX * this.progression, inactiveProgressionTransform.localScale.y);

		var valueToModifyTmpUgui = this.valueToModifyReference.GetComponent<TextMeshProUGUI>();
		if (valueToModifyTmpUgui.enabled)
		{
			this.valueToModifyReference.GetComponent<TextMeshProUGUI>().text = $"{this.progression * 100:0.0}%";
		}
	}

	private static GameObject CreateUiGameObject(GameObject original, Vector2 position, Quaternion rotation)
	{
		var uiGameObject = Instantiate(original, position, rotation);
		uiGameObject.GetComponent<SpriteRenderer>().sortingLayerName = UI.Name;
		return uiGameObject;
	}

	private static void CreateProgressionGameObject(out GameObject toBeCreated, GameObject original, Vector2 position,
		Quaternion rotation, Color color, int sorthingOrder)
	{
		toBeCreated =
			UI.CreateUiGameObject(original, position, rotation);
		var inactiveProgressionSpriteRenderer = toBeCreated.GetComponent<SpriteRenderer>();
		inactiveProgressionSpriteRenderer.color = color;
		inactiveProgressionSpriteRenderer.sortingOrder = sorthingOrder;
	}
}
