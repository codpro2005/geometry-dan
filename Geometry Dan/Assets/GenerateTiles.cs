using System.Collections;
using System.Collections.Generic;
using MyUnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateTiles : MonoBehaviour
{
	[SerializeField] private GameObject loadAround;
	[SerializeField] private Tile tile;
	[SerializeField] private Vector2Int lazyLoadDistance;
	[SerializeField] private int groundHeigth;
	[SerializeField] private int skyHeigth;

	private Transform currentTransform;
	private Tilemap currentTilemap;
	private GameObject loadAroundReference;
	private Vector2Int latestLoadAroundPositionCell;
	private List<Vector3Int> generatedTilesPositionCell;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();
		this.currentTilemap = this.GetComponent<Tilemap>();
		this.loadAroundReference = GameObject.Find(loadAround.name);
	}

	// Start is called before the first frame update
	private void Start()
	{
		this.currentTilemap.SetTile(Vector3Int.zero, this.tile);
		this.currentTilemap.SetTile(Vector3Int.up, this.tile);
	}

	// Update is called once per frame
	private void Update()
	{
		//var loadAroundPositionCell = ((Vector2)this.loadAroundReference.transform.position).ToVector2Int();
		//if (loadAroundPositionCell == this.latestLoadAroundPositionCell) return;
		//this.latestLoadAroundPositionCell = loadAroundPositionCell;
		//var lazyLoadPositionGroundLimit = new Vector2Int(loadAroundPositionCell.x + this.lazyLoadDistance.x, -this.groundHeigth);
		//var newGeneratedTilesPositionCell = new List<Vector3Int>();

		//for (var tileIndexX = -lazyLoadPositionGroundLimit.x; tileIndexX <= lazyLoadPositionGroundLimit.x; tileIndexX++)
		//{
		//	for (var tileIndexY = 0; tileIndexY >= lazyLoadPositionGroundLimit.y; tileIndexY++)
		//	{
		//		var generateTilePositionCell = new Vector3Int(tileIndexX, tileIndexY, this.currentTransform.position.ToVector3Int().z);
		//		newGeneratedTilesPositionCell.Add(generateTilePositionCell);
		//		if (this.generatedTilesPositionCell.Contains(generateTilePositionCell)) continue;
		//		this.currentTilemap.SetTile(generateTilePositionCell, this.tile);
		//	}
		//}

		//this.generatedTilesPositionCell = newGeneratedTilesPositionCell;
	}
}
