using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Data")]
public class TileData : ScriptableObject
{
	public GameObject prefab;
	public TileType tileType;
	public bool walkable;
}