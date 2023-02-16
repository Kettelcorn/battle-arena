using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    public Animator anim;

    private float timer;
    private float randomX;
    private float randomZ;
    private float tempSpeed;
    
    private Vector3 movement;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        tempSpeed = moveSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (timer < 1)
        {
            timer += Time.deltaTime;
        } 
        else if (timer < 3)
        {
            anim.SetBool("isMoving", false);
            Debug.Log("should be still");
            timer += Time.deltaTime;
            moveSpeed = 0;
            
        } 
        else
        {
            timer = 0;
            randomX = Random.Range(-1f, 1f);
            randomZ = Random.Range(-1f, 1f);
            moveSpeed = tempSpeed;
            anim.SetTrigger("startMove");
            anim.SetBool("isMoving", true);
            Debug.Log("should be moving");
        }

        movement = new Vector3(randomX, 0, randomZ);
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotateSpeed);
    }
}
