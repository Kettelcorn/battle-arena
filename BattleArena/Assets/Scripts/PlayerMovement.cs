using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float rotateSpeed;

    private Rigidbody rb;
    private Vector3 movement;
    private float speed;

    private Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = walkSpeed;
        anim = GetComponent<Animator>();
        Debug.Log(anim);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
        

        if (movement.x != 0 || movement.z != 0)
        {
            anim.SetBool("isWalking", true);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotateSpeed);

            if (Input.GetKey("left shift"))
            {
                anim.SetBool("isRunning", true);
                if (anim.GetBool("isGrounded")) speed = runSpeed;
            }
            else
            {
                anim.SetBool("isRunning", false);
                if (anim.GetBool("isGrounded")) speed = walkSpeed;
            }
        }
        else
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }


        if (Input.GetButtonDown("Jump") && anim.GetBool("isGrounded"))
        {
            anim.SetBool("isGrounded", false);
            anim.SetTrigger("isJumping");
            //rb.AddForce(0, jumpSpeed, 0);
            rb.velocity = new Vector3(0, jumpSpeed, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            anim.SetBool("isGrounded", true);
        }
    }

    private void OnCollistionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            anim.SetBool("isGrounded", false);
        }
    }
} 
