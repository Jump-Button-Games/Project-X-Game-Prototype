using UnityEngine;

[HelpURL("https://learn.unity.com/tutorial/live-session-2d-platformer-character-controller#5c7f8528edbc2a002053b68e")]
public class PlayerController : PhysicsObject
{
    [Header("Player Controller Values", order = 0)]


    [Header("Movement Controls", order = 1)]

    [Tooltip("Determines the max speed the player is allowed run at")]
    [SerializeField] float maxSpeed = 5;


    [Header("Jump Controls")]

    [Tooltip("Determines the height the player can jump at")]
    [SerializeField] float jumpTakeOffSpeed = 5;

    [Tooltip("Tracks whether the player is jumping or not")]
    [SerializeField] bool isJumping;

    [Tooltip("Stops the player from infinitely jumping. The lock ensures the player must complete a jump fully before jumping again")]
    [SerializeField] bool jumpedLocked = false;


    [Header("Direction Controls", order = 0)]

    [Tooltip("Tracks which direction the player is facing")]
    [SerializeField] bool facingRight = true;

    [Tooltip("Static variable which tracks whether the player is in shooting upwards animation. This variable will be read by the Shoot script")]
    public static bool facingUpwards = false;

    [Tooltip("Static variable which tracks whether the player is in crouching animation. This variable will be read by the Shoot script")]
    public static bool crouching = false;


    [Header("Player Input Controls")]
    PlayerInputContoller playerInputController;

    [Header("Movement Animation Controls")]
    Animator animator;

    void Awake()
    {
        playerInputController = new PlayerInputContoller();

        // Move Input Controls

        // This code not needed by the looks of it
        //playerInputController.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());

        // Jump Input Controls
        playerInputController.Player.Jump.performed += _ => ActivateJump(); 
        playerInputController.Player.Jump.canceled += ctx => isJumping = false;

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

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        // Move Left and Right Controls
        move.x = Move(playerInputController.Player.Move.ReadValue<Vector2>());

        // Jump Button Pressed and/or held
        if (isJumping && grounded && !jumpedLocked)
        {
            velocity.y = jumpTakeOffSpeed;
           
            // Need to lock the jump as if button is held then isJumping never 
            // changes to false and player can jump infinitely
            jumpedLocked = true;
        }
        else if (isJumping == false) // Jumping Button Released
        {
            if (velocity.y > 0)
            {
                velocity.y *= 0.5f;
            }
           
            jumpedLocked = false;
        }

        if (move.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (move.x < 0 && facingRight)
        {
            Flip();
        }

        // Controls for player facing upwards or crouching
        move.y = VerticalDirection(playerInputController.Player.Move.ReadValue<Vector2>());

        // If 'W' is pressed
        if (move.y == 1)
        {
            facingUpwards = true;
            animator.SetBool("isFacingUpwards", facingUpwards);
        }
        else 
        {
            facingUpwards = false;
            animator.SetBool("isFacingUpwards", facingUpwards);
        }

        // If 'S' is pressed
        if (move.y == -1)
        {
            crouching = true;
            animator.SetBool("isCrouching", crouching);
        }
        else
        {
            crouching = false;
            animator.SetBool("isCrouching", crouching);
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        // Move the character
        targetVelocity = move * maxSpeed;
    }

    // Movement Methods
    float Move(Vector2 direction)
    {
        return direction.x;
    }

    // Jump Methods
    void ActivateJump()
    {
        isJumping = true;
    }

    // Direction Facing Methods
    void Flip()
    {
        facingRight = !facingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    float VerticalDirection(Vector2 direction)
    {
        return direction.y;
    }
}
