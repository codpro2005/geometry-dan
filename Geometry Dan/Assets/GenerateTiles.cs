using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateTiles : MonoBehaviour
{
	[SerializeField] private Tile tile;
	private Tilemap currentTilemap;

	// Awake is called before Start and should be used as the constructor
	private void Awake()
	{
		this.currentTilemap = this.GetComponent<Tilemap>();
	}

    // Start is called before the first frame update
    private void Start()
    {
        this.currentTilemap.SetTile(Vector3Int.zero, this.tile);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
