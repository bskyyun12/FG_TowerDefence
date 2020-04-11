using System.Collections;
using UnityEngine;

public class FreezeBall : ProjectileBase
{
	float speed;

	[SerializeField] Transform freezeVisual = default;
	float freezeVisualDuration = .5f;
	float freezeVisualSize = 1f;
	Coroutine freezeCoroutine;

	public override void Reset()
	{
		base.Reset();
		freezeVisual.localScale = Vector3.one;
	}

	public override void FireBullet(TowerBase tower, TargetPoint target, float projectileSpeed)
	{
		base.FireBullet(tower, target, projectileSpeed);

		this.tower = tower;
		this.target = target;
		speed = projectileSpeed;

		transform.position = LaunchPoint;
	}

	protected override void Update()
	{
		base.Update();

		if (target)
		{
			float dist = Vector3.Distance(transform.position, target.Position);
			if (dist > (ProjectileScaleX + TargetScaleX) * .5f)
			{
				transform.position = Vector3.Lerp(transform.position,
												  target.Position,
												  speed * Time.deltaTime
												  );
			}
			else
			{
				if (freezeCoroutine == null)
				{
					freezeCoroutine = StartCoroutine(Explode());
				}
			}
		}
	}

	private IEnumerator Explode()
	{
		freezeVisual.position = new Vector3(transform.position.x, 0f, transform.position.z);
		float colliderRadius = freezeVisual.GetComponent<SphereCollider>().radius;
		freezeVisual.localScale = Vector3.one * freezeVisualSize / colliderRadius / transform.localScale.x;

		tower.OnDamageTarget(target);

		yield return new WaitForSeconds(freezeVisualDuration);
		OnDeath();
		freezeCoroutine = null;
	}
}
