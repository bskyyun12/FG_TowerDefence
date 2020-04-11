using UnityEngine;
using System.Collections;
using System;
using Tools;

public class EnemyManager : MonoBehaviour
{
	IPool<EnemyBase>[] enemyPools;

	int currentWave;
	Array enemyEnumArray;
	int enemyIndex;

	[SerializeField, Range(.1f, 3f)]
	float timeBetweenSpawn = 2f;

	public int SpawnedEnemies { get; private set; }
	public int SpawnAmoutLeft { get; private set; }

	void Start()
	{
		enemyEnumArray = Enum.GetValues(typeof(EnemyType));
		enemyPools = new ObjectPool<EnemyBase>[enemyEnumArray.Length];

		for (int i = 0; i < enemyEnumArray.Length; i++)
		{
			EnemyBase enemy = EnemyMethods.GetEnemy((EnemyType)enemyEnumArray.GetValue(i));
			enemyPools[i] = new ObjectPool<EnemyBase>(new PrefabFactory<EnemyBase>(enemy.Prefab));
		}
	}

	public void StartSpawn(int currentWave)
	{
		this.currentWave = currentWave;

		// Get num of enemies of first type(Standard)
		SpawnAmoutLeft = MapData.GetNumberOfEnemy((int)enemyEnumArray.GetValue(0), currentWave);
		enemyIndex = 0;

		StartCoroutine(Spawn());
	}

	public IEnumerator Spawn()
	{
		while (true)
		{
			if (SpawnAmoutLeft == -1)
			{
				StopCoroutine(Spawn());
				yield break;
			}

			if (SpawnAmoutLeft == 0)
			{				
				enemyIndex++;
				
				if (enemyIndex < enemyEnumArray.Length)
				{
					SpawnAmoutLeft = MapData.GetNumberOfEnemy((int)enemyEnumArray.GetValue(enemyIndex), currentWave);
				}
				else
				{
					Debug.Log("No more Enemies left in this wave.");
					//HasNextLevel = false;
					StopCoroutine(Spawn());
					yield break;
				}
			}

			if (SpawnAmoutLeft > 0)
			{
				//SpawnEnemy((EnemyType)values.GetValue(valuesIterator));
				SpawnEnemy(enemyIndex);
				SpawnAmoutLeft--;
			}


			yield return new WaitForSeconds(timeBetweenSpawn);
		}
	}

	public void SpawnEnemy(int enemyIndex)
	{
		EnemyBase enemy = enemyPools[enemyIndex].Get();
		enemy.gameObject.SetActive(true);
		SpawnedEnemies++;

		EventHandler handler = null;
		handler = (sender, e) =>
		{
			enemyPools[enemyIndex].ReturnToPool(enemy);
			SpawnedEnemies--;
			enemy.Death -= handler;
		};
		enemy.Death += handler;
	}
}
