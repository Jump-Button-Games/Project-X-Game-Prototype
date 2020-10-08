using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] float movSpeed = 6;            //The movement speed when grounded
    [SerializeField] float airMovSpeed;             //The movement speed when in the air
    [SerializeField] float movAccel = 7;            //The maximum change in velocity the player can do on the ground. This determines how responsive the character will be when on the ground.
    [SerializeField] float airMovAccel;             //The maximum change in velocity the player can do in the air. This determines how responsive the character will be in the air.

    [Header("Jump")]
    [SerializeField] float initialJumpForce = 5;        //The force applied to the player when starting to jump
    [SerializeField] float holdJumpForce = 5;           //The force applied to the character when holding the jump button
    [SerializeField] float maxJumpTime = 5;             //The maximum amount of time the player can hold the jump button

    [Header("Ground Detection")]
    [SerializeField] float groundCastRadius = 0.2f;        //Radius of the circle when doing the circle cast to check for the ground
    [SerializeField] float groundCastDist = 0.54f;          //Distance of the circle cast

    [Header("Misc")]
    [SerializeField] float gravityMultiplier = 1f;

    [Header("Player Input Controls")]
    private PlayerInputContoller playerInputController;

    [Header("Player Physics")]
    private Rigidbody2D rb2d;

    private Animator animator;

    private bool isGrounded;

    private bool facingRight = true;

    private bool isJumping;

    void Awake()
    {
        playerInputController = new PlayerInputContoller();

        // Add controls to the registry

        // Horizontal Movement Controls
        playerInputController.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());

        // Jumping Controls
        // Top action here may be able to go into a if statement
        playerInputController.Player.Jump.performed += _ => Jump(); // Should be associated with the Interaction "Press"
        playerInputController.Player.Jump.started += ctx => isJumping = true;
        playerInputController.Player.Jump.canceled += ctx => isJumping = false;
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        playerInputController.Enable();
    }

    void OnDisable()
    {
        playerInputController.Disable();
    }

    void Update()
    {

        rb2d.AddForce(gravityMultiplier * Physics2D.gravity * rb2d.mass, ForceMode2D.Force);

        isGrounded = DoGroundCheck();

        Move(playerInputController.Player.Move.ReadValue<Vector2>());

    }

    void Move(Vector2 movementDirection)
    {
        Vector2 velocity = rb2d.velocity;

        //calculate the ground direction based on the ground normal
        Vector2 groundDir = Vector2.Perpendicular(DoGroundCast()).normalized;
        groundDir.x *= -1; //Vector2.Perpendicular rotates the vector 90 degrees counter clockwise, inverting X. So here we invert X back to normal

        //The velocity we want our character to have. We get the movement direction, the ground direction and the speed we want (ground speed or air speed)
        Vector2 targetVelocity = groundDir * movementDirection * (isGrounded ? movSpeed : airMovSpeed);

        //The change in velocity we need to perform to achieve our target velocity
        Vector2 velocityDelta = targetVelocity - velocity;

        //The maximum change in velocity we can do
        float maxDelta = isGrounded ? movAccel : airMovAccel;

        //Clamp the velocity delta to our maximum velocity change
        velocityDelta.x = Mathf.Clamp(velocityDelta.x, -maxDelta, maxDelta);

        //We don't want to move the character vertically
        velocityDelta.y = 0;

        //Apply the velocity change to the character
        rb2d.AddForce(velocityDelta * rb2d.mass, ForceMode2D.Impulse);

        if (velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (velocity.x < 0 && facingRight)
        {
            Flip();
        }

        animator.SetFloat("velocityX", Mathf.Abs(velocity.x));
    }

    bool DoGroundCheck()
    {
        //If DoGroundCast returns Vector2.zero (it's the same as Vector2(0, 0)) it means it didn't hit the ground and therefore we are not grounded.
        return DoGroundCast() != Vector2.zero;
    }

    Vector2 DoGroundCast()
    {
        //We will use this array to get what the CircleCast returns. The size of this array determines how many results we will get.
        //Note that we have a size of 2, that's because we are always going to get the player as the first element, since the cast
        //has its origin inside the player's collider.
        RaycastHit2D[] hits = new RaycastHit2D[2];

        if (Physics2D.CircleCast(transform.position, groundCastRadius, Vector3.down, new ContactFilter2D(), hits, groundCastDist) > 1)
        {
            return hits[1].normal;
        }

        return Vector2.zero;
    }

    void Jump()
    {

        if (!isGrounded)
        {
            return;
        }

        rb2d.AddForce(Vector2.up * initialJumpForce * rb2d.mass, ForceMode2D.Impulse);

        StartCoroutine(JumpCoroutine());
    }

    IEnumerator JumpCoroutine()
    {
        //true if the player is holding the Jump button down
        bool wantsToJump = true;

        //Counts for how long we've been jumping
        float jumpTimeCounter = 0;

        while (wantsToJump && jumpTimeCounter <= maxJumpTime)
        {

            jumpTimeCounter += Time.deltaTime;

            // isJumping show be true when the jump button is held down
            wantsToJump = isJumping;

            rb2d.AddForce(Vector3.up * holdJumpForce * rb2d.mass * maxJumpTime / jumpTimeCounter);

            yield return null;
        }
    }

    void Flip()
    {
        
        facingRight = !facingRight;

        transform.Rotate(0f, 180f, 0f);
    }

}
