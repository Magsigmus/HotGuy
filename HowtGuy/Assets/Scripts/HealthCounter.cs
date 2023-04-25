using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthCounter : MonoBehaviour
{
    public int healthPoints = 4;
    public Slider slider;

    private void Start()
    {
        SetMaxHealth(healthPoints);
    }

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
        StartCoroutine(SetHealth(healthPoints));
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

    }

    public IEnumerator SetHealth(int health)
    {
        float startHealth = slider.value;
        for(int i = 0; i < 30; i++)
        {
            slider.value = Mathf.Lerp(startHealth, health, ((float)i)/30f);
            yield return new WaitForEndOfFrame();
        }
        slider.value = health;
    }
}
