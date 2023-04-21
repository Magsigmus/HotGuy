using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowPointer : MonoBehaviour
{
    public GameObject Arrow;
    float Charge = 0;

    public float Coldown;
    float ColdownTwo;

   
    public float MaxCharge;
    public float ExtraCharge;
    public float TimeToCharge;
    public float TimeInExtraCharge;
    public float EndCharge;
    float ChargeTime;

    public Sprite bow0;
    public Sprite bow1;
    public Sprite bow2;
    public Sprite bow3;
    public Sprite bow4;

    SpriteRenderer bow;

    // Start is called before the first frame update
    void Start()
    {
        bow = GameObject.Find("Bow").GetComponent<SpriteRenderer>();
    }
    //public float distToPlayer = 1;
    // Update is called once per frame

    void Update()
    {
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.parent.position;
        float angleZ = Mathf.Acos(dir.normalized.x) * Mathf.Sign(dir.y) * Mathf.Rad2Deg;
        gameObject.transform.localEulerAngles = new Vector3(0, 0, angleZ);

        ColdownTwo -= Time.deltaTime;
        if (ColdownTwo < 0)
        { 
            
            if (Input.GetMouseButton(0))
            {
                ChargeTime += Time.deltaTime;
                if (ChargeTime < TimeToCharge)
                {
                    Charge += Time.deltaTime * MaxCharge / TimeToCharge;
                }
                else if (ChargeTime < TimeToCharge + TimeInExtraCharge)
                {
                    Charge = MaxCharge + ExtraCharge;
                }
                else

                {
                    Charge = EndCharge;
                }
                if(Charge == EndCharge)
                {
                    bow.sprite = bow3;
                }
                else if (Charge < MaxCharge / 3 * 1)
                {
                    bow.sprite = bow1;
                }
                else if(Charge < MaxCharge / 3 * 2)
                {
                    bow.sprite = bow2;
                }
                else if(Charge < MaxCharge)
                {
                    bow.sprite = bow3;
                }
                else
                {
                    bow.sprite = bow4;
                }




            }
            else if (Input.GetMouseButtonUp(0))
            {
                GameObject newArrow = Instantiate(Arrow, transform.position, transform.rotation);
                newArrow.GetComponent<Rigidbody2D>().velocity = newArrow.transform.right * Charge;
                Charge = 0;
                ChargeTime = 0;
                bow.sprite = bow0;
                ColdownTwo = Coldown;
            }
        }

    }

}
