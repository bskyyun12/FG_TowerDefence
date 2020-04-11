using AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : StateMachine
{
	public EnemyManager enemyManager;

	public int CurrentLevel { get; set; } = 0;
	public int CurrentWave { get; set; } = 0;
	public float CoroutineDelay { get; set; } = 1f;
	public float GameSpeed { get; set; } = 1f;
	public float MaxGameSpeed { get; private set; } = 20f;

	public static bool GameOver { get; set; } = false;

	private void Start()
	{
		SetState(new GameStart(this));


	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			if (GameSpeed < MaxGameSpeed)
			{
				GameSpeed++;
			}
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			if (GameSpeed != 0f)
			{
				GameSpeed--;
			}
		}
		Time.timeScale = GameSpeed;
	}
}
