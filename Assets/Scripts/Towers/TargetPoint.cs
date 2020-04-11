using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TargetPoint : MonoBehaviour
{
    public EnemyBase Enemy { get; private set; }
    public Vector3 Position => transform.position;
    public float ColliderRadius => GetComponent<SphereCollider>().radius;
    public float LocalScaleX => transform.localScale.x;

	const int enemyLayerMask = 1 << 9;
	static Collider[] targetsBuffer = new Collider[100];
	public static int BufferedCount { get; private set; }

	private void Awake()
    {
        Enemy = transform.root.GetComponent<EnemyBase>();
        Debug.Assert(Enemy != null, "Target point without Enemy root!", this);
        Debug.Assert(gameObject.layer == 9, "Target point on wrong layer!", this);
    }

	public static bool FillBuffer(Vector3 position, float range)
	{
		BufferedCount = Physics.OverlapSphereNonAlloc(
					position, range, targetsBuffer, enemyLayerMask);

		return BufferedCount > 0;
	}

	public static TargetPoint GetBuffered(int index)
	{
		var target = targetsBuffer[index].GetComponent<TargetPoint>();
		Debug.Assert(target != null, "Targeted non-enemy!", targetsBuffer[index]);
		return target;
	}


}
