using UnityEngine;
using TowerDefense;

public static class MapReader
{
	public static TextAsset GetTextAsset(int currentLevel)
	{
		string fileName = "map_" + currentLevel.ToString();
		TextAsset textData = Resources.Load(ProjectPaths.RESOURCES_MAP_SETTINGS + fileName) as TextAsset;

		if (textData == null)
		{
			Debug.Log("There is no file named: " + fileName);
		}

		return textData;
	}
}