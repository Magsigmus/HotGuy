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

    [Header("Physics")]
    public float speed = 200f;
    public float nextWayPointDistance = 3f;
    public float nodeJumpRequirement = 0.8f;
    public float jumpForce = 0.8f;
    public float jumpCheckOffset = 0.8f;

    [Header("Custom Behaviour")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool dirLook = true;

    private Path currentPath;
    private int currentWayPoint = 0;
    private bool isGrounded = false;
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
        throw new NotImplementedException();
    }

    private void FollowPath()
    {
        if (currentPath == null) { return; }
        if (currentWayPoint >= currentPath.vectorPath.Count) { return; }


        direc
    }

    private bool TargetInDistance()
    {
        throw new NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground") { isGrounded = true; }
    }
}
