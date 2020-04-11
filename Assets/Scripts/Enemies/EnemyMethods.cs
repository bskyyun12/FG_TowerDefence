using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EnemyMethods
{
	private static Dictionary<EnemyType, EnemyBase> enemiesByType;
	private static bool IsInitialized => enemiesByType != null;

	private static void InitializeFactory()
	{
		if (IsInitialized)
		{ return; }

		enemiesByType = new Dictionary<EnemyType, EnemyBase>();

		foreach (var enemy in GetAllInstances<EnemyBase>())
		{
			enemiesByType.Add(enemy.EnemyType, enemy);
		}
	}

	private static List<T> GetAllInstances<T>() where T : UnityEngine.Object
	{
		List<T> assets = new List<T>();
		string[] guids = AssetDatabase.FindAssets("t:Prefab");
		for (int i = 0; i < guids.Length; i++)
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			T asset = AssetDatabase.LoadAssetAtPath<T>(path);
			if (asset)
			{
				assets.Add(asset);
			}
		}

		return assets;
	}

	public static EnemyBase GetEnemy(EnemyType enemyType)
	{
		InitializeFactory();

		if (enemiesByType.ContainsKey(enemyType))
		{
			return enemiesByType[enemyType];
		}

		return null;
	}

	public static IEnumerable<EnemyType> GetAllEnemyTypes()
	{
		InitializeFactory();
		return enemiesByType.Keys;
	}

	public static IEnumerable<EnemyBase> GetAllEnemy()
	{
		InitializeFactory();
		return enemiesByType.Values;
	}
}
