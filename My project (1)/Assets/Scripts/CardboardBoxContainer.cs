using System.Collections;
using UnityEngine;

/// <summary>
/// A pop container with four flaps (like a cardboard box) that open outward to reveal a collectible,
/// then close again after a short delay. Flaps animate individually based on axis.
/// </summary>
public class CardboardBoxContainer : PopContainerBase
{
    // ────────────────────────────────────────────────────────────────────────
    // Flap Settings
    // ────────────────────────────────────────────────────────────────────────

    [Header("Flaps (0:X+, 1:Z-, 2:Z+, 3:X-)")]
    [Tooltip("The four flap transforms of the cardboard box")]
    [SerializeField] private Transform[] flaps = new Transform[4];

    [Tooltip("Angle (in degrees) each flap opens")]
    [SerializeField, Range(0, 180)] private float openAngle = 120f;

    [Tooltip("Speed at which flaps open and close")]
    [SerializeField, Min(0.1f)] private float speed = 5f;

    // ────────────────────────────────────────────────────────────────────────
    // Internal State
    // ────────────────────────────────────────────────────────────────────────

    private Quaternion[] closedRot; // Stores original (closed) rotations for each flap

    private void Awake()
    {
        // Cache initial rotations for all four flaps
        closedRot = new Quaternion[flaps.Length];
        for (int i = 0; i < flaps.Length; i++)
            closedRot[i] = flaps[i].localRotation;
    }

    // ────────────────────────────────────────────────────────────────────────
    // Overrides from PopContainerBase
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>Plays the flap open animation.</summary>
    protected override IEnumerator OpenRoutine() => AnimateFlaps(true);

    /// <summary>Plays the flap close animation.</summary>
    protected override IEnumerator CloseRoutine() => AnimateFlaps(false);

    // ────────────────────────────────────────────────────────────────────────
    // Animation Logic
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Animates all flaps opening or closing.
    /// </summary>
    /// <param name="opening">True to open, false to close</param>
    private IEnumerator AnimateFlaps(bool opening)
    {
        Quaternion[] startRotations = opening ? closedRot : GetOpenRotations();
        Quaternion[] endRotations   = opening ? GetOpenRotations() : closedRot;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            for (int i = 0; i < flaps.Length; i++)
                flaps[i].localRotation = Quaternion.Slerp(startRotations[i], endRotations[i], t);
            yield return null;
        }
    }

    /// <summary>
    /// Calculates open rotations for all flaps based on index and openAngle.
    /// </summary>
    private Quaternion[] GetOpenRotations()
    {
        Quaternion[] openRotations = new Quaternion[flaps.Length];

        for (int i = 0; i < flaps.Length; i++)
        {
            Vector3 rotationAxis = i switch
            {
                0 => new Vector3( openAngle, 0, 0),  // Flap 0 → X+
                1 => new Vector3(0, 0, -openAngle),  // Flap 1 → Z-
                2 => new Vector3(0, 0,  openAngle),  // Flap 2 → Z+
                3 => new Vector3(-openAngle, 0, 0),  // Flap 3 → X-
                _ => Vector3.zero
            };

            openRotations[i] = flaps[i].localRotation * Quaternion.Euler(rotationAxis);
        }

        return openRotations;
    }
}
