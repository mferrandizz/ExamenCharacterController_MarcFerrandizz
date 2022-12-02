using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController controller;
    private Animator anim;
    [SerializeField] private Transform cam;

    [Header("Physics")]
    public float speed = 5;
    public float jumpHeight = 1;
    public float gravity = -9.81f;

    [Header("Ground Sensor")]
    public bool isGrounded;
    public Transform groundSensor;
    public float sensorRadius = 0.1f;
    public LayerMask ground;
    private Vector3 playerVelocity;

    [Header("Character Rotation")]
    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    [Header("Virtual Camera Mouse Movement")]
    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }


    void Update()
    {
        Movement();

        Jump();

        
    }


    void Movement()
    {
        float z = Input.GetAxisRaw("Vertical");
        anim.SetFloat("VelZ", z);

        float x = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("VelX", x);

        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if(move != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);

        }


    }

    void Jump()
    {
        if(Physics.Raycast(groundSensor.position, Vector3.down, sensorRadius, ground))
        {
            isGrounded = true;
            Debug.DrawRay(groundSensor.position, Vector3.down * sensorRadius, Color.green);
        }
        else
        {
            isGrounded = false;
            Debug.DrawRay(groundSensor.position, Vector3.down * sensorRadius, Color.red);
        }

        anim.SetBool("Jump", !isGrounded);

        
        if(isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        //si estamos en el suelo y pulasamos el imput de salto hacemos que salte el personaje
        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity); 
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }




}




