using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	[SerializeField] Slider healthSlider = default;
	[SerializeField] float baseHealth = 20f;

	public ReactiveProperty<float> CurrentHealth { get; private set; } = new FloatReactiveProperty();

	Player player;


	private static Subject<PlayerHealth> deathStream = new Subject<PlayerHealth>();
	public static IObservable<PlayerHealth> DeathObservable
											=> deathStream.AsObservable();

	
	private void Awake()
	{
		player = GetComponent<Player>();
	}

	private void Start()
	{
		this.UpdateAsObservable()
			.Where(x => CurrentHealth.Value > 0f)
			.Subscribe(_ =>
			{
				healthSlider.value = CalculateHealth();
			});

		this.UpdateAsObservable()
			.Where(x => CurrentHealth.Value <= 0f)			
			.Subscribe(_ =>
			{
				Player.IsDead.Value = true;
			});

		EnemyBase.DamageObservable
			.Subscribe(x =>
			{
				TakeDamage(x.Damage);
			});
	}

	private void OnEnable()
	{
		CurrentHealth.Value = baseHealth;
	}

	float CalculateHealth()
	{
		return CurrentHealth.Value / baseHealth;
	}

	public void TakeDamage(int amount)
	{
		if (CurrentHealth.Value >= 0)
		{
			CurrentHealth.Value -= amount;

			if (!player.IsDamaged.Value)
			{ StartCoroutine(DamageCoroutine()); }
		}
		else
		{
			deathStream.OnNext(this);
		}
	}

	private IEnumerator DamageCoroutine()
	{
		player.IsDamaged.Value = true;
		yield return new WaitForSeconds(.5f);
		player.IsDamaged.Value = false;
	}
}

