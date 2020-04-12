using System;
using System.Collections;
using UnityEngine;

public class BombBall : ProjectileBase
{
	Vector3 launchVelocity;
	float time;
	float explodeRadius;
	Vector3 targetPos;

	[SerializeField] Transform explosion = default;
	float explodeVisualDuration = .5f;
	Coroutine explodeCoroutine;

	private void Update()
	{
		if (target != null)
		{ Move(); }
	}

	public override void Reset()
	{
		base.Reset();
		time = 0f;
		explosion.localScale = Vector3.one;
	}

	public override void LaunchCannon(TowerBase tower, TargetPoint target, Vector3 launchVelocity, float explodeRadius)
	{
		base.LaunchCannon(tower, target, launchVelocity, explodeRadius);

		this.tower = tower;
		this.target = target;
		this.launchVelocity = launchVelocity;
		this.explodeRadius = explodeRadius;

		targetPos = target.Position;
		transform.position = LaunchPoint;

	}

	void Move()
	{
		time += Time.deltaTime;
		Vector3 d = launchVelocity;
		d.y -= 9.81f * time;
		transform.localRotation = Quaternion.LookRotation(d);

		Vector3 p = LaunchPoint + launchVelocity * time;
		p.y -= 0.5f * 9.81f * time * time;

		if (p.y > 0f)
		{
			transform.localPosition = p;
		}
		else
		{
			if (explodeCoroutine == null)
			{
				explodeCoroutine = StartCoroutine(Explode());
			}
			else
			{
				float colliderRadius = explosion.GetComponent<SphereCollider>().radius;
				Vector3 targetScale = Vector3.one * explodeRadius / colliderRadius / transform.localScale.x;
				explosion.localScale = Vector3.Lerp(explosion.localScale, targetScale, 5f * Time.deltaTime);
			}
		}
	}

	private IEnumerator Explode()
	{
		explosion.position = transform.position;

		tower.OnAreaDamage(targetPos);

		yield return new WaitForSeconds(explodeVisualDuration);
		OnReturnToPool();
		explodeCoroutine = null;
	}
}
