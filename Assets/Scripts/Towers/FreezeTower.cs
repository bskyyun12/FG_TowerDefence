using System;
using UnityEngine;

public class FreezeTower : TowerBase
{
	[Header("Freeze Tower")]
	[SerializeField, Range(5f, 50f)] float projectileSpeed = 30f;
	[SerializeField, Range(.1f, 100f)] float freezeChance = 100f;
	[SerializeField, Range(10f, 100f)] float moveSpeedDecrese = 15f;
	[SerializeField, Range(.1f, 10f)] float freezeDuration = 2f;

	protected override void Shoot(TargetPoint target)
	{
		base.Shoot(target);

		ProjectileBase projectile = pool.Get();
		projectile.gameObject.SetActive(true);
		projectile.FireBullet(this, target, projectileSpeed);

		EventHandler handler = null;
		handler = (sender, e) =>
		{
			pool.ReturnToPool(projectile);
			projectile.ReturnToPool -= handler;
		};
		projectile.ReturnToPool += handler;
	}

	public override void OnDamageTarget(TargetPoint target)
	{
		base.OnDamageTarget(target);
		
		if (!target.Enemy.IsDead)
		{
			target.Enemy.TakeDamage(Damage);

			if (UnityEngine.Random.Range(0, 100) < freezeChance)
			{
				target.Enemy.Freeze(moveSpeedDecrese, freezeDuration);
			}
		}
	}
}
