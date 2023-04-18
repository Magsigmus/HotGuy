using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowPointer : MonoBehaviour
{
    public GameObject Arrow;
    public float Charge = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    //public float distToPlayer = 1;
    // Update is called once per frame
    
    void Update()
    {
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.parent.position;
      //  dir = dir.normalized * distToPlayer;
      //  gameObject.transform.localPosition = dir;
        float angleZ = Mathf.Acos(dir.normalized.x) * Mathf.Sign(dir.y) * Mathf.Rad2Deg;

      //  float angleY = 180 * Mathf.Floor(Mathf.Abs(angleZ / 90));
        gameObject.transform.localEulerAngles = new Vector3(0, 0, angleZ);
        //  gameObject.transform.GetChild(0).localEulerAngles = new Vector3(angleY, 0, 0);
        // this is for later if time  if (gameObject.transform.rotation.z > )
        
        if (Input.GetMouseButton(0))
        {
            Charge += Time.deltaTime;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Instantiate(Arrow, transform.position, transform.rotation);
            Charge = 0;
        }


    }
   
}
