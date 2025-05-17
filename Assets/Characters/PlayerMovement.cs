using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Joystick joystick;
    private Rigidbody rb;
    private Animator animator;

    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input from joystick
        moveDirection = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        // Update Animator parameters
        animator.SetFloat("Speed", moveDirection.magnitude);

        // Optional: If you want directional parameters
        animator.SetFloat("MoveX", moveDirection.x);
        animator.SetFloat("MoveZ", moveDirection.z);
    }

    void FixedUpdate()
    {
        // Apply movement
        rb.velocity = moveDirection * moveSpeed;

        // Rotate to face movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = toRotation;
        }
    }
}