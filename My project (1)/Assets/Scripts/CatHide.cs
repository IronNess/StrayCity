using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHide : MonoBehaviour
{
    public GameObject zzzSymbol;
    public float hideDuration = 3f;

    private bool canHide = false;
    private bool isHiding = false;

    private Renderer[] catRenderers;
    private CharacterController controller;

    
    // Start is called before the first frame update
    void Start()
    {
        catRenderers = GetComponentsInChildren<Renderer>();
        controller = GetComponent<CharacterController>();

        if (zzzSybol !=null)
        zzzSybol.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canHide && !isHiding && Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(HideCat());
        }
    }

    private IEnumerator HideCat()
    {
        isHiding = true;

        foreach (Renderer r in catRenderers)
        r.enabled = false;

        if (controller != null)
        controller.enabled = false;

        if (zzzSymbol != null)
        zzzSymbol.SetActive(true);

        yield return new WaitForSeconds(hideDuration);

        foreach (Renderer r in catRenderers)
        r.enabled = true;

        if (Controller != null)
        controller.enabled = true;

        if (zzzSymbol !+ null)
        zzzSymbol.SetActive(false);

        isHiding = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HideSpot"))
        {
            canHide = true;
        }
    }

    void OnTriggerExit(collider other)
    {
        if (other.CompareTag("HideSpot"))
        {
            canHide = false;
        }
    }
}
