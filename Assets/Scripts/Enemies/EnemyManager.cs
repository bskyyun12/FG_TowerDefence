using UnityEngine;
using System.Collections;
using System;
using Tools;
using UniRx;

public class EnemyManager : MonoBehaviour
{
	public static IPool<EnemyBase>[] enemyPools;

	int currentWave;
	Array enemyEnumArray;
	int enemyIndex;

	[SerializeField, Range(.1f, 3f)]
	float timeBetweenSpawn = 2f;

	public static int SpawnedEnemies { get; set; }
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

		// get the stream and subscribe
		EnemyBase.OnReturnToPoolObservable
			.Subscribe(x => OnReturnToPool(x));
	}

	void OnReturnToPool(EnemyBase enemy)
	{
		enemyPools[enemy.PoolIndex].ReturnToPool(enemy);
		SpawnedEnemies--;
	}

	public void StartSpawn(int currentWave)
	{
		this.currentWave = currentWave;

		// Get num of enemies of first type(Standard)
		SpawnAmoutLeft = MapData.GetNumberOfEnemy((int)enemyEnumArray.GetValue(0), currentWave);
		SpawnedEnemies = 0;
		enemyIndex = 0;

		StartCoroutine(Spawn());
	}

	public IEnumerator Spawn()
	{
		while (true)
		{
			if (SpawnAmoutLeft == -1 || GameManager.GameOver)
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
					StopCoroutine(Spawn());
					yield break;
				}
			}

			if (SpawnAmoutLeft > 0)
			{
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
		enemy.PoolIndex = enemyIndex;
		SpawnedEnemies++;
	}
}
