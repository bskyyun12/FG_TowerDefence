using System;
using System.Collections.Generic;
using UnityEngine;

public static class MapData
{
	public static Vector2Int startTile;
	public static Vector2Int endTile;

	private static string[] tileData;
	private static string[] levelData;

	// Tuple< Wave, Enemy Enum Value, Num of Enemy >
	private static List<Tuple<int, int, int>> waveDataList;

	public static void SplitTextData(TextAsset textData)
	{
		string[] Data = textData.text.Split('#');

		// split with \n and remove empty index
		tileData = Data[0].Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

		// split with \n and remove empty index
		levelData = Data[1].Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		
		waveDataList = new List<Tuple<int, int, int>>();
		var values = Enum.GetValues(typeof(EnemyType));
		int numOfWaves = levelData.Length;

		for (int y = 0; y < numOfWaves; y++)
		{
			string[] enemyNumberData = levelData[y].Split(' ');

			for (int x = 0; x < values.Length; x++)
			{
				waveDataList.Add(new Tuple<int, int, int>(y + 1, (int)values.GetValue(x), int.Parse(enemyNumberData[x])));
			}
		}
	}

	public static int LastWave()
	{
		return levelData.Length;
	}

	public static Vector2Int GetMapSize()
	{
		return new Vector2Int(tileData[0].ToCharArray().Length, tileData.Length);
	}

	public static string GetTileRowData(int index)
	{
		return tileData[index];
	}

	public static int GetNumberOfEnemy(int enemyEnumValue, int currentWaveLevel)
	{
		foreach (var item in waveDataList)
		{
			if (item.Item1 == currentWaveLevel && item.Item2 == enemyEnumValue)
			{
				return item.Item3;
			}
		}

		return -1;
	}
}
