using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	[SerializeField] Slider slider = default;

	static float MaxLife => 20;
	public static float CurrentLife { get; private set; }

	public static void Initialize()
	{
		CurrentLife = MaxLife;
	}

	// Update is called once per frame
	void Update()
	{
		slider.value = CalculateHealth();
	}

	float CalculateHealth()
	{
		return CurrentLife / MaxLife;
	}

	public static void LoseLife(int amount)
	{
		if (CurrentLife >= 0)
		{ CurrentLife -= amount; }
	}
}
