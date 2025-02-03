using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderMove : IMoveBehavior
{
    private float wanderRadius = 10f;

    public void Move(UnityEngine.AI.NavMeshAgent agent, Transform target)
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += agent.transform.position;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out UnityEngine.AI.NavMeshHit hit, wanderRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }
}
