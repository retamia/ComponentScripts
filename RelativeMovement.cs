using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RelativeMovement : MonoBehaviour
{
    public float rotSpeed = 1.5f;
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -10.0f;
    public float minFall = -1.5f;
    public float pushForce = 3.0f;

    [SerializeField] private Transform target;

    private Animator animator;
    private CharacterController charController;
    private float vertSpeed;
    private ControllerColliderHit contact;
    private Vector3 offset;

    void Awake()
    {
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        offset = target.position - transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        minFall = vertSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.zero;

        Move(ref movement);
        Jump(ref movement);

        charController.Move(movement * Time.deltaTime);
    }

    void Move(ref Vector3 movement)
    {
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");

        if (horInput != 0 || vertInput != 0)
        {
            movement.x = horInput * moveSpeed;
            movement.z = vertInput * moveSpeed;

            movement = Vector3.ClampMagnitude(movement, moveSpeed);

            if (Input.GetMouseButton(1))
            {
                Quaternion tmp = target.rotation;
                target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
                movement = target.TransformDirection(movement);
                target.rotation = tmp;
            }
            else
            {
                movement = transform.rotation * movement;
            }
        }

        animator.SetFloat("speed", movement.sqrMagnitude);
    }

    void Jump(ref Vector3 movement)
    {
        bool hitGround = false;
        RaycastHit hit;

        if (vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float check = (charController.height + charController.radius) / 1.9f;
            hitGround = hit.distance <= check;
        }

        if(hitGround) 
        {
            if (Input.GetButtonUp("Jump"))
            {
                vertSpeed = jumpSpeed;
                animator.SetBool("jumping", true);
            }
            else
            {
                vertSpeed = minFall;
                animator.SetBool("jumping", false);
            }
        }
        else 
        {
            vertSpeed += gravity * 5 * Time.deltaTime;
            if (vertSpeed < terminalVelocity)
            {
                vertSpeed = terminalVelocity;
            }

            if (charController.isGrounded)
            {
                if (Vector3.Dot(movement, contact.normal) < 0)
                {
                    movement = contact.normal * moveSpeed;
                }
                else 
                {
                    movement += contact.normal * moveSpeed;
                }
            }
        }

        movement.y = vertSpeed;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contact = hit;

        Rigidbody body = hit.collider.attachedRigidbody;
        if (body != null && !body.isKinematic)
        {
            body.velocity = hit.moveDirection * pushForce;
        }
    }
}
