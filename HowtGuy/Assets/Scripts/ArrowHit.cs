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
            if (hit.collider.gameObject.tag == "Ground" || hit.collider.gameObject.tag == "Enemy")
            {
                rb.isKinematic = true;
                ArrowAnimator AAnimator = GetComponentInChildren<ArrowAnimator>();
                AAnimator.enabled = false;
                rb.velocity = new Vector3(0, 0, 0);
                rb.gravityScale = 0;
                transform.position = hit.point;
                transform.SetParent(hit.collider.gameObject.transform, true);

                //this is to disable the skript so that unnedded code wont run in the background
                enabled = false;
            }
        }
       

        
    }
 
}
