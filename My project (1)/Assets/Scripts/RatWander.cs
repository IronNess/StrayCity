using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RatWander : MonoBehaviour
{
    public float wanderRadius = 10f;       // How far from the start point the rat can wander
    public float wanderInterval = 2f;      // How often to choose a new point
    public float speed = 3.5f;             // NavMesh agent movement speed

    private NavMeshAgent agent;
    private Vector3 homePosition;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        homePosition = transform.position;
        PickNewDestination();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!agent.pathPending &&
            (agent.remainingDistance <= agent.stoppingDistance || timer >= wanderInterval))
        {
            PickNewDestination();
        }
    }

    void PickNewDestination()
    {
        timer = 0f;

        for (int i = 0; i < 10; i++) // Try up to 10 times to find a valid point
        {
            Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
            randomDir.y = 0f;
            Vector3 potentialPos = homePosition + randomDir;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(potentialPos, out hit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return; // Found a valid point, exit early
            }
        }

        // If no point was valid after 10 tries, reset path
        agent.ResetPath();
    }
}
