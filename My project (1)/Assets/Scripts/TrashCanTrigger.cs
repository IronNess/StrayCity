using System.Collections;
using UnityEngine;

public class TrashCanTrigger : MonoBehaviour
{
    public GameObject fish2;
    public Transform lid;
    public Transform[] flaps; // Assign flap1, flap2, flap3, flap4
    public float lidOpenAngle = -120f;
    public float lidOpenSpeed = 5f;
    public float lidCloseDelay = 2f;
    public float flapOpenAngle = 120f;
    public Transform popOutPosition;

    private bool hasPopped = false;
    private bool playerNear = false;
    private Quaternion originalLidRotation;
    private Quaternion[] originalFlapRotations;

    void Start()
    {
        if (lid != null)
            originalLidRotation = lid.localRotation;

        if (flaps != null && flaps.Length > 0)
        {
            originalFlapRotations = new Quaternion[flaps.Length];
            for (int i = 0; i < flaps.Length; i++)
                originalFlapRotations[i] = flaps[i].localRotation;
        }
    }

    void Update()
    {
        if (playerNear && !hasPopped && Input.GetKeyDown(KeyCode.E))
        {
            hasPopped = true;

            if (fish2 != null && popOutPosition != null)
            {
                fish2.transform.position = popOutPosition.position;
                fish2.SetActive(true);

                Rigidbody rb = fish2.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.AddForce(popOutPosition.up * 3f + popOutPosition.forward * 2f, ForceMode.Impulse);
                }
            }

            if (lid != null)
                StartCoroutine(OpenAndCloseLid());
            else if (flaps != null && flaps.Length > 0);
                StartCoroutine(OpenAndCloseFlaps());
        }

        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
        Quaternion openRot = Quaternion.Euler(0, 0, lidOpenAngle);
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * lidOpenSpeed;
            lid.localRotation = Quaternion.Slerp(originalLidRotation, openRot, t);
            yield return null;
        }

        yield return new WaitForSeconds(lidCloseDelay);

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * lidOpenSpeed;
            lid.localRotation = Quaternion.Slerp(openRot, originalLidRotation, t);
            yield return null;
        }
    }

    private IEnumerator OpenAndCloseFlaps()
    {
        Quaternion[] openRotations = new Quaternion[flaps.Length];

        for (int i = 0; i < flaps.Length; i++)
        {
            Vector3 axisRotation = Vector3.zero;

            switch (i)
            {

              case 0: axisRotation = new Vector3(flapOpenAngle, 0, 0); break;     // Flap1 → X+
              case 1: axisRotation = new Vector3(0, 0, -flapOpenAngle); break;    // Flap2 → Z-
              case 2: axisRotation = new Vector3(0, 0, flapOpenAngle); break;     // Flap3 → Z+
              case 3: axisRotation = new Vector3(-flapOpenAngle, 0, 0); break;    // Flap4 → X-
            }


            openRotations[i] = flaps[i].localRotation * Quaternion.Euler(axisRotation);
        }

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * lidOpenSpeed;
            for (int i = 0; i < flaps.Length; i++)
            {
                flaps[i].localRotation = Quaternion.Slerp(originalFlapRotations[i], openRotations[i], t);
            }
            yield return null;
        }

        yield return new WaitForSeconds(lidCloseDelay);

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * lidOpenSpeed;
            for (int i = 0; i < flaps.Length; i++)
            {
                flaps[i].localRotation = Quaternion.Slerp(openRotations[i], originalFlapRotations[i], t);
            }
            yield return null;
        }
    }
}
