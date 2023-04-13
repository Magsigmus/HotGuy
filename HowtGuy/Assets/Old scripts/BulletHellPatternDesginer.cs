using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletPatteren
{
    public int shotsInBurst, bulletSpeed;
    public GameObject bulletPefab, specialBulletFormation;
    public bool facePlayerOnParentLevel;
    public bool facePlayerOnBulletLevel;
    [Range(-180,180)] public float startDegree;
    public float degreesBetweenShots, randomDeviation, timeBetweenShots, timeToNextBurst;

    public BulletPatteren() { }
}

public class BulletHellPatternDesginer : MonoBehaviour
{
    public bool activateOnce = false;

    public float bulletLifeTime = 5f;

    public GameObject player, spawner;

    public BulletPatteren[] bulletPatterens;
    public BulletPatteren[] ondeathBulletPatterens;

    private GameObject parent;
    GameObject newBullet;

    Coroutine bulletCorotine;

    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        StartBulletPattern(bulletPatterens);
    }

    public void StopBulletPattern()
    {
        if (bulletCorotine != null)
        {
            StopCoroutine(bulletCorotine);
        }
        bulletCorotine = null;
    }

    public void StartBulletPattern(BulletPatteren[] inBulletPatterens)
    {
        StopBulletPattern();
        bulletCorotine = StartCoroutine(executeBulletPatterns(inBulletPatterens));
    }

    public void TriggerDeath()
    {
        StopBulletPattern();
        StartCoroutine(Death(ondeathBulletPatterens));
    }

    IEnumerator Death(BulletPatteren[] inBulletPatterens)
    {
        StopBulletPattern();
        activateOnce = true;
        yield return StartCoroutine(executeBulletPatterns(inBulletPatterens));
        Destroy(spawner);
        Destroy(this);
    }

    IEnumerator executeBulletPatterns(BulletPatteren[] currentBulletPatterns)
    {
        while (true)
        {
            // Goes through all the bulletpatterns
            for (int i = 0; i < currentBulletPatterns.Length; i++)
            {
                BulletPatteren currentBulletPatteren = currentBulletPatterns[i];

                if (currentBulletPatteren.specialBulletFormation != null)
                {
                    parent = Instantiate(currentBulletPatteren.specialBulletFormation);
                    parent.transform.position = spawner.transform.position;
                }

                // goes through all the shots in the Burst
                for (int j = 0; j < currentBulletPatteren.shotsInBurst; j++)
                {
                    if (currentBulletPatteren.specialBulletFormation != null)
                    {
                        // Finds the bullet
                        newBullet = parent.transform.GetChild(j).gameObject;
                    }

                    Vector2 dirTowardsPlayer = new Vector2();

                    float rotation = currentBulletPatteren.degreesBetweenShots * j +
                        currentBulletPatteren.startDegree + Random.Range(currentBulletPatteren.randomDeviation, -currentBulletPatteren.randomDeviation);

                    if (currentBulletPatteren.facePlayerOnParentLevel)
                    {
                        dirTowardsPlayer = AddVec2(player.transform.position, -spawner.transform.position).normalized;
                    }
                    else if (currentBulletPatteren.facePlayerOnBulletLevel)
                    {
                        dirTowardsPlayer = AddVec2(player.transform.position, -newBullet.transform.position).normalized;
                    }

                    rotation += Mathf.Sign(dirTowardsPlayer.y) * Mathf.Acos(dirTowardsPlayer.x) * Mathf.Rad2Deg;

                    rotation /= Mathf.Rad2Deg;

                    Vector2 bulletDirection;
                    // Turns that rotation into a vector
                    bulletDirection = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));

                    if(currentBulletPatteren.specialBulletFormation == null)
                    {
                        // Makes the bullet
                        newBullet = Instantiate(currentBulletPatteren.bulletPefab);
                        newBullet.transform.position = AddVec2(spawner.transform.position, bulletDirection);
                        newBullet.transform.up = bulletDirection;
                    }

                    // Gives the bullet an velocity 
                    newBullet.GetComponent<Rigidbody2D>().velocity = bulletDirection * currentBulletPatteren.bulletSpeed;

                    // Destoryes the bullet after its lifespan
                    Destroy(newBullet, bulletLifeTime);

                    // Destorys the specialized bullet pattern
                    if(currentBulletPatteren.specialBulletFormation != null)
                    {
                        Destroy(parent, bulletLifeTime * 2);
                    }

                    // Waits between shots
                    if (currentBulletPatteren.timeBetweenShots != 0)
                    {
                        yield return new WaitForSeconds(currentBulletPatteren.timeBetweenShots);
                    }
                }

                // Waits between fazes
                if(currentBulletPatteren.timeToNextBurst != 0)
                {
                    yield return new WaitForSeconds(currentBulletPatteren.timeToNextBurst);
                }
            }

            if (activateOnce) { break; }
        }
    }




    Vector2 AddVec2(Vector2 a, Vector2 b)
    {
        return new Vector2(a.x + b.x, a.y + b.y);
    }
}