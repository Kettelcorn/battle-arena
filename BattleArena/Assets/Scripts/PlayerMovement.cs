using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpSpeed;

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
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        movement = new Vector3(x, 0, z);

        if (movement.x != 0 || movement.z != 0)
        {
            anim.SetBool("isWalking", true);

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

        transform.LookAt(movement + transform.position);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(0, jumpSpeed, 0);
            anim.SetBool("isGrounded", false);
            anim.SetTrigger("isJumping");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            anim.SetBool("isGrounded", true);
        }
    }
} 
