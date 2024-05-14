using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseState : MonoBehaviour
{
    public abstract void RunState();
    public abstract BaseState GetNextState();
    protected BaseState nextState;
    private float sightDistance = 50f;
    private float attackDistance = 2f;
    protected GameObject player;
    protected Animator animator;
    protected NavMeshAgent agent;

    public bool CanSeePlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        RaycastHit hit;
        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), player.transform.position + new Vector3(0, 1, 0) - (transform.position + new Vector3(0, 1, 0)), Color.red, 5f);
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), player.transform.position + new Vector3(0, 1, 0) - (transform.position + new Vector3(0, 1, 0)), out hit, sightDistance)) // add height offset, player base is in floor
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    public bool CanAttackPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), player.transform.position + new Vector3(0, 1, 0) - (transform.position + new Vector3(0, 1, 0)), out hit, attackDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

}
