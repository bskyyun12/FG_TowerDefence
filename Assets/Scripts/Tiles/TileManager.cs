using AI;
using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class TileManager : MonoBehaviour
{
	public Vector2Int size;

	Dictionary<TileData, IPool<GameObject>> tileDataByPool;

	private void Awake()
	{
		LevelStart.TileManager_Initialize += Initialize;
	}

	private void Start()
	{
		tileDataByPool = new Dictionary<TileData, IPool<GameObject>>();
		
		var tileEnumArray = Enum.GetValues(typeof(TileType));
		for (int i = 0; i < tileEnumArray.Length; i++)
		{
			TileType tileType = (TileType)tileEnumArray.GetValue(i);
			TileData tile = TileMethods.GetTile(tileType);		

			tileDataByPool.Add(tile, new TilePool(tile.prefab, transform));
		}
	}

	public void Initialize(LevelStart levelStart)
	{
		foreach (var tileData in tileDataByPool.Keys)
		{
			tileDataByPool[tileData].ReturnToPool(tileData.prefab);
		}

		size = MapData.GetMapSize();
		Astar.SetGrid(size);

		CreateBoard();
	}

	public void CreateBoard()
	{
		// set worldStart topleft
		Vector3 worldStart = new Vector3((transform.position.x - size.x + 1), (transform.position.y + size.y - 1));

		// y axis
		for (int y = 0; y < size.y; y++)
		{
			// separate numbers one by one
			char[] newTiles = MapData.GetTileRowData(y).ToCharArray();
			// x axis
			for (int x = 0; x < size.x; x++)
			{
				int key = (int)Char.GetNumericValue(newTiles[x]);
				TileType tileType = (TileType)key;
				PlaceTile(tileType, x, y, worldStart);
			}
		}
	}

	private void PlaceTile(TileType tileType, int x, int y, Vector3 worldStart)
	{
		TileData tile = TileMethods.GetTile(tileType);
		if (tile.prefab)
		{
			GameObject newTile = tileDataByPool[tile].Get();
			newTile.SetActive(true);

			if (newTile)
			{
				newTile.transform.localPosition = new Vector3(worldStart.x + x * 2, 0f, worldStart.y - y * 2);

				// Astar
				Astar.AddNode(newTile.transform.localPosition, x, y, tile.walkable);
				if (tileType == TileType.Start)
				{
					MapData.startTile.x = x;
					MapData.startTile.y = y;
				}
				else if (tileType == TileType.End)
				{
					MapData.endTile.x = x;
					MapData.endTile.y = y;
				}
			}
		}
	}
}
