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

    [Header("Physics")]
    public float acceleration = 0.01f;
    public float deacceleration = 0.03f;
    public float maxSpeed = 200f;
    public float nodeJumpRequirement = 0.8f;
    public float jumpVelocity = 1f;
    public float jumpCheckOffset = 0.8f;

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
        
        if(Math.Sign(rb2D.velocity.x) == Math.Sign(dir.x))
        {
            rb2D.velocity += new Vector2(Math.Sign(dir.x) * acceleration, rb2D.velocity.y);
        }
        else
        {
            rb2D.velocity += new Vector2(Math.Sign(dir.x) * deacceleration, rb2D.velocity.y);
        }

        if (jumpEnabled && isGrounded && dir.y > nodeJumpRequirement)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpVelocity);
            isGrounded = false;
        }

        float dist = Vector2.Distance(rb2D.position, currentPath.vectorPath[currentWayPoint]);
        if(dist < nextWayPointDistance)
        {
            currentWayPoint++;
        }

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
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activationDistance;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Ground") { isGrounded = true; }
    }
}
