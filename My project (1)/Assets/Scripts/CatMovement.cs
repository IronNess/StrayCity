using UnityEngine;

/// <summary>
/// Handles cat movement, interactions (e.g. collecting items), and attacking enemies.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class CatMovement : MonoBehaviour
{
    // ────────────────────────────────────────────────────────────────────────
    // Movement Settings
    // ────────────────────────────────────────────────────────────────────────

    [Header("Movement")]
    [Tooltip("How fast the cat moves")]
    [SerializeField, Range(1, 100)] private float moveSpeed = 40f;

    [Tooltip("How quickly the cat turns to face the direction of movement")]
    [SerializeField, Range(30, 360)] private float rotationSpeed = 200f;

    /// <summary>Read-only access for other systems if needed.</summary>
    public float MoveSpeed     => moveSpeed;
    public float RotationSpeed => rotationSpeed;

    // ────────────────────────────────────────────────────────────────────────
    // Input Settings
    // ────────────────────────────────────────────────────────────────────────

    [Header("Input")]
    [Tooltip("Key used to interact with objects")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Tooltip("Key used to attack enemies")]
    [SerializeField] private KeyCode attackKey = KeyCode.F;

    // ────────────────────────────────────────────────────────────────────────
    // Components
    // ────────────────────────────────────────────────────────────────────────

    private CharacterController controller;
    private Animator animator;

    // ────────────────────────────────────────────────────────────────────────
    // Runtime State
    // ────────────────────────────────────────────────────────────────────────

    private IInteractable currentInteractable;
    private Enemy currentEnemy;

    // ────────────────────────────────────────────────────────────────────────
    // Unity Lifecycle
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Initialize components on object creation.
    /// </summary>
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator   = GetComponent<Animator>();

        if (animator)
            animator.applyRootMotion = false; // We’re controlling movement manually
    }

    /// <summary>
    /// Handles movement, interaction, and combat every frame.
    /// </summary>
    private void Update()
    {
        HandleMovement();
        HandleInteraction();
        HandleAttack();
    }

    // ────────────────────────────────────────────────────────────────────────
    // Movement and Input Handling
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Handles player-controlled movement and rotation.
    /// </summary>
    private void HandleMovement()
    {
        Vector3 direction = (transform.right * Input.GetAxisRaw("Horizontal") +
                             transform.forward * Input.GetAxisRaw("Vertical")).normalized;

        controller.Move(direction * moveSpeed * Time.deltaTime);

        // Rotate the cat to face movement direction
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, rotationSpeed * Time.deltaTime);
        }

        animator?.SetBool("isWalking", direction.sqrMagnitude > 0.001f);
    }

    /// <summary>
    /// Handles interaction with items (e.g., pressing E near a fish).
    /// </summary>
    private void HandleInteraction()
    {
        if (currentInteractable == null) return;

        if (Input.GetKeyDown(interactKey))
        {
            animator?.SetTrigger("CollectItem");
            currentInteractable.Interact(this);
            currentInteractable = null; // prevent multiple triggers
        }
    }

    /// <summary>
    /// Handles attacking enemies (e.g., pressing F near the rat).
    /// </summary>
    private void HandleAttack()
    {
        if (currentEnemy == null) return;

        if (Input.GetKeyDown(attackKey))
        {
            animator?.SetTrigger("CollectItem"); // reuse collect animation
            currentEnemy.Defeat(); // custom logic in enemy class
            currentEnemy = null;
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    // Trigger Detection (for interaction + combat)
    // ────────────────────────────────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
            currentInteractable = interactable;

        if (other.TryGetComponent(out Enemy enemy))
            currentEnemy = enemy;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable) &&
            ReferenceEquals(interactable, currentInteractable))
        {
            currentInteractable = null;
        }

        if (other.TryGetComponent(out Enemy enemy) &&
            ReferenceEquals(enemy, currentEnemy))
        {
            currentEnemy = null;
        }
    }


    /// <summary>
    /// Called on death (if triggered externally).
    /// </summary>
    private void OnDeath()
    {
        GameManager.Instance.GameOver();
    }
}
