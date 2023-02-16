using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject volleyballPrefab;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private GameObject firePosition;


    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float acc;
    [SerializeField] private float dmgMulti;
    [SerializeField] private float vertKnock;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float spawnDistance;

    private GameObject flame;
    private Rigidbody rb;
    private Vector3 movement;
    private float speed;
    private Animator anim;
    private bool hitStatus;
    private bool jetFire;

    public bool punched;

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

        if (Input.GetKey(KeyCode.Space) && !anim.GetBool("isGrounded"))
        {
            if (!jetFire)
            {
                Transform flamePoint = transform.Find("firePosition");
                flame = Instantiate(firePrefab, firePosition.transform.position, Quaternion.Euler(180f, 0f, 0f), flamePoint);
                jetFire = true;
            }

            rb.AddForce(Vector3.up * 2f, ForceMode.Force);

        }
        else
        {
            Destroy(flame);
            jetFire = false;
        }

        // If player inputs to punch, trigger punching animation
        if (Input.GetMouseButtonDown(0) && anim.GetBool("isGrounded") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Hook Punch"))
        {
            movement = new Vector3(0, 0, 0);
            rb.velocity = new Vector3(0, 0, 0);
            anim.SetTrigger("isPunching");
        }   

        if (Input.GetMouseButtonDown(1))
        {
            anim.SetTrigger("isMagic");
            StartCoroutine(delay());
        }

    }


    IEnumerator delay()
    {
        yield return new WaitForSeconds(1.4f);
        Vector3 startingPosition = new Vector3(transform.position.x + transform.forward.x * spawnDistance,
            transform.position.y + 1, transform.position.z + transform.forward.z * spawnDistance);

        GameObject volleyball = Instantiate(volleyballPrefab, startingPosition, transform.rotation);

        Vector3 shoot = new Vector3(transform.forward.x * projectileSpeed, 4, transform.forward.z * projectileSpeed);
        volleyball.GetComponent<Rigidbody>().velocity = shoot;

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

    private void OnTriggerStay(Collider other)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hook Punch") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5 &&
            hitStatus != true)      
        {
            punched = true;
            float rotation = transform.localEulerAngles.y;
            float xSpeed = 0;
            float zSpeed = 0;

            if (rotation <= 90)
            {
                xSpeed = rotation;
                zSpeed = (rotation - 90) * -1;
            }
            else if (rotation <= 180)
            {
                xSpeed = (rotation - 180) * -1;
                zSpeed = (rotation - 90) * -1;
            }
            else if (rotation <= 270)
            {
                xSpeed = (rotation - 180) * -1;
                zSpeed = rotation - 270;
            }
            else
            {
                xSpeed = rotation - 360;
                zSpeed = rotation - 270; 
            }

            other.attachedRigidbody.AddForce(new Vector3(xSpeed * dmgMulti, vertKnock * dmgMulti, zSpeed * dmgMulti));
            other.gameObject.GetComponent<Enemy>().anim.SetTrigger("isPunched");
            hitStatus = true;
        }
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hook Punch"))
            hitStatus = false;
    }
} 
