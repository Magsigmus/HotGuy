using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHit : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    RaycastHit2D hit;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float maxDistance = rb.velocity.magnitude * Time.fixedDeltaTime;
        hit = Physics2D.Raycast(rb.position, rb.velocity, maxDistance);
        
        if (hit)
        {
            Debug.Log("THERE IS A HIT");
            if (hit.collider.gameObject.tag == "Ground" || hit.collider.gameObject.tag == "Enemy")
            {
                rb.isKinematic = true;
                ArrowAnimator AAnimator = GetComponentInChildren<ArrowAnimator>();
                AAnimator.enabled = false;
                rb.velocity = new Vector3(0, 0, 0);
                rb.gravityScale = 0;
                transform.position = hit.point;
                transform.SetParent(hit.collider.gameObject.transform, true);
            }
        }
        /*
        if (Physics2D.Raycast(transform.position, rb.velocity, out hit))
        {
            // Get the point where the ray hit the object
            Debug.Log("THERE IS A HIT");
            if (hit.collider.gameObject.tag == "Ground" || hit.collider.gameObject.tag == "Enemy")
            {
                rb.isKinematic = true;
                rb.gravityScale = 0;
                transform.position = hit.point;
                transform.SetParent(hit.collider.gameObject.transform, true);
            }

        }*/

        
    }
 
}
