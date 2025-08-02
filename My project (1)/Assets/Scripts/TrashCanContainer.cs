using System.Collections;
using UnityEngine;

/// <summary>A bin with one lid that swings up, pops a fish, then closes.</summary>
public class TrashCanContainer : PopContainerBase
{
    [Header("Lid")]
    [SerializeField] private Transform lid;
    [SerializeField, Range(-180f, 0f)]
    private float openAngle = -120f;
    [SerializeField, Min(0.1f)]
    private float speed = 5f;

    private Quaternion closedRot;

    private void Awake() => closedRot = lid ? lid.localRotation : Quaternion.identity;

    protected override IEnumerator OpenRoutine()
    {
        if (!lid) yield break;
        Quaternion openRot = closedRot * Quaternion.Euler(0, 0, openAngle);
        yield return RotateLid(lid, closedRot, openRot);
    }

    protected override IEnumerator CloseRoutine()
    {
        if (!lid) yield break;
        Quaternion openRot = closedRot * Quaternion.Euler(0, 0, openAngle);
        yield return RotateLid(lid, openRot, closedRot);
    }

    private IEnumerator RotateLid(Transform t, Quaternion from, Quaternion to)
    {
        float tLerp = 0;
        while (tLerp < 1)
        {
            tLerp += Time.deltaTime * speed;
            t.localRotation = Quaternion.Slerp(from, to, tLerp);
            yield return null;
        }
    }
}
