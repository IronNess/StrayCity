using UnityEngine;

/// <summary>Any object the cat can interact with.</summary>
public interface IInteractable
{
    void Interact(CatMovement cat);
}