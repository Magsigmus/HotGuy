using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;

    public float dashDist, dashTime, dashCooldown = 0.1f;
    public float dashTimer;
    public bool dashing = false;
    private LayerMask mask;
    public string wallLayer = "Walls";

    public GameObject bullet;
    public float bulletSpeed, bulletLifeTime, shotCooldown;
    private float shotBulletTime;

    Vector2 move;
    private Vector2 lastMove = new Vector2(-1,0);

    //Fetch the Animator
    public Animator m_Animator;
    private SpriteRenderer _renderer;
    private static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");

    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        m_Animator = GetComponent<Animator>();
        if (m_Animator == null)
        {
            Debug.LogError("The player sprite is missing an Animator Component");
        }

        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("Player Sprite is missing a renderer");
        }

        rb = GetComponent<Rigidbody2D>();

        mask = LayerMask.GetMask(wallLayer);

        m_Animator.SetBool("moving", false);
    }

    private void Update()
    {
        shotBulletTime -= Time.deltaTime;

        if (currentHealth < 1)
        {
            Debug.Log("Player Dead");
            Application.Quit();
            // SWITCH SENCE
        } 

        dashTimer -= Time.deltaTime;

        if (!dashing)
        {
            move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //input shit
            move = move.normalized * speed; //speed is same no matter what direction dumbass

            lastMove = move;

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                _renderer.flipX = false;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                _renderer.flipX = true;
            }
            m_Animator.SetFloat(MovementSpeed, Mathf.Abs(Input.GetAxisRaw("Horizontal")));
        
        if (move.x != 0|| move.y != 0)
            {
                m_Animator.SetBool("moving", true);
               
            }
            else m_Animator.SetBool("moving", false);



            if (Input.GetAxisRaw("Fire1") == 1 && shotBulletTime <= 0)
            {
                shotBulletTime = shotCooldown;
                ShootProjecttile();
            }
        }
        
        if (Input.GetAxisRaw("Fire2") == 1 && !(lastMove.x == 0 && lastMove.y == 0) && dashTimer <= 0)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, lastMove, dashDist, mask);
        Vector2 dashToPoint = new Vector2();

        dashing = true;

        if (hit.collider == null)
        {
            dashToPoint = (Vector2)transform.position + lastMove.normalized * dashDist;
        }
        else
        {
            Debug.Log(-lastMove.normalized * 0.5f);
            
            dashToPoint = hit.point - lastMove.normalized * 0.5f;
            Vector3 temp = dashToPoint;
            Debug.DrawLine(transform.position, temp, Color.red, 1f);
        }
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        
        yield return new WaitForSeconds(dashTime);

        transform.position = dashToPoint;
        gameObject.GetComponent<Collider2D>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;

        dashing = false;
        dashTimer = dashCooldown;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dashing)
        {
            rb.velocity = move;
        }
        else
        {
            rb.velocity = new Vector2();
        }
    }
    public void ShootProjecttile()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = new Vector2(
            mousePos.x - transform.position.x,
            mousePos.y - transform.position.y
            ).normalized * 1.5f;

        GameObject newBullet = Instantiate(bullet);

        newBullet.transform.position = new Vector2(dir.x + transform.position.x, dir.y + transform.position.y);
        newBullet.transform.up = dir;

        newBullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * bulletSpeed;

        Destroy(newBullet, bulletLifeTime);
    }

    public void TakeDamage(int loss)
    {
        currentHealth -= loss;

        healthBar.SetHealth(currentHealth);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dashing = false;
        dashTimer = 0;
    }
}