using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHit : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    public Vector3 start;
    RaycastHit hit;
    Vector3 hitPoint;
    Vector3 end;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        start = rb.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        end = rb.position;
        if (Physics.Raycast(start, end - start, out hit))
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

        }

        start = rb.position;
    }
}
