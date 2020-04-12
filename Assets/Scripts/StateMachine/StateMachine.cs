using UnityEngine;
using System.Collections;
using AI;
using System;

public abstract class StateMachine : MonoBehaviour
{
	protected State currentState;

	public void SetState(State state)
	{
		currentState = state;
		StartCoroutine(currentState.Start());
	}
}

public abstract class State
{
	protected GameManager gameManager;

	public State(GameManager gameManager)
	{
		this.gameManager = gameManager;
	}

	public abstract IEnumerator Start();

	public virtual IEnumerator SpawnEnemy()
	{
		yield break;
	}
}

public class GameStart : State
{
	public GameStart(GameManager gameManager) : base(gameManager)
	{
		GameManager.GameOver = false;
		PlayerHealth.Initialize();

		gameManager.CurrentLevel = 0;
		gameManager.CurrentWave = 0;
	}

	public override IEnumerator Start()
	{
		Debug.Log("Game Start!");

		yield return new WaitForSeconds(gameManager.CoroutineDelay);

		gameManager.SetState(new LevelStart(gameManager));
	}
}

public class LevelStart : State
{
	public static event Action<LevelStart> TileManager_Initialize;

	public LevelStart(GameManager gameManager) : base(gameManager)
	{
		gameManager.CurrentLevel++;
		gameManager.CurrentWave = 0;
	}

	public override IEnumerator Start()
	{
		Debug.Log("Level Start!");

		// Get map resource with currentLevel
		TextAsset textData = MapReader.GetTextAsset(gameManager.CurrentLevel);

		// if there isn't next map, victory. otherwise, Split map data
		if (textData == null)
		{
			gameManager.SetState(new Victory(gameManager));
			yield break;
		}
		else
		{ MapData.SplitTextData(textData); }

		// GameBoard.Initialize
		TileManager_Initialize?.Invoke(this);

		yield return new WaitForSeconds(gameManager.CoroutineDelay);

		gameManager.SetState(new WaveStart(gameManager));
		yield break;
	}
}

public class WaveStart : State
{
	public WaveStart(GameManager gameManager) : base(gameManager)
	{
		gameManager.CurrentWave++;
	}

	public override IEnumerator Start()
	{
		Debug.Log("Wave " + gameManager.CurrentWave + " Start!");

		yield return new WaitForSeconds(gameManager.CoroutineDelay);
		gameManager.enemyManager.StartSpawn(gameManager.CurrentWave);

		while (true)
		{
			if (EnemyManager.SpawnedEnemies == 0)
			{
				gameManager.SetState(new WaveClear(gameManager));
				yield break;
			}

			if (PlayerHealth.CurrentLife <= 0)
			{
				gameManager.SetState(new Lost(gameManager));
				yield break;
			}

			yield return new WaitForSeconds(gameManager.CoroutineDelay);
		}
	}
}

public class WaveClear : State
{
	public WaveClear(GameManager gameManager) : base(gameManager)
	{
	}

	public override IEnumerator Start()
	{
		Debug.Log("Wave Clear!");

		yield return new WaitForSeconds(gameManager.CoroutineDelay);

		// check if this was last wave. if not, go to next level
		if (gameManager.CurrentWave == MapData.LastWave())
		{
			gameManager.SetState(new LevelClear(gameManager));
			yield break;
		}
		else
		{
			gameManager.SetState(new WaveStart(gameManager));
			yield break;
		}
	}
}


public class LevelClear : State
{
	public LevelClear(GameManager gameManager) : base(gameManager)
	{
	}

	public override IEnumerator Start()
	{
		Debug.Log("Level Clear!");

		yield return new WaitForSeconds(gameManager.CoroutineDelay);

		gameManager.SetState(new LevelStart(gameManager));
		yield break;
	}
}

public class Victory : State
{
	public Victory(GameManager gameManager) : base(gameManager)
	{
		GameManager.GameOver = true;
	}

	public override IEnumerator Start()
	{
		Debug.Log("Victory!");
		while (true)
		{
			Debug.Log("Press P to Start the Game!");
			if (Input.GetKeyDown(KeyCode.P))
			{
				gameManager.SetState(new GameStart(gameManager));
				yield break;
			}

			yield return new WaitForSeconds(gameManager.CoroutineDelay);
		}
	}
}

public class Lost : State
{
	int countdown;
	public Lost(GameManager gameManager) : base(gameManager)
	{
		GameManager.GameOver = true;
		countdown = 6;
	}

	public override IEnumerator Start()
	{
		Debug.Log("Lost!");
		while (true)
		{
			countdown--;
			Debug.Log("Game Starts in... " + countdown);
			if (countdown == 0)
			{
				gameManager.SetState(new GameStart(gameManager));
				yield break;
			}

			yield return new WaitForSeconds(gameManager.CoroutineDelay);
		}
	}
}