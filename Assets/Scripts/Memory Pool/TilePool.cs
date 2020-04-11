using System.Collections.Generic;
using Tools;
using UnityEngine;

public class TilePool : IPool<GameObject>
{
	private Queue<GameObject> tileQueue = new Queue<GameObject>();
	public List<GameObject> pooledTileList;

	GameObject tilePrefab;
	Transform parent;

	public TilePool(GameObject tilePrefab, Transform parent)
	{
		pooledTileList = new List<GameObject>();
		this.tilePrefab = tilePrefab;
		this.parent = parent;
	}

	public GameObject Get()
	{
		if (tileQueue.Count == 0)
		{
			GameObject newTile = GameObject.Instantiate(tilePrefab, parent);
			tileQueue.Enqueue(newTile);
			pooledTileList.Add(newTile);
		}

		return tileQueue.Dequeue();
	}

	public void ReturnToPool(GameObject tile)
	{
		for (int i = 0; i < pooledTileList.Count; i++)
		{
			if (pooledTileList[i].activeInHierarchy)
			{
				pooledTileList[i].SetActive(false);
				tileQueue.Enqueue(pooledTileList[i]);
			}
		}
	}
}
