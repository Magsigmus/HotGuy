using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
         
        if (collision.gameObject.tag == "Ground")
        {
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
            transform.SetParent(collision.gameObject.transform, true);
        }
    }


}
