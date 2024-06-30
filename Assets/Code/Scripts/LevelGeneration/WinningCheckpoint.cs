using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningCheckpoint : MonoBehaviour
{
    private MenuManager menuManager;
    void Start()
    {
        menuManager = FindObjectOfType<MenuManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
        if(other.tag == "Player")
        {
            menuManager.WinGame();
        }
    }
}
