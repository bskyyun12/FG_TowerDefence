using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class TileMethods
{
	private static Dictionary<TileType, TileData> tilesByType;
	private static bool IsInitialized => tilesByType != null;

	private static void InitializeFactory()
	{
		if (IsInitialized)
		{ return; }

		tilesByType = new Dictionary<TileType, TileData>();

		var tileClasses = GetAllInstances<TileData>();
		foreach (var tileClass in tileClasses)
		{
			tilesByType.Add(tileClass.tileType, tileClass);
		}

	}

	private static T[] GetAllInstances<T>() where T : ScriptableObject
	{
		// search for a ScriptObject called typeof(T).Name
		string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
		T[] a = new T[guids.Length];
		for (int i = 0; i < guids.Length; i++)
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
		}

		return a;
	}

	public static TileData GetTile(TileType type)
	{
		InitializeFactory();

		if (tilesByType.ContainsKey(type))
		{
			return tilesByType[type];
		}

		return null;
	}

	public static GameObject GetPrefabByType(TileType type)
	{
		InitializeFactory();

		if (tilesByType.ContainsKey(type))
		{
			return tilesByType[type].prefab;
		}

		return null;
	}

	public static IEnumerable<TileType> GetAllTileTypes()
	{
		InitializeFactory();
		return tilesByType.Keys;
	}
}
