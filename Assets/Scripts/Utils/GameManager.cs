using AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
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

	private void Awake()
	{
		this.UpdateAsObservable()
			.Where(x => Input.GetKeyDown(KeyCode.UpArrow))
			.Where(x => GameSpeed < MaxGameSpeed)
			.Subscribe(_ => Time.timeScale = ++GameSpeed);

		this.UpdateAsObservable()
			.Where(x => Input.GetKeyDown(KeyCode.DownArrow))
			.Where(x => GameSpeed != 0f)
			.Subscribe(_ => Time.timeScale = --GameSpeed);
	}

	private void Start()
	{
		SetState(new GameStart(this));
	}
}
