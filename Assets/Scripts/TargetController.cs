using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetController : MonoBehaviour
{
    private float waitTime = 1.0f;
    private float timer = 0.0f;
    private float wander = 1.1f;
    private NavMeshAgent target;
    private void OnEnable()
    {
        target = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > waitTime)
        {
            Vector3 newDestination = RandomSphere(transform.position, wander, -1);
            target.SetDestination(newDestination);
            timer -= waitTime;
        }
    }

    private Vector3 RandomSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit hit;
        NavMesh.SamplePosition(randDirection, out hit, dist, layermask);
        return hit.position;
    }
}

