using UnityEngine;
using System.Collections;
using System;

public abstract class ProjectileBase : MonoBehaviour, IResettable
{
	public event EventHandler ReturnToPool;

	protected TargetPoint target;

	protected TowerBase tower;

	public float ProjectileScaleX => transform.localScale.x;
	public float TargetScaleX => target.LocalScaleX;
	public Vector3 LaunchPoint => tower.TowerHead.position;

	public virtual void LaunchCannon(TowerBase tower, TargetPoint target, Vector3 launchVelocity, float explodeRadius)
	{ }

	public virtual void FireBullet(TowerBase tower, TargetPoint target, float projectileSpeed)
	{ }

	protected void OnReturnToPool()
	{
		ReturnToPool?.Invoke(this, null);
	}

	public virtual void Reset()
	{
		gameObject.SetActive(false);
	}
}
