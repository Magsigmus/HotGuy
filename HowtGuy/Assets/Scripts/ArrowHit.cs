using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHit : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    RaycastHit2D hit;
    public int damage = 1;
    private bool disabledMovment = false;
    private ArrowAnimator aAnimator;
    private float orignalGrav;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aAnimator = GetComponentInChildren<ArrowAnimator>();
        orignalGrav = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (disabledMovment) { return; }

        float maxDistance = rb.velocity.magnitude * Time.fixedDeltaTime;
        hit = Physics2D.Raycast(rb.position, rb.velocity, maxDistance);
        
        if (hit)
        {
            if (hit.collider.gameObject.tag == "Ground")
            {
                StopMovement();
            }

            if(hit.collider.gameObject.tag == "Enemy" && !hit.collider.gameObject.GetComponent<KnightEnemyBehaviour>().isDead)
            {

                StopMovement();
                hit.collider.gameObject.GetComponent<KnightEnemyBehaviour>().TakeDamage(damage);
            }
        }
        
    }

    private void StopMovement()
    {
        rb.isKinematic = true;
        aAnimator.enabled = false;
        rb.velocity = new Vector3(0, 0, 0);
        rb.gravityScale = 0;
        transform.position = hit.point;
        transform.SetParent(hit.collider.gameObject.transform, true);

        disabledMovment = true;
    }

    public void StartMovement()
    {
        rb.isKinematic = false;
        aAnimator.enabled = true;
        rb.gravityScale = orignalGrav;
        gameObject.transform.parent = null;
        disabledMovment = false;
    }
}
