using System.Collections;
using UnityEngine;

public class TrashCanTrigger : MonoBehaviour
{
    public GameObject fish2;
    public Transform lid;
    public float lidOpenAngle = -120f;
    public float lidOpenSpeed = 5f;
    public float lidCloseDelay = 2f;

    private bool hasPopped = false;
    private bool playerNear = false;
    private Quaternion originalLidRotation;

    void Start()
    {
        if (lid != null)
            originalLidRotation = lid.localRotation;
    }

    void Update()
    {
        if (playerNear && !hasPopped && Input.GetKeyDown(KeyCode.E))
        {
            hasPopped = true;

            if (fish2 != null)
                fish2.SetActive(true);

            if (lid != null)
                StartCoroutine(OpenAndCloseLid());
        }
    }

    void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("Trigger works!");
        if (!hasPopped)
        {
            hasPopped = true;
            if (fish2) fish2.SetActive(true);
            if (lid) StartCoroutine(OpenAndCloseLid());
        }
        playerNear = true;
    }
}

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }

    private IEnumerator OpenAndCloseLid()
    {
        Quaternion openRotation = Quaternion.Euler(0, 0, lidOpenAngle);
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * lidOpenSpeed;
            lid.localRotation = Quaternion.Slerp(originalLidRotation, openRotation, t);
            yield return null;
        }

        yield return new WaitForSeconds(lidCloseDelay);

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * lidOpenSpeed;
            lid.localRotation = Quaternion.Slerp(openRotation, originalLidRotation, t);
            yield return null;
        }
    }
}
