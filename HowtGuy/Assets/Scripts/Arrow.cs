using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    public float charge;
    public int Modifyer;
    public Rigidbody2D rb;
    public GameObject Shulder;
    public BowPointer BP;

    void Start()
    {
        Shulder = GameObject.Find("ShulderPoint");
        BP = Shulder.GetComponent<BowPointer>();

        charge = BP.Charge;
        Debug.Log("Charge level: " + charge);
        rb = GetComponent<Rigidbody2D>();

        //rb.velocity = new Vector2(Mathf.Cos(transform.rotation.z) * charge * Modifyer, Mathf.Sign(transform.rotation.z) * charge * Modifyer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
