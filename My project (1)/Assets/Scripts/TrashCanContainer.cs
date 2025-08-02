using System.Collections;
using UnityEngine;

/// <summary>
/// A container with a single swinging lid that pops a fish when opened,
/// then automatically closes after a short delay.
/// </summary>
public class TrashCanContainer : PopContainerBase
{
    // ────────────────────────────────────────────────────────────────────────
    // Lid Animation Settings
    // ────────────────────────────────────────────────────────────────────────

    [Header("Lid")]
    [Tooltip("The lid Transform that swings open")]
    [SerializeField] private Transform lid;

    [Tooltip("Angle to open the lid in degrees (negative Z)")]
    [SerializeField, Range(-180f, 0f)] private float openAngle = -120f;

    [Tooltip("How fast the lid opens and closes")]
    [SerializeField, Min(0.1f)] private float speed = 5f;

    // ────────────────────────────────────────────────────────────────────────
    // Internal State
    // ────────────────────────────────────────────────────────────────────────

    private Quaternion closedRot; // Original (closed) lid rotation

    private void Awake()
    {
        // Store the initial local rotation of the lid to return to later
        closedRot = lid ? lid.localRotation : Quaternion.identity;
    }

    // ────────────────────────────────────────────────────────────────────────
    // Overrides from PopContainerBase
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Opens the lid using rotation animation.
    /// </summary>
    protected override IEnumerator OpenRoutine()
    {
        if (!lid) yield break;

        Quaternion openRot = closedRot * Quaternion.Euler(0, 0, openAngle);
        yield return RotateLid(lid, closedRot, openRot);
    }

    /// <summary>
    /// Closes the lid by rotating it back to its original position.
    /// </summary>
    protected override IEnumerator CloseRoutine()
    {
        if (!lid) yield break;

        Quaternion openRot = closedRot * Quaternion.Euler(0, 0, openAngle);
        yield return RotateLid(lid, openRot, closedRot);
    }

    // ────────────────────────────────────────────────────────────────────────
    // Lid Rotation Utility
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Smoothly rotates a transform from one rotation to another.
    /// </summary>
    /// <param name="t">The transform to rotate</param>
    /// <param name="from">Starting rotation</param>
    /// <param name="to">Target rotation</param>
    private IEnumerator RotateLid(Transform t, Quaternion from, Quaternion to)
    {
        float tLerp = 0f;

        while (tLerp < 1f)
        {
            tLerp += Time.deltaTime * speed;
            t.localRotation = Quaternion.Slerp(from, to, tLerp);
            yield return null;
        }
    }
}
