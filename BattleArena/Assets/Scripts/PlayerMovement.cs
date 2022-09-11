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
    [SerializeField] private float acc;

    private Rigidbody rb;
    private Vector3 movement;
    private float speed;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = walkSpeed;
        anim = GetComponent<Animator>();
    }

    // Updates the players current movement and animations
    void Update()
    {
      
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Checks to see if player is punching, and stops movement if they are
        // If not, the player moves normally based on inputs
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hook Punch") && anim.GetBool("isGrounded"))
        {
            movement = new Vector3(0, 0, 0);
            rb.velocity = new Vector3(0, 0, 0);       
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        // If player is moving horizontally, rotate position of character to look in direction it is moving
        // Sets idle, walk, or running animations based on inputs from user
        if (movement.x != 0 || movement.z != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotateSpeed);
            float blend = Mathf.Min(Mathf.Abs(movement.x) + Mathf.Abs(movement.z), 1);
            anim.SetFloat("Blend", blend);

            if (Input.GetKey("left shift"))
            {
                anim.SetBool("isRunning", true);
                if (anim.GetBool("isGrounded"))
                {
                    anim.SetFloat("Blend", blend);
                    if (speed < runSpeed && anim.GetBool("isGrounded"))
                    speed += speed * Time.fixedDeltaTime * acc;
                }
            }
            else
            {
                anim.SetBool("isRunning", false);
                if (speed > walkSpeed && anim.GetBool("isGrounded"))
                    speed -= speed * Time.fixedDeltaTime * acc;
            }
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        // If player inputs to jump, trigger jump animation and change player position
        if (Input.GetButtonDown("Jump") && anim.GetBool("isGrounded"))
        {
            anim.SetTrigger("isJumping");
            rb.velocity = new Vector3(0, jumpSpeed, 0);
        }

        // If player inputs to punch, trigger punching animation
        if (Input.GetMouseButtonDown(0) && anim.GetBool("isGrounded") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Hook Punch"))
        {
            anim.SetTrigger("isPunching");
        }   
    }

    // On contact with the ground, set isGrounded to true 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            anim.SetBool("isGrounded", true);
        }
    }

    // On exiting contract with the ground, set is grounded to false
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            anim.SetBool("isGrounded", false);
        }
    }
} 
