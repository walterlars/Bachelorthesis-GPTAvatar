using UnityEngine;

public class CharacterGravityController : MonoBehaviour
{
    public float gravity = -9.81f; // Gravity strength
    public float groundDistance = 0.4f; // Distance to check for the ground
    public LayerMask groundMask; // Mask to define what is considered ground
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Transform groundCheck;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        groundCheck = new GameObject("GroundCheck").transform;
        groundCheck.SetParent(transform);
        groundCheck.localPosition = new Vector3(0, -controller.height / 2, 0);
    }

    void Update()
    {
        // Check if the character is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ensures the character stays grounded
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move the character
        controller.Move(velocity * Time.deltaTime);
    }
}
