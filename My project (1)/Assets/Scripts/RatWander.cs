using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Makes the rat enemy wander randomly within a radius using NavMeshAgent.
/// Inherits defeat behavior from Enemy and adds sound on defeat.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class RatWander : Enemy
{
    // ────────────────────────────────────────────────────────────────────────
    // Wandering Behavior Settings
    // ────────────────────────────────────────────────────────────────────────

    [Header("Wander")]
    [Tooltip("How far the rat can wander from its position")]
    [SerializeField, Min(1)] private float radius = 100f;

    [Tooltip("Time between selecting new destinations")]
    [SerializeField, Min(0.1f)] private float interval = 2f;

    [Tooltip("Speed of the rat when moving")]
    [SerializeField, Min(0.1f)] private float speed = 4.5f;

    // ────────────────────────────────────────────────────────────────────────
    // Internal State
    // ────────────────────────────────────────────────────────────────────────

    private NavMeshAgent agent;
    private float timer;

    // ────────────────────────────────────────────────────────────────────────
    // Unity Methods
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Called when the object is first enabled; initializes agent.
    /// </summary>
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    /// <summary>
    /// Called when the rat is activated in the scene.
    /// Immediately picks a destination to start moving.
    /// </summary>
    private void OnEnable() => PickDestination();

    /// <summary>
    /// Continuously checks if it's time to pick a new destination.
    /// </summary>
    private void Update()
    {
        timer += Time.deltaTime;

        // Pick a new destination if the rat arrived or timed out
        if (!agent.pathPending &&
            (agent.remainingDistance <= agent.stoppingDistance || timer >= interval))
        {
            PickDestination();
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    // Wandering Logic
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Chooses a new random point on the NavMesh to move toward.
    /// </summary>
    private void PickDestination()
    {
        timer = 0f;

        Vector3 pos = RandomNavLocation(radius);

        if (pos != Vector3.zero)
            agent.SetDestination(pos);
        else
            agent.ResetPath(); // fallback if NavMesh is invalid
    }

    /// <summary>
    /// Finds a valid position on the NavMesh within a certain radius.
    /// </summary>
    private static Vector3 RandomNavLocation(float r, int tries = 10)
    {
        for (int i = 0; i < tries; i++)
        {
            Vector3 random = Random.insideUnitSphere * r;
            random.y = 0;

            if (NavMesh.SamplePosition(random, out var hit, 10f, NavMesh.AllAreas))
                return hit.position;
        }

        return Vector3.zero; // failed to find valid location
    }

    // ────────────────────────────────────────────────────────────────────────
    // Enemy Defeat Override
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Plays sound and triggers GameManager logic when defeated.
    /// </summary>
    public override void Defeat()
    {
        AudioManager.Instance.PlayRatSplat(); // Play splat sound
        base.Defeat(); // Call base defeat (disables rat, updates state)
    }
}
