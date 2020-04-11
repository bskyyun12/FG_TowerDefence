using System;
using UnityEngine;

public class BombTower : TowerBase
{
	[Header("Bomb Tower")]
	[SerializeField, Range(.1f, 5f)] float explodeRadius = 2f;
	
	float launchSpeed;
	Vector3 launchVelocity;

	void Awake()
	{
		OnValidate();
	}

	void OnValidate()
	{
		float x = Range + 1.001f;
		float y = -TowerHead.position.y;
		launchSpeed = Mathf.Sqrt(9.81f * (y + Mathf.Sqrt(x * x + y * y)));
	}

	public override void OnAreaDamage(Vector3 centerPosition)
	{
		base.OnAreaDamage(centerPosition);

		// here implement aoe damage
		TargetPoint.FillBuffer(centerPosition, explodeRadius);
		for (int i = 0; i < TargetPoint.BufferedCount; i++)
		{
			TargetPoint.GetBuffered(i).Enemy.TakeDamage(Damage);
		}
	}

	protected override void Shoot()
	{
		base.Shoot();

		Launch(target);

		ProjectileBase projectile = pool.Get();
		projectile.gameObject.SetActive(true);
		projectile.LaunchCannon(this, target, launchVelocity, explodeRadius);

		EventHandler handler = null;
		handler = (sender, e) =>
		{
			pool.ReturnToPool(projectile);
			projectile.Death -= handler;
		};
		projectile.Death += handler;

	}

	private void Launch(TargetPoint target)
	{
		Vector3 launchPoint = TowerHead.position;
		Vector3 targetPoint = target.Position;
		targetPoint.y = 0f;
		
		Vector2 dir;
		dir.x = targetPoint.x - launchPoint.x;
		dir.y = targetPoint.z - launchPoint.z;
		float x = dir.magnitude;
		float y = -launchPoint.y;
		dir /= x;

		Debug.DrawLine(launchPoint, targetPoint, Color.yellow, 1f);
		Debug.DrawLine(
			new Vector3(launchPoint.x, 0f, launchPoint.z),
			new Vector3(launchPoint.x + dir.x * x, 0f, launchPoint.z + dir.y * x),
			Color.white,
			1f
		);
		
		float g = 9.81f;
		float s = launchSpeed;
		float s2 = s * s;

		#region Calculations
		// 1)
		// dx = vx * t;
		// dy = vy * t - gt^2 / 2

		// 2)
		// s = hypotenuse = launchSpeed
		// vx = adjacent = s * cos(theta)
		// vy = opposite = s * sin(theta)

		// Combine 1) and 2)
		// dx = s * cos(theta) * t
		// dy = s * sin(theta) * t - gt^2 / 2

		// Therefore..
		// t = dx / s * cos(theta)
		// dy = s * sin(theta) * (dx / s * cos(theta)) - g(dx / s * cos(theta))^2 / 2
		// ...
		// dy = dx * sin(theta) / cos(theta) - g * dx^2 / 2 * s^2 * cos^2(theta)
		// dy = dx * tan(theta) - g * dx^2 / 2 * s^2 * cos^2(theta)
		// ...
		// tan(theta) = (s^2 + sqrt(s^4 -g(gx^2 + 2ys^2))) / gx

		// rootContent needs to be at least 0. higher number, higher height, less range
		// I don't need to shoot further than the tower's range
		// At max range, r = 0
		// rootContetn = s2 * s2 - g * (g * x * x + 2f * y * s2) = 0
		// ...
		// s2 = g(y+sqrt(x2 + y2))
		// s = sqrt(s2)
		#endregion Calculations

		float rootContent = s2 * s2 - g * (g * x * x + 2f * y * s2);
		Debug.Assert(rootContent >= 0f, "Launch velocity insufficient for range!" + rootContent);

		float tanTheta = (s2 + Mathf.Sqrt(rootContent)) / (g * x);
		float theta = Mathf.Atan(tanTheta);
		float cosTheta = Mathf.Cos(theta);
		float sinTheta = Mathf.Sin(theta);

		launchVelocity = new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y);

		Vector3 prev = launchPoint, next;
		for (int i = 1; i <= 10; i++)
		{
			float t = i / 10f;
			float dx = s * cosTheta * t;
			float dy = s * sinTheta * t - .5f * g * t * t;
			next = launchPoint + new Vector3(dir.x * dx, dy, dir.y * dx);
			Debug.DrawLine(prev, next, Color.blue, 1f);
			prev = next;
		}
	}
}
