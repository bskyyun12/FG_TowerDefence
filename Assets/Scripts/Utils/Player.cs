using UniRx;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static BoolReactiveProperty IsDead { get; set; } = new BoolReactiveProperty();
	public BoolReactiveProperty IsDamaged { get; set; } = new BoolReactiveProperty();
	
	private void Start()
	{
		IsDead.AsObservable()
			.Where(x => x);
	}

	private void OnEnable()
	{
		IsDead.Value = false;
	}
}