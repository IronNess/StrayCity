using System.Collections;
using UnityEngine;

public class TrashCanTrigger : MonoBehaviour
{
    public GameObject fish2;
    public Transform lid;
    public float lidOpenAngle = -120f;
    public float lidOpenSpeed = 5f;
    public float lidCloseDelay = 2f;
    public Transform popOutPosition;         

    private bool hasPopped  = false;
    private bool playerNear = false;
    private Quaternion originalLidRotation;

    void Start()
    {
        if (lid != null)
            originalLidRotation = lid.localRotation;
    }

    void Update()
    {
        if (playerNear)
            Debug.Log("Player is near trash can");

        if (playerNear && !hasPopped && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Pressed E - activating trash can");
            hasPopped = true;

           
            if (fish2 != null && popOutPosition != null)
            {
                Debug.Log("Popping fish out!");
                Debug.Log("FISH BLOCK EXECUTED");
                fish2.transform.position = popOutPosition.position;
                fish2.SetActive(true);

              Rigidbody rb = fish2.GetComponent<Rigidbody>();
               if (rb != null)
               {
                   rb.velocity = Vector3.zero;
                   rb.AddForce(popOutPosition.up * 3f + popOutPosition.forward * 2f,
                               ForceMode.Impulse);
               }
            }
        

            if (lid != null)
                StartCoroutine(OpenAndCloseLid());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger works!");
            playerNear = true;
        }
    }

    private IEnumerator OpenAndCloseLid()
    {
        Debug.Log("Opening lid…");
        Quaternion openRot = Quaternion.Euler(0, 0, lidOpenAngle);

        for (float t = 0; t < 1f; t += Time.deltaTime * lidOpenSpeed)
        {
            lid.localRotation = Quaternion.Slerp(originalLidRotation, openRot, t);
            yield return null;
        }

        Debug.Log("Lid opened. Waiting…");
        yield return new WaitForSeconds(lidCloseDelay);

        for (float t = 0; t < 1f; t += Time.deltaTime * lidOpenSpeed)
        {
            lid.localRotation = Quaternion.Slerp(openRot, originalLidRotation, t);
            yield return null;
        }
        Debug.Log("Lid closed.");
    }
}
