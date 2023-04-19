using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    
    Rigidbody2D rb;
    private bool isStuck = false;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.right = rb.velocity;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
         
        if ((collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Enemy") && !isStuck)
        {
            rb.isKinematic = true;
            rb.gravityScale = 0;
            transform.SetParent(collision.gameObject.transform, true);
            isStuck = true;
        }
    }
}
