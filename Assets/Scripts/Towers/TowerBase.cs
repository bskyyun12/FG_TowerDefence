using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public abstract class TowerBase : MonoBehaviour
{
	[Header("General")]
	[SerializeField] private Transform towerHead = default;
	[SerializeField] private GameObject projectilePrefab = default;
	[SerializeField, Range(2f, 10f)] float range = 4f;
	[SerializeField, Range(.1f, 5f)] float fireRate = 2f;
	[SerializeField, Range(1f, 100f)] float damage = 50f;
	
	protected ObjectPool<ProjectileBase> pool;

	public float Damage => damage;
	public float Range => range;
	public Vector3 Position => transform.position;
	public Transform TowerHead => towerHead;
	public GameObject ProjectilePrefab => projectilePrefab;


	protected TargetPoint target = null;
	static Collider[] targetsBuffer = new Collider[1];
	const int enemyLayerMask = 1 << 9;

	Coroutine fireCoroutine;

	private void OnEnable()
	{
		target = null;
		fireCoroutine = null;

		//ProjectileBase.OnDamageTarget += OnDamageTarget;
	}
	private void OnDisable()
	{
		//ProjectileBase.OnDamageTarget -= OnDamageTarget;
	}

	public virtual void OnDamageTarget(TargetPoint target)
	{
	}
	public virtual void OnAreaDamage(Vector3 centerPosition)
	{
	}

	private void Start()
	{
		pool = new ObjectPool<ProjectileBase>(new PrefabFactory<ProjectileBase>(ProjectilePrefab));
	}

	protected virtual void Update()
	{
		Physics.SyncTransforms();
		if (GetTarget() != null)
		{
			if (fireCoroutine == null)
			{ fireCoroutine = StartCoroutine(Fire()); }

			TowerHead.LookAt(target.Position);
		}
	}

	private TargetPoint GetTarget()
	{
		if (!TargetPoint.FillBuffer(Position, range))
		{
			target = null;
			return target;
		}
		
		if (target && target.isActiveAndEnabled && target.Enemy.IsAlive)
		{
			float colliderRadius = target.ColliderRadius * target.LocalScaleX;
			if (Vector3.Distance(transform.localPosition, target.Position) > range + colliderRadius)
			{
				target = null;
			}
		}
		else
		{
			target = TargetPoint.GetBuffered(0);
		}

		return target;
	}

	private IEnumerator Fire()
	{
		while (true)
		{
			if (target != null && target.Enemy.IsAlive)
			{
				// shoot
				Shoot();
			}
			else
			{
				// stop shooting
				fireCoroutine = null;
				yield break;
				//if (fireCoroutine != null)
				//{
				//StopCoroutine(fireCoroutine);
				//fireCoroutine = null;

				//}
			}
			yield return new WaitForSeconds(fireRate);
		}
	}

	protected virtual void Shoot()
	{

	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(Position, range);

		if (target != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(Position, target.Position);
		}
	}
}
