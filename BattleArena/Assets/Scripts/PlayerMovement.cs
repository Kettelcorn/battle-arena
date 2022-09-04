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
    private int jumpTracker;
    private float speed;

    private Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpTracker = 1;
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
                if (jumpTracker == 0) speed = runSpeed;
            }
            else
            {
                anim.SetBool("isRunning", false);
                if (jumpTracker == 0) speed = walkSpeed;
            }
        }
        else anim.SetBool("isWalking", false);

        transform.LookAt(movement + transform.position);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        if (Input.GetButtonDown("Jump") && jumpTracker < 2)
        {
            rb.velocity = new Vector3(0, jumpSpeed, 0);
            jumpTracker++;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpTracker = 0;
        }
    }
} 
