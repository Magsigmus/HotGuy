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

    // Start is called before the first frame update
    void Start()
    {

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
            }
            else if (Input.GetMouseButtonUp(0))
            {
                GameObject newArrow = Instantiate(Arrow, transform.position, transform.rotation);
                newArrow.GetComponent<Rigidbody2D>().velocity = newArrow.transform.right * Charge;
                Charge = 0;
                ChargeTime = 0;
                ColdownTwo = Coldown;
            }
        }

    }

}
