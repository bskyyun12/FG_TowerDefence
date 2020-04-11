using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthSlider : MonoBehaviour
{
    [SerializeField] EnemyBase enemy = default;
    [SerializeField] Slider slider = default;

    float CurrentHealth => enemy.CurrentHealth;
    float MaxHealth => enemy.BaseHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = CalculateHealth();
    }

    float CalculateHealth()
    {
        return CurrentHealth / MaxHealth;
    }
}
