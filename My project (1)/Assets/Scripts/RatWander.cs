using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RatWander : Enemy
{
    [Header("Wander")]
    [SerializeField, Min(1)] private float radius   = 100f;
    [SerializeField, Min(0.1f)] private float interval = 2f;
    [SerializeField, Min(0.1f)] private float speed  = 4.5f;

    private NavMeshAgent agent;
    private float timer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    private void OnEnable() => PickDestination();

    private void Update()
    {
        timer += Time.deltaTime;
        if (!agent.pathPending &&
            (agent.remainingDistance <= agent.stoppingDistance || timer >= interval))
            PickDestination();
    }

    private void PickDestination()
    {
        timer = 0;
        Vector3 pos = RandomNavLocation(radius);
        if (pos != Vector3.zero) agent.SetDestination(pos);
        else agent.ResetPath();
    }

    private static Vector3 RandomNavLocation(float r, int tries = 10)
    {
        for (int i = 0; i < tries; i++)
        {
            Vector3 random = Random.insideUnitSphere * r;
            random.y = 0;
            if (NavMesh.SamplePosition(random, out var hit, 10f, NavMesh.AllAreas))
                return hit.position;
        }
        return Vector3.zero;
    }
}
