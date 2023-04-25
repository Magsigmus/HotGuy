using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using System.Xml.Linq;
using System.Linq;

public class KnightEnemyBehaviour : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform player;
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

    [Header("Combat")]
    public int maxHealthPoints = 5;
    public float attackCooldown = 0.3f;
    public float windUpTime = 0.5f;
    public Transform attackStartingPoint;
    public float attackRadius = 1.5f;
    public float attackAnimationTime = 0.2f;
    public int attackDamage = 1;
    public float dyingTime = 1f;

    [HideInInspector]
    public bool isDead = false;

    private int healthPoints;
    private bool canMove = true, attacking = false;
    private Path currentPath;
    private int currentWayPoint = 0;
    private bool isGrounded = false;
    private Seeker seeker;
    private Rigidbody2D rb2D;
    private GameObject GFXObject;
    private Animator animator;
    private float attackCoolDownTimer = 0;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb2D = GetComponent<Rigidbody2D>();
        GFXObject = gameObject.transform.GetChild(0).gameObject;
        animator = GFXObject.GetComponent<Animator>();

        healthPoints = maxHealthPoints;

        InvokeRepeating("UpdatePath", 0f, pathUpdateTime);
    }

    private void Update()
    {
        attackCoolDownTimer += Time.deltaTime;
        if (Vector2.Distance(player.position, transform.position) < playerStopDistance && 
            isGrounded && 
            rb2D.velocity.x == 0 &&
            !attacking &&
            attackCoolDownTimer >= attackCooldown &&
            !isDead)
        {
            StartCoroutine(Attack());
        }

        MakeAnimations();

        //Sig: Apllies gravity
        rb2D.velocity -= new Vector2(0, gravity * Time.deltaTime);

        if (TargetInDistance() && followEnabled && !isDead)
        {
            FollowPath();
        }
        else
        {
            float vx = Math.Sign(rb2D.velocity.x) * deacceleration * Time.deltaTime + rb2D.velocity.x;
            vx = Mathf.Clamp(vx, Math.Min(0, Math.Sign(vx) * maxSpeed), Math.Max(0, Math.Sign(vx) * maxSpeed));
            rb2D.velocity = new Vector2(vx, rb2D.velocity.y);
        }
    }


    private void MakeAnimations()
    {
        if (Math.Abs(rb2D.velocity.x) < 1f) { animator.SetBool("Running", false); }
        else { animator.SetBool("Running", true); }

        if (rb2D.velocity.y > 0.5f) 
        { 
            animator.SetBool("Jumping", true);
            animator.SetBool("Falling", false);
        }
        else if (rb2D.velocity.y < -0.5f)
        { 
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", true);
        }
        else
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
        }

        //Sig: Makes the sprite look in the appopriate direction
        if (directionLookEnabled)
        {
            if (rb2D.velocity.x < -1f)
            {
                Vector3 newLocalScale = GFXObject.transform.localScale;
                newLocalScale.x = -1 * Math.Abs(newLocalScale.x);
                GFXObject.transform.localScale = newLocalScale;
            }
            else if (rb2D.velocity.x > 1f)
            {
                Vector3 newLocalScale = GFXObject.transform.localScale;
                newLocalScale.x = Math.Abs(newLocalScale.x);
                GFXObject.transform.localScale = newLocalScale;
            }
        }
    }

    #region Pathfinding
    private void UpdatePath()
    {
        if (TargetInDistance() && followEnabled && seeker.IsDone())
        {
            seeker.StartPath(rb2D.position, player.position, OnPathComplete);
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
        if (currentPath == null || !canMove) { return; }
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
        if (hit.collider == null && Vector2.Distance(player.position, transform.position) >= playerStopDistance)
        {
            if (Math.Sign(rb2D.velocity.x) == Math.Sign(dir.x))
            {
                float vx = Mathf.Clamp(Math.Sign(dir.x) * acceleration * Time.deltaTime + rb2D.velocity.x, -maxSpeed, maxSpeed);
                rb2D.velocity = new Vector2(vx, rb2D.velocity.y);
                rb2D.velocity -= new Vector2(Math.Sign(rb2D.velocity.x) * deacceleration * Time.deltaTime, 0);
            }
            else
            {
                float vx = Mathf.Clamp(Math.Sign(dir.x) * deacceleration * Time.deltaTime + rb2D.velocity.x, -maxSpeed, maxSpeed);
                rb2D.velocity = new Vector2(vx, rb2D.velocity.y);
            }
        }
        else
        {
            if (rb2D.velocity.x < deacceleration)
            {
                rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            }
            else
            {
                float vx = -1 * Math.Sign(rb2D.velocity.x) * deacceleration * Time.deltaTime + rb2D.velocity.x;
                vx = Mathf.Clamp(vx, Math.Min(0, Math.Sign(vx) * maxSpeed), Math.Max(0, Math.Sign(vx) * maxSpeed));
                rb2D.velocity = new Vector2(vx, rb2D.velocity.y);
            }
        }

        //Sig: Makes the enemy jump
        if (jumpEnabled && isGrounded && height > minJumpHeight && maxJumpHeight > height && Vector2.Distance(player.position, transform.position) >= playerStopDistance)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpVelocity);
            isGrounded = false;
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
        return Vector2.Distance(transform.position, player.transform.position) < activationDistance;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Ground") { isGrounded = true; }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground") { isGrounded = false; }
    }
    #endregion

    #region Combat
    private IEnumerator Attack()
    {
        attackCoolDownTimer = 0;
        attacking = true;
        animator.SetBool("StartedAttacking", true);

        canMove = false;
        yield return new WaitForSeconds(windUpTime);

        animator.SetBool("StartedAttacking", false);
        animator.SetBool("FinishedAttacking", true);

        RaycastHit2D[] coll = Physics2D.CircleCastAll((Vector2)attackStartingPoint.position, attackRadius, new Vector2(0, 0), 0);
        if(coll.Select(e => e.transform.gameObject.tag).Contains("Player"))
        {
            player.gameObject.GetComponent<HealthCounter>().TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(attackAnimationTime);

        animator.SetBool("FinishedAttacking", false);
        canMove = true;
        attacking = false;
    }

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            StopAllCoroutines();
            canMove = false;
            gameObject.layer = LayerMask.NameToLayer("Decor");

            ArrowHit[] allArrows = gameObject.GetComponentsInChildren<ArrowHit>();
            foreach(ArrowHit arrow in allArrows) { arrow.StartMovement(); }
            animator.SetTrigger("Dying");
            isDead = true;
            rb2D.constraints = RigidbodyConstraints2D.FreezePositionX;
            Destroy(gameObject, dyingTime);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.DrawWireDisc(attackStartingPoint.position, Vector3.back, attackRadius);
    }
#endif
#endregion
}
