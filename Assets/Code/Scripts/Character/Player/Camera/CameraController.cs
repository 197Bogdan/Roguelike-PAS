using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private Vector3 cameraOffset = new Vector3(15, -15, 15);


    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(playerTransform == null)
            return;
            
        transform.position = playerTransform.position + cameraOffset;
    }
}
