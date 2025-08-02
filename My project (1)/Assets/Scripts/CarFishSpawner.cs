using System.Collections;
using UnityEngine;

/// <summary>
/// Car with NO moving parts. Press E and a fish pops out — that's it.
/// </summary>
public class CarFishSpawner : PopContainerBase
{
     protected override IEnumerator OpenRoutine()  { yield break; }
    protected override IEnumerator CloseRoutine() { yield break; }
}