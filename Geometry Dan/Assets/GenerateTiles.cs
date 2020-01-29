using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyProperties;
using MyUnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateTiles : MonoBehaviour
{
	[SerializeField] private GameObject loadAround;
	[SerializeField] private bool loadGround;
	[SerializeField] [ConditionalField("loadGround")] private int groundHeigth;
	[SerializeField] private bool loadSky;
	[SerializeField] [ConditionalField("loadSky")] private int skyHeigth;
	[SerializeField] private Tile tile;
	[SerializeField] private Vector2Int lazyLoadDistance;

	private Transform currentTransform;
	private Tilemap currentTilemap;
	private GameObject loadAroundReference;
	private List<Vector3Int> generatedTilesCellPosition;
	private int latestLoadAroundCellPositionX;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTransform = this.GetComponent<Transform>();
		this.currentTilemap = this.GetComponent<Tilemap>();
		this.loadAroundReference = GameObject.Find(loadAround.name);
		this.generatedTilesCellPosition = new List<Vector3Int>();
	}

	// Start is called before the first frame update
	private void Start()
	{

	}

	// Update is called once per frame
	private void Update()
	{
		var loadAroundCellPositionX = (int)this.loadAroundReference.transform.position.x;
		if (loadAroundCellPositionX == this.latestLoadAroundCellPositionX) return;
		this.latestLoadAroundCellPositionX = loadAroundCellPositionX;
		var newGeneratedTilesCellPosition = new List<Vector3Int>();
		this.SetAllActiveTiles(loadAroundCellPositionX, newGeneratedTilesCellPosition);
		this.DeleteAllInactiveTiles(newGeneratedTilesCellPosition);
		this.generatedTilesCellPosition = newGeneratedTilesCellPosition;
	}

	private void SetAllActiveTiles(int startingPointX, ICollection<Vector3Int> tileCollection)
	{
		for (var tileIndexX = startingPointX - lazyLoadDistance.x; tileIndexX <= startingPointX + this.lazyLoadDistance.x; tileIndexX++)
		{
			if (loadGround)
			{
				for (var tileIndexY = this.groundHeigth; tileIndexY >= this.groundHeigth - this.lazyLoadDistance.y; tileIndexY--)
				{
					this.SetSingleTile(new Vector3Int(tileIndexX, tileIndexY, this.currentTransform.position.ToVector3Int().z), tileCollection);
				}
			}

			if (loadSky)
			{
				for (var tileIndexY = this.skyHeigth; tileIndexY <= this.skyHeigth + this.lazyLoadDistance.y; tileIndexY++)
				{
					this.SetSingleTile(new Vector3Int(tileIndexX, tileIndexY, this.currentTransform.position.ToVector3Int().z), tileCollection);
				}
			}
		}
	}

	private void SetSingleTile(Vector3Int generateTileCellPosition, ICollection<Vector3Int> extendCollectionByNewTile = null)
	{
		extendCollectionByNewTile?.Add(generateTileCellPosition);
		if (this.generatedTilesCellPosition.Contains(generateTileCellPosition)) return;
		this.currentTilemap.SetTile(generateTileCellPosition, this.tile);
	}

	private void DeleteAllInactiveTiles(ICollection<Vector3Int> tileCollection)
	{
		foreach (var toDeleteTileCellPosition in this.generatedTilesCellPosition.Where(generatedTileCellPosition => !tileCollection.Contains(generatedTileCellPosition)))
		{
			this.currentTilemap.SetTile(toDeleteTileCellPosition, null);
		}
	}
}
