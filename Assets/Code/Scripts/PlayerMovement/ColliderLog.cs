using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.name == "Mage")
    {
        Debug.Log("Do something here");
    }

    if (collision.gameObject.tag == "MyGameObjectTag")
    {
        Debug.Log("Do something else here");
    }
}
}
