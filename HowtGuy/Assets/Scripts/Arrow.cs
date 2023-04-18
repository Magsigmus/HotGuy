using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    float charge;
    public GameObject Shulder;
    void Start()
    {
        BowPointer otherScript = Shulder.GetComponent<BowPointer>();

        charge = otherScript.Charge;
        Debug.Log("Charge level: " + charge);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
