using UnityEngine;

/// <summary>Base enemy class. Cat calls Defeat() when attacking.</summary>
public abstract class Enemy : MonoBehaviour
{
    public virtual void Defeat()
    {
        GameManager.Instance.CatchRat();
        Destroy(gameObject);
    }
}
