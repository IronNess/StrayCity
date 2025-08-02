using System.Collections;
using UnityEngine;

/// <summary>
/// Abstract base class for any object that can pop out a collectible
/// when the player (cat) interacts with it (e.g., box, trashcan, car).
/// Subclasses define how to visually open/close.
/// </summary>
public abstract class PopContainerBase : MonoBehaviour, IInteractable
{
    // ────────────────────────────────────────────────────────────────────────
    // Collectible Settings
    // ────────────────────────────────────────────────────────────────────────

    [Header("Collectible")]
    [Tooltip("The prefab that will pop out when opened (e.g., fish)")]
    [SerializeField] private GameObject collectiblePrefab;

    [Tooltip("Position and orientation where the collectible appears")]
    [SerializeField] private Transform popPosition;

    [Tooltip("Upward force applied to the popped object")]
    [SerializeField, Min(0.1f)] private float popForce = 4f;

    // ────────────────────────────────────────────────────────────────────────
    // Timing Settings
    // ────────────────────────────────────────────────────────────────────────

    [Header("Timing")]
    [Tooltip("Delay before the container closes again")]
    [SerializeField, Min(0.1f)] private float closeDelay = 2f;

    // ────────────────────────────────────────────────────────────────────────
    // State
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// True if the container has already been used
    /// </summary>
    public bool HasPopped { get; private set; }

    // ────────────────────────────────────────────────────────────────────────
    // Interaction Logic
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Triggered when the cat interacts with this object.
    /// Starts the pop animation if it hasn't been triggered yet.
    /// </summary>
    /// <param name="cat">The cat interacting with the object</param>
    public void Interact(CatMovement cat)
    {
        if (!HasPopped)
            StartCoroutine(PopRoutine());
    }

    /// <summary>
    /// Full animation sequence: open → spawn item → close
    /// </summary>
    private IEnumerator PopRoutine()
    {
        HasPopped = true;

        // Play open animation (custom per subclass)
        yield return StartCoroutine(OpenRoutine());

        // Spawn and launch collectible
        if (collectiblePrefab && popPosition)
        {
            GameObject item = Instantiate(collectiblePrefab, popPosition.position, popPosition.rotation);
            item.tag = "Collectible";

            // Launch it upward with force if it has a Rigidbody
            if (item.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity = Vector3.zero;
                rb.AddForce(popPosition.up * popForce, ForceMode.Impulse);
            }
        }

        // Wait before closing
        yield return new WaitForSeconds(closeDelay);

        // Play close animation (custom per subclass)
        yield return StartCoroutine(CloseRoutine());
    }

    // ────────────────────────────────────────────────────────────────────────
    // Visual Animation Hooks
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Must be implemented by subclasses to define opening animation.
    /// </summary>
    protected abstract IEnumerator OpenRoutine();

    /// <summary>
    /// Must be implemented by subclasses to define closing animation.
    /// </summary>
    protected abstract IEnumerator CloseRoutine();
}



