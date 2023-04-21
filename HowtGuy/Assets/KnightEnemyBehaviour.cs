using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class KnightEnemyBehaviour : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float activationDistance = 50f;
    public float pathUpdateTime = 0.5f;
    public float nextWayPointDistance = 3f;
    public float obstecaleStopDistance = 0.3f;
    public float playerStopDistance = 2;

    [Header("Movement")]
    public float acceleration = 0.01f;
    public float deacceleration = 0.03f;
    public float maxSpeed = 200f;
    public float jumpVelocity = 1f;
    public float minJumpHeight = 0.8f;
    public float maxJumpHeight = 1f;
    public float gravity = 0.1f;

    [Header("Custom Behaviour")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    private Path currentPath;
    private int currentWayPoint = 0;
    public bool isGrounded = false;
    private Seeker seeker;
    private Rigidbody2D rb2D;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb2D = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateTime);
    }

    private void FixedUpdate()
    {
        //Sig: Apllies gravity
        rb2D.velocity -= new Vector2(0, gravity * Time.fixedDeltaTime);

        if (TargetInDistance() && followEnabled) 
        {
            FollowPath();
        }
    }

    private void UpdatePath()
    {
        if (TargetInDistance() && followEnabled && seeker.IsDone())
        {
            seeker.StartPath(rb2D.position, target.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        if (p.error) { return; }

        currentPath = p;
        currentWayPoint = 0;
    }

    private void FollowPath()
    {

        if (currentPath == null) { return; }
        if (currentWayPoint >= currentPath.vectorPath.Count) { return; }

        while(currentWayPoint < currentPath.vectorPath.Count &&
            (Vector2)currentPath.vectorPath[currentWayPoint] == rb2D.position){
            currentWayPoint++;
        }

        Vector2 dir = (Vector2)currentPath.vectorPath[currentWayPoint] - rb2D.position;
        float height = dir.y;
        for(int i = currentWayPoint + 1; i < currentPath.vectorPath.Count; i++)
        {
            if (currentPath.vectorPath[i].x == 0)
            {
                height += currentPath.vectorPath[i].y - currentPath.vectorPath[i - 1].y;
            }
            else { break; }
        }

        // Sig: Is there anything that is in the direct path?
        LayerMask rayMask = LayerMask.GetMask("Solids");
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, new Vector2(dir.x, 0).normalized, obstecaleStopDistance, rayMask);
        Debug.DrawRay((Vector2)transform.position, new Vector2(dir.x, 0).normalized * obstecaleStopDistance, Color.red);

        // Sig: Apply force to the velocity in the x-axis
        if (hit.collider != null || Vector2.Distance(target.position, transform.position) < playerStopDistance)
        {
            rb2D.velocity -= new Vector2(Math.Sign(rb2D.velocity.x) * deacceleration, 0);
        }
        else if (!isGrounded || Math.Sign(rb2D.velocity.x) != Math.Sign(dir.x))
        {
            float vx = Mathf.Clamp(Math.Sign(dir.x) * deacceleration + rb2D.velocity.x, -maxSpeed, maxSpeed);
            rb2D.velocity = new Vector2(vx, rb2D.velocity.y);

        }
        else
        {
            float vx = Mathf.Clamp(Math.Sign(dir.x) * acceleration + rb2D.velocity.x, -maxSpeed, maxSpeed);
            rb2D.velocity = new Vector2(vx, rb2D.velocity.y);
        }

        //Sig: Makes the enemy jump
        if (jumpEnabled && isGrounded && height > minJumpHeight && maxJumpHeight > height)
        {
            Debug.Log("Triggered");
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpVelocity);
            isGrounded = false;
        }

        //Sig: Makes the sprite look in the appopriate direction
        if (directionLookEnabled)
        {
            if (rb2D.velocity.x > 0.05f)
            {
                Vector3 newLocalScale = transform.localScale;
                newLocalScale.x = -1 * Math.Abs(transform.localScale.x);
                transform.localScale = newLocalScale;
            }
            else if (rb2D.velocity.x > 0.05f)
            {
                Vector3 newLocalScale = transform.localScale;
                newLocalScale.x = Math.Abs(transform.localScale.x);
                transform.localScale = newLocalScale;
            }
        }

        //Sig: Do we need to go to the next waypoint?
        float dist = Vector2.Distance(rb2D.position, currentPath.vectorPath[currentWayPoint]);
        if(dist < nextWayPointDistance)
        {
            currentWayPoint++;
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activationDistance;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Ground") { isGrounded = true; }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground") { isGrounded = false; }
    }
}
