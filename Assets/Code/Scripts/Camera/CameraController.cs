using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 lastPlayerPosition;


    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lastPlayerPosition = playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = playerTransform.position - lastPlayerPosition;
        transform.position += offset;
        lastPlayerPosition = playerTransform.position;
    }
}
