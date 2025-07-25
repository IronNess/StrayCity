using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    public float moveSpeed = 40f;
    public float rotationSpeed = 200f; // How fast the Cat will turn direction
    private Animator animator; // referencing animator for playing animations
    private CharacterController controller; // Character Controller for movement

    private bool isNearItem = false;
    private GameObject currentItem; // For pickup of items


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); // aniamtor attached to my CatObject
        controller = GetComponent<CharacterController>(); // Get character controller

        if (animator != null) animator.applyRootMotion = false; 
        
    }


    // Update is called once per frame
    void Update()
    {
        // Keyboard input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = (transform.right * h + transform.forward * v).normalized; // combine into movement direction vector (X, Y Z)

        controller.Move(moveDir * moveSpeed * Time.deltaTime); // Move cat using character controller

        if (moveDir.sqrMagnitude > 0.0001f) // If movement, rotate cat in correct direction
        {

            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
             // This will rotate smoothly toward the direction the cat is walking
        }


            //Trigger the walking animation
            if (animator != null) animator.SetBool("isWalking", moveDir.sqrMagnitude > 0.0001f);

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            isNearItem = true;
            currentItem = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            isNearItem = false;
            currentItem = null;
        }
    }
}
