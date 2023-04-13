using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage;
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Wall") { Destroy(gameObject); }
        if (coll.gameObject.tag == "Enemy")
        {
            coll.gameObject.GetComponent<EnemyBehaviour>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
