using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseState : MonoBehaviour
{
    public abstract void RunState();
    public abstract BaseState GetNextState();

    private Vector3 heightOffset = new Vector3(0, 0.5f, 0);
    private LayerMask enemySightLayer;
    private string layerName1 = "Player";
    private string layerName2 = "Wall";

    protected BaseState nextState;
    protected float activeDistance = 50f;
    protected float hearingDistance = 15f;
    protected float sightDistance = 20f;
    protected float attackDistance = 1f;
    protected GameObject player;
    protected Animator animator;
    protected NavMeshAgent agent;

    public void InitValues()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemySightLayer = LayerMask.GetMask(layerName1, layerName2);

    }

    public bool IsInDistance(float distance)
    {
        return Vector3.Distance(transform.position, player.transform.position) < distance;
    }

    public bool IsLineOfSight(float range)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + heightOffset, player.transform.position - transform.position, out hit, range, enemySightLayer))
            if (hit.collider.CompareTag("Player"))
                return true;
        return false;
    }

}
