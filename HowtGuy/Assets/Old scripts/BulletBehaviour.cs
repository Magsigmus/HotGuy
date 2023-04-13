using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public int damage;
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Wall") { Destroy(gameObject); }
        if (coll.gameObject.tag == "Player")
        {
            coll.gameObject.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
