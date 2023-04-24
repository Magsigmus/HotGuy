using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthCounter : MonoBehaviour
{
    public int healthPoints = 4;
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    private void Start()
    {

    }

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
    }

}
