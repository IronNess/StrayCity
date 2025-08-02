using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for containers that pop a collectible when the cat presses E.
/// Sub-classes provide their own open / close visuals.
/// </summary>
public abstract class PopContainerBase : MonoBehaviour, IInteractable
{
    [Header("Collectible")]
    [SerializeField]  private GameObject collectiblePrefab;
    [SerializeField]  private Transform  popPosition;
    [SerializeField, Min(0.1f)]
    private float popForce = 4f;

    [Header("Timing")]
    [SerializeField, Min(0.1f)]
    private float closeDelay = 2f;

    public bool HasPopped { get; private set; }

    public void Interact(CatMovement cat)
    {
        if (!HasPopped)
            StartCoroutine(PopRoutine());
    }

    private IEnumerator PopRoutine()
    {
        HasPopped = true;
        
        yield return StartCoroutine(OpenRoutine());

        if (collectiblePrefab && popPosition)
        {
            GameObject item = Instantiate(collectiblePrefab,
                                          popPosition.position,
                                          popPosition.rotation);
            item.tag = "Collectible";

            if (item.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity = Vector3.zero;
                rb.AddForce(popPosition.up * popForce, ForceMode.Impulse);
            }
        }

         yield return new WaitForSeconds(closeDelay);
        yield return StartCoroutine(CloseRoutine());
    }

    protected abstract IEnumerator OpenRoutine();
    protected abstract IEnumerator CloseRoutine();
}






