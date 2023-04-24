using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class PlatformerPlayerController : MonoBehaviour
{
    public float jumpGrav, normalGrav, jumpForce, runAcceleration, topSpeed, runDeacceleration,
        switchDeacceleration, coyotetime, holdingTime, runningJumpExpantion, cutOffVelocity;
    private float holdingTimer, coyotetimeTimer;
    public bool onGround, stoppedJumping = false;
    public Rigidbody2D rb2D;

    Animator playerAnimator;
    Transform spriteTrans;

    private void Start()
    {
        holdingTimer = holdingTime;
        //this is to find the animator
        playerAnimator = GameObject.Find("PlayerSprite").GetComponent<Animator>();
        spriteTrans = GameObject.Find("PlayerSprite").GetComponent<Transform>();
    }

    private void Update()
    {
        if (!onGround) { coyotetimeTimer += Time.deltaTime; }
        holdingTimer += Time.deltaTime;

        //tgus is to give the animator the speed of the player
        playerAnimator.SetFloat("Speed", rb2D.velocity.magnitude);

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            //This is for the animator
            playerAnimator.SetBool("Grounded", false);
            if (onGround || coyotetimeTimer < coyotetime)
            {
                Jump();
                coyotetimeTimer = coyotetime;
            }
            else
            {
                holdingTimer = 0;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Run();

        DeaccelerateInX();

        ApplyGravity();
    }

    void ApplyGravity()
    {
        float changeInYVelocity = 0;
        // Depending of if jump is pressed when the player jumps a different gravaty is applied
        if (Input.GetAxisRaw("Jump") == 1 && rb2D.velocity.y >= 0 && !stoppedJumping)
        { changeInYVelocity -= jumpGrav; }
        else
        { changeInYVelocity -= normalGrav; stoppedJumping = true; }

        rb2D.velocity += new Vector2(0, changeInYVelocity);
    }

    // Initiates a jump
    void Run()
    {
        float changeInXVelocity = 0;

        // Gets input from player
        float runDir = Input.GetAxisRaw("Horizontal");

        // Is the velocity under the max speed?
        if (runDir != 0 && rb2D.velocity.x * runDir < topSpeed) 
        {
            //this is to change the diraction the player is facing
            spriteTrans.localScale = new Vector3 (runDir * Mathf.Abs(spriteTrans.localScale.x), spriteTrans.localScale.y, 1);

            changeInXVelocity += runAcceleration * runDir;
            
            // Checks if the player is moving in a different direction than the user wants it to
            bool differnetDir = (runDir < 0) != (rb2D.velocity.x < 0);
            bool switchingDir = differnetDir && !(Mathf.Abs(rb2D.velocity.x) < 0.001);
            if (switchingDir) 
            {
                changeInXVelocity += switchDeacceleration * runDir;
            }
        }

        // Applies the change to the velocity vector
        rb2D.velocity += new Vector2(changeInXVelocity, 0);
    }

    void DeaccelerateInX()
    {
        // Is the player trying to move?
        if (Input.GetAxisRaw("Horizontal") == 0 && Mathf.Abs(rb2D.velocity.x) > 0)
        {
            // Deaccelerates:
            if (Mathf.Abs(rb2D.velocity.x) < runDeacceleration)
            {
                rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            }
            else
            {
                float xVal = -1 * Mathf.Sign(rb2D.velocity.x) * runDeacceleration;
                rb2D.velocity += new Vector2(xVal, 0);
            }
        }
    }

    // Initiates a jump
    void Jump()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce + Mathf.Abs(rb2D.velocity.x) * runningJumpExpantion);
        onGround = false;
        stoppedJumping = false;
        coyotetimeTimer = 2*coyotetime;
    }

    // Testing if the player is on the ground
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ground") 
        { 
            onGround = true;
            //This is for the animator
            playerAnimator.SetBool("Grounded", true);
            stoppedJumping = false;
            coyotetimeTimer = 0;
            if(holdingTimer < holdingTime) { Jump(); holdingTimer = holdingTime; }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground") 
        { 
            onGround = false;
            //This is for the animator
            playerAnimator.SetBool("Grounded", false);
        }
    }
}