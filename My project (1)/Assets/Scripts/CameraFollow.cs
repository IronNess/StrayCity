using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Target to follow
    public Vector3 offset = new Vector3(0f, 1.5f, -3f); // Position behind player head

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 rotatedOffset = target.rotation * offset; // Collects the offset relative to the cats rotation
            transform.position = target.position + rotatedOffset; // Position Camera behind the cat
            transform.LookAt(target.position + Vector3.up * 1f); // Look at cat
        }
    }
}
