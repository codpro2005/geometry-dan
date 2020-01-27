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
	    this.progression = (this.xDeterminedByReferenceTransform.position.x - this.startX) / (this.endX - this.startX);

	    var progressionBarPosition = (Vector2)this.currentTransform.position + this.progressionBar.offset;
	    var inactiveProgressionTransform = this.inactiveProgression.transform;
	    inactiveProgressionTransform.position = progressionBarPosition;

	    var activeProgressionTransform = this.activeProgression.transform;
	    activeProgressionTransform.position = progressionBarPosition;
	    activeProgressionTransform.localScale = new Vector2(inactiveProgressionTransform.localScale.x * this.progression, activeProgressionTransform.localScale.y);
	}

    private static GameObject CreateUiGameObject(GameObject original, Vector2 position, Quaternion rotation)
    {
	    var uiGameObject = Instantiate(original, position, rotation);
	    uiGameObject.GetComponent<SpriteRenderer>().sortingLayerName = UI.Name;
	    return uiGameObject;
    }
}
