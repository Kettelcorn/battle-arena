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
        Debug.Log(anim);
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
        

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


        if (Input.GetButtonDown("Jump") && anim.GetBool("isGrounded"))
        {
            anim.SetBool("isGrounded", false);
            anim.SetTrigger("isJumping");
            rb.velocity = new Vector3(0, jumpSpeed, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("isPunching");
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
