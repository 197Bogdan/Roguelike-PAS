using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] private BaseState currentState;

    void Start()
    {
        currentState = GetComponent<IdleState>();
    }

    void Update()
    {
        currentState.RunState();
        currentState = currentState.GetNextState();
    }
}
