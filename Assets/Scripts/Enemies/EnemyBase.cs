using AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IResettable
{
	public event EventHandler Death;
	
	//public PrefabFactory<EnemyBase> prefabFactory;

	[Header("Stat")]
	[SerializeField] EnemyType enemyType = default;
	[SerializeField] GameObject prefab = default;
	[SerializeField] float baseHealth = 100f;
	[SerializeField] float currentHealth = 0f;
	[SerializeField] int damage = 1;

	[Header("Movement")]
	[SerializeField] float baseSpeed = 10.0f;
	[SerializeField] float currentSpeed;
	[SerializeField] float turnSpeed = 10.0f;
	[SerializeField] float accuracy = .5f;

	int pathIndex = 0;
	int pathLength;
	Vector3 goal;
	Vector3 startTilePos;
	Vector3 endTilePos;

	Coroutine deathCoroutine;

	public bool IsAlive => CurrentHealth > 0f;
	public float SpawnOffsetY => transform.localScale.y * 0.75f;
	public float BaseHealth => baseHealth;
	public float CurrentHealth => currentHealth;
	public GameObject Prefab => prefab;
	public EnemyType EnemyType => enemyType;


	// Freeze
	bool isFreezing;
	float freezeTimer;
	float freezeDuration;

	// Anim
	Animator animator;

	// UniRx
	public ReactiveProperty<float> CurrentLife;


	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		Observable.EveryUpdate()
			.Where(x => CurrentLife.Value <= 0 || GameManager.GameOver)
			.First()
			.Subscribe(x => StartCoroutine(OnDeath()));
	}

	private void OnEnable()
	{
		// UniRx
		CurrentLife = new FloatReactiveProperty(baseHealth);

		startTilePos = Astar.grid[MapData.startTile.x, MapData.startTile.y].worldPosition;
		endTilePos = Astar.grid[MapData.endTile.x, MapData.endTile.y].worldPosition;

		currentHealth = BaseHealth;
		currentSpeed = baseSpeed;

		transform.position = new Vector3(startTilePos.x, SpawnOffsetY, startTilePos.z);

		deathCoroutine = null;

		animator.SetBool("Killed", false);
		animator.SetBool("isWalking", true);

		GoToGoal();
	}

	private void GoToGoal()
	{
		IEnumerable<Node> pathList = Astar.FindPath(startTilePos, endTilePos);
		pathLength = pathList.Count();
		pathIndex = 0;
	}

	public void Reset()
	{
		gameObject.SetActive(false);
	}

	void LateUpdate()
	{
		//if (GameManager.GameOver)
		//{ StartCoroutine(OnDeath()); }

		if (IsAlive)
		{ Move(); }

		if (isFreezing)
		{
			freezeTimer += Time.deltaTime;
			if (freezeTimer > freezeDuration)
			{
				isFreezing = false;
				currentSpeed = baseSpeed;
			}
		}
	}

	private void Move()
	{
		// already in the goal || have finished traveling
		if (pathLength == 0 || pathIndex == pathLength)
		{
			return;
		}

		// if we are close enough to the current pathIndex
		if (Vector3.Distance(Astar.GetWorldPosition(pathIndex), transform.position) < accuracy + SpawnOffsetY)
		{
			// if the current pathIndex is not the last index
			if (pathIndex != pathLength - 1)
			{
				pathIndex++;
			}
			else
			{
				if (deathCoroutine == null)
				{
					PlayerHealth.LoseLife(damage);
					deathCoroutine = StartCoroutine(OnDeath());
				}
			}
		}

		// if we are not at the end of the path
		if (pathIndex < pathLength)
		{
			goal = Astar.GetWorldPosition(pathIndex);
			Vector3 lookAtGoal = new Vector3(goal.x,
											 transform.position.y,
											 goal.z);

			Vector3 direction = lookAtGoal - transform.position;
			Quaternion targetRotation = Quaternion.LookRotation(direction);

			// Look at the first path tile immediately
			if (pathIndex == 1)
			{ transform.rotation = targetRotation; }

			transform.rotation = Quaternion.Slerp(transform.rotation,
												  targetRotation,
												  Time.deltaTime * turnSpeed);

			transform.Translate(0, 0, currentSpeed * Time.deltaTime);
		}
	}


	public void TakeDamage(float damage)
	{
		Debug.Assert(damage >= 0f, "Damage value can't be negative!");
		currentHealth -= damage;

		if (!IsAlive)
		{
			//if (deathCoroutine == null)
			//{
			//	deathCoroutine = StartCoroutine(OnDeath());
			//}
		}
		else
		{
			// doesn't work properly.. idk why
			//animator.SetTrigger("Damaged");
		}
	}

	private IEnumerator OnDeath()
	{
		currentHealth = 0f;
		animator.SetBool("Killed", true);
		//animator.SetTrigger("Killed");
		yield return new WaitForSeconds(2f);
		Death?.Invoke(this, null);
	}


	public void Freeze(float MoveSpeedDecrese, float freezeDuration)
	{
		currentSpeed *= (1f - (MoveSpeedDecrese * .01f));
		currentSpeed = Mathf.Max(0f, currentSpeed);

		isFreezing = true;
		freezeTimer = 0f;
		this.freezeDuration = freezeDuration;
	}
}