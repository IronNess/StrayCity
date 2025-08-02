using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleFish : MonoBehaviour, IInteractable
{
    // Called by CatMovement when the player presses E
    public void Interact(CatMovement cat)
    {
        GameManager.Instance.CollectFish();   // update HUD & game logic
        Destroy(gameObject);                  // remove the fish
    }
}