using System.Collections;
using UnityEngine;

/// <summary>Cardboard box with four flaps that fold open and shut.</summary>
public class CardboardBoxContainer : PopContainerBase
{
    [Header("Flaps (0:X+, 1:Z-, 2:Z+, 3:X-)")]
    [SerializeField] private Transform[] flaps = new Transform[4];
    [SerializeField, Range(0, 180)]
    private float openAngle = 120f;
    [SerializeField, Min(0.1f)]
    private float speed = 5f;

    private Quaternion[] closedRot;

    private void Awake()
    {
        closedRot = new Quaternion[flaps.Length];
        for (int i = 0; i < flaps.Length; i++)
            closedRot[i] = flaps[i].localRotation;
    }

    protected override IEnumerator OpenRoutine()  => AnimateFlaps(true);
    protected override IEnumerator CloseRoutine() => AnimateFlaps(false);

    private IEnumerator AnimateFlaps(bool opening)
    {
        Quaternion[] start = opening ? closedRot       : GetOpenRot();
        Quaternion[] end   = opening ? GetOpenRot()    : closedRot;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            for (int i = 0; i < flaps.Length; i++)
                flaps[i].localRotation = Quaternion.Slerp(start[i], end[i], t);

            yield return null;
        }
    }

    private Quaternion[] GetOpenRot()
    {
        Quaternion[] open = new Quaternion[flaps.Length];
        for (int i = 0; i < flaps.Length; i++)
        {
            Vector3 axis = i switch
            {
                0 => new Vector3( openAngle, 0, 0),
                1 => new Vector3(0, 0, -openAngle),
                2 => new Vector3(0, 0,  openAngle),
                3 => new Vector3(-openAngle,0, 0),
                _ => Vector3.zero
            };
            open[i] = flaps[i].localRotation * Quaternion.Euler(axis);
        }
        return open;
    }
}
