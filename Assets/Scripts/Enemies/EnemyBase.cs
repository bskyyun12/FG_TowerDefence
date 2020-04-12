using AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IResettable
{
	//public event EventHandler Death;

	//public PrefabFactory<EnemyBase> prefabFactory;

	[Header("Stat")]
	[SerializeField] EnemyType enemyType = default;
	[SerializeField] GameObject prefab = default;
	[SerializeField] float baseHealth = 100f;
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

	public bool IsDead => isDead.Value;
	public float CurrentLife => currentLife.Value;
	public float SpawnOffsetY => transform.localScale.y * 0.75f;
	public float BaseHealth => baseHealth;
	public GameObject Prefab => prefab;
	public EnemyType EnemyType => enemyType;
	public int Damage => damage;

	// Freeze
	bool isFreezing = false;
	float freezeTimer;

	// Anim
	Animator animator;

	// UniRx
	private IReactiveProperty<float> currentLife;
	private BoolReactiveProperty isDead = new BoolReactiveProperty();

	public int PoolIndex { get; set; }

	// Stream for dead enemies
	private static Subject<EnemyBase> onReturnToPoolStream = new Subject<EnemyBase>();	
	// public Observable stream
	public static IObservable<EnemyBase> OnReturnToPoolObservable 
											=> onReturnToPoolStream.AsObservable();
	

	private static Subject<EnemyBase> damageStream = new Subject<EnemyBase>();
	public static IObservable<EnemyBase> DamageObservable
											=> damageStream.AsObservable();


	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		// UniRx
		// reset life and dead flag
		currentLife = new FloatReactiveProperty(baseHealth);
		isDead.Value = false;

		// Move if not dead
		//this.UpdateAsObservable()
		//	.Where(_ => !isDead.Value)
		//	.Subscribe(x => Move());

		// dead condition
		this.UpdateAsObservable()
			.Where(_ => currentLife.Value <= 0f || GameManager.GameOver)
			.Subscribe(_ =>
			{
				isDead.Value = true;
			});
			   
		// if dead, start death coroutine
		isDead
			.Where(x => x)
			.Subscribe(x => StartCoroutine(OnDeath()));



		startTilePos = Astar.grid[MapData.startTile.x, MapData.startTile.y].worldPosition;
		endTilePos = Astar.grid[MapData.endTile.x, MapData.endTile.y].worldPosition;

		currentSpeed = baseSpeed;

		transform.position = new Vector3(startTilePos.x, SpawnOffsetY, startTilePos.z);

		animator.SetBool("Killed", false);
		animator.SetBool("isWalking", true);

		GoToGoal();
	}

	private void Update()
	{
		if (!IsDead)
		{
			Move();
		}
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
				//Player.LoseLife(Damage);
				damageStream.OnNext(this);
				isDead.Value = true;
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
		currentLife.Value -= damage;
	}

	private IEnumerator OnDeath()
	{
		animator.SetBool("Killed", true);

		yield return new WaitForSeconds(2f);

		// let this script flow in the PoolStream
		onReturnToPoolStream.OnNext(this);
	}


	public void Freeze(float MoveSpeedDecrese, float freezeDuration)
	{
		freezeTimer = 0f;
		if (!isFreezing)
		{
			StartCoroutine(OnFreeze(MoveSpeedDecrese, freezeDuration));
		}
	}

	private IEnumerator OnFreeze(float MoveSpeedDecrese, float freezeDuration)
	{
		isFreezing = true;
		currentSpeed *= (100f - MoveSpeedDecrese) * 0.01f;
		currentSpeed = Mathf.Clamp(currentSpeed, 0f, baseSpeed);

		while (true)
		{
			freezeTimer += Time.deltaTime;
			if (freezeTimer > freezeDuration)
			{
				isFreezing = false;
				currentSpeed = baseSpeed;
				yield break;
			}
			yield return null;
		}
	}
}