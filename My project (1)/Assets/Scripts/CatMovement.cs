using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CatMovement : MonoBehaviour
{
    // ─── Movement ────────────────────────────────────────────────────────────
    [Header("Movement")]
    [SerializeField, Range(1, 100)]  private float moveSpeed     = 40f;
    [SerializeField, Range(30, 360)] private float rotationSpeed = 200f;
    public float MoveSpeed     => moveSpeed;
    public float RotationSpeed => rotationSpeed;

    // ─── Input ───────────────────────────────────────────────────────────────
    [Header("Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode attackKey   = KeyCode.F;

    // ─── Components ──────────────────────────────────────────────────────────
    private CharacterController controller;
    private Animator            animator;

    // ─── State ───────────────────────────────────────────────────────────────
    private IInteractable currentInteractable;
    private Enemy         currentEnemy;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator   = GetComponent<Animator>();
        if (animator) animator.applyRootMotion = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
        HandleAttack();
    }

    private void HandleMovement()
    {
        Vector3 dir = (transform.right * Input.GetAxisRaw("Horizontal") +
                       transform.forward* Input.GetAxisRaw("Vertical")).normalized;

        controller.Move(dir * moveSpeed * Time.deltaTime);

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                          look,
                                                          rotationSpeed * Time.deltaTime);
        }
        animator?.SetBool("isWalking", dir.sqrMagnitude > 0.001f);
    }

    private void HandleInteraction()
    {
        if (currentInteractable == null) return;
        if (Input.GetKeyDown(interactKey))
        {
            animator?.SetTrigger("CollectItem");
            currentInteractable.Interact(this);
        }
    }

    private void HandleAttack()
    {
        if (currentEnemy == null) return;
        if (Input.GetKeyDown(attackKey))
        {
            animator?.SetTrigger("CollectItem");
            currentEnemy.Defeat();
            currentEnemy = null;
        }
    }

    // ─── Trigger detection ───────────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable i)) currentInteractable = i;
        if (other.TryGetComponent(out Enemy         e)) currentEnemy       = e;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable i) && ReferenceEquals(i, currentInteractable))
            currentInteractable = null;
        if (other.TryGetComponent(out Enemy e) && ReferenceEquals(e, currentEnemy))
            currentEnemy = null;
    }

    // Optional death hook
    private void OnDeath() => GameManager.Instance.GameOver();
}
