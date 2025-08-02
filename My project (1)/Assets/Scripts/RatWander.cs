using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RatWander : MonoBehaviour
{
    public float wanderRadius = 100f;       // How far from the start point the rat can wander
    public float wanderInterval = 2f;      // How often to choose a new point
    public float speed = 4.5f;             // NavMesh agent movement speed

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
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
        Vector3 newPos = RandomNavmeshLocation(wanderRadius);

        if (newPos != Vector3.zero)
        {
            agent.SetDestination(newPos);
        }
        else
        {
            agent.ResetPath();
        }
    }
     
     Vector3 RandomNavmeshLocation(float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection.y = 0;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

              return Vector3.zero;
    }
}