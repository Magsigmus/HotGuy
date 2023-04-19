using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    
    Rigidbody2D rb;
    
    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.right = rb.velocity;
        
        
    }
   /* void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        Gizmos.DrawRay(start.position, transform.position - start.position);
    }*/

    /*  private void OnTriggerStay2D(Collider2D collision)
      {

          if ((collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Enemy") && !isStuck)
          {
              rb.isKinematic = true;
              rb.gravityScale = 0;
              transform.SetParent(collision.gameObject.transform, true);
              isStuck = true;
          }
      }*/
}
