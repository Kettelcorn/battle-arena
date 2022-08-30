using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;

    private Rigidbody rb;
    private Vector3 movement;
    private int jumpTracker;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpTracker = 1;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // moves player along x and z axis
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        movement = new Vector3(x, 0, z);

        //rotate player to face direction it is moving
        transform.LookAt(movement + transform.position);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        // allows player to jump and double jump
        if (Input.GetButtonDown("Jump") && jumpTracker < 2)
        {
            rb.velocity = new Vector3(0, jumpSpeed, 0);
            jumpTracker++;
        }

    }

    // resets jump tracker upon collision with ground
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) jumpTracker = 0;
    }
}
