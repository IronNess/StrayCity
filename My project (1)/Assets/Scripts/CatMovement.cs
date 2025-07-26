using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    public float moveSpeed = 40f;
    public float rotationSpeed = 200f;

    private Animator animator;
    private CharacterController controller;

    private bool isNearItem = false;
    private GameObject currentItem;

    private bool nearEnemy = false;
    private GameObject currentEnemy;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        if (animator != null)
            animator.applyRootMotion = false;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = (transform.right * h + transform.forward * v).normalized;
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        if (animator != null)
            animator.SetBool("isWalking", moveDir.sqrMagnitude > 0.0001f);

        // E to collect fish
        if (isNearItem && Input.GetKeyDown(KeyCode.E))
        {
            if (animator != null)
                animator.SetTrigger("CollectItem");

            if (currentItem != null)
            {
                Destroy(currentItem);
                Debug.Log("Item Collected");
            }

            isNearItem = false;
        }

        // F to attack enemy (rat)
        if (nearEnemy && Input.GetKeyDown(KeyCode.F))
        {
            if (animator != null)
                animator.SetTrigger("CollectItem"); // reuse animation

            if (currentEnemy != null)
            {
                Destroy(currentEnemy);
                Debug.Log("Enemy defeated!");
            }

            nearEnemy = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            isNearItem = true;
            currentItem = other.gameObject;
        }

        if (other.CompareTag("Enemy"))
        {
            nearEnemy = true;
            currentEnemy = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            isNearItem = true;
            currentItem = other.gameObject;
        }

        else if (other.CompareTag("Enemy"))
        {
           if (animator != null)
           animator.SetTrigger("CollectItem");

           Destroy(other.gameObject);
           Debug.Log("Rat Caught");
        }
    }
}
