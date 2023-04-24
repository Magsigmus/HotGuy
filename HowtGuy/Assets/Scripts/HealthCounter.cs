using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCounter : MonoBehaviour
{
    public int healthPoints = 4;

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
    }
}
