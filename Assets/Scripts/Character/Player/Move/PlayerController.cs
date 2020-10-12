using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Player Movement")]

    [Tooltip("Maximum Velocity Change Player Can Perform On The Ground. Determines Responsiveness Of Player On The Ground.")]
    [SerializeField] float movementAcceleration = 7;

    [Tooltip("Player Movement Speed When On The Ground.")]
    [SerializeField] float movementSpeed = 6;

    [Tooltip("Maximum Velocity Change Player Can Perform In The Ground. Determines Responsiveness Of Player In The Air.")]
    [SerializeField] float airMovementAcceleration = 0;             

    [Tooltip("Player Movement Speed When In The Air.")]
    [SerializeField] float airMovementSpeed = 0;

    

    [Header("Player Jump")]

    [Tooltip("Force Applied To Player When Tapping Jump. Starts The Initial Jump.")]
    [SerializeField] float jumpForce = 5;

    [Tooltip("Force Applied To Player When Holding Jump Button. Continues The Jump While Jump Button Is Held Down.")]
    [SerializeField] float holdingJumpForce = 5;

    [Tooltip("Maximum Amount Of Time That Force Is Continually Applied To The Player. Only Applies When Jump Button Is Held Down.")]
    [SerializeField] float maxJumpTime = 5;

    [Tooltip("Tracks Whether The Player Is Currently Jumping. Currently When Jump Button Is Pressed Then Value Is True And When Jump Button Is Released Then Value Is False.")]
    [SerializeField] bool isJumping = false;



    [Header("Player Gravity Controls")]

    [Tooltip("Changes How To Player Is Affected By Gravity. This Overrides The Default Value Set By The Physics Engine.")]
    [SerializeField] float gravityMultiplier = 1f;



    [Header("Player Facing Direction")]

    [Tooltip("Tracks Which Way The Player Is Currently Facing. Value Is True If Facing Right And False If Facing Left.")]
    [SerializeField] bool facingRight = true;



    [Header("Ground Detection")]

    [Tooltip("Determines The Size Of The Radius For The Circle. The Circle Cast Is Used To Detect Whether The Player Is In Contact With The Ground Or Not. Currently Casts From The Players" +
        "transform.position But May Be Better To Create An Empty Object And Position The Circle Cast Manaully.")]
    [SerializeField] float groundCastRadius = 0.2f;

    [Tooltip("The Distance To Cast The Circle")]
    [SerializeField] float groundCastDistance = 0.54f;

    [Tooltip("Tracks Whether The Player Is Currently On The Ground Or Not. Value Is True If Player Is On The Ground And False If Not.")]
    [SerializeField] bool isGrounded;

    [Header("Movement Animation Controls")]
    Animator animator;

    [Header("Player Input Controls")]
    PlayerInputContoller playerInputController;

    [Header("Player Physics")]
    Rigidbody2D rb2d;

    void Awake()
    {
        playerInputController = new PlayerInputContoller();

        // Add controls to the registry
        playerInputController.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
        playerInputController.Player.Jump.performed += _ => Jump();
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

        animator.SetBool("grounded", isGrounded);

        Move(playerInputController.Player.Move.ReadValue<Vector2>());
    }

    void Move(Vector2 movementDirection)
    {
        Vector2 velocity = rb2d.velocity;

        //calculate the ground direction based on the ground normal
        Vector2 groundDir = Vector2.Perpendicular(DoGroundCast()).normalized;
        groundDir.x *= -1; //Vector2.Perpendicular rotates the vector 90 degrees counter clockwise, inverting X. So here we invert X back to normal

        //The velocity we want our character to have. We get the movement direction, the ground direction and the speed we want (ground speed or air speed)
        Vector2 targetVelocity = groundDir * movementDirection * (isGrounded ? movementSpeed : airMovementSpeed);

        //The change in velocity we need to perform to achieve our target velocity
        Vector2 velocityDelta = targetVelocity - velocity;

        //The maximum change in velocity we can do
        float maxDelta = isGrounded ? movementAcceleration : airMovementAcceleration;

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

        if (Physics2D.CircleCast(transform.position, groundCastRadius, Vector3.down, new ContactFilter2D(), hits, groundCastDistance) > 1)
        {
            return hits[1].normal;
        }

        return Vector2.zero;
    }

    void Jump()
    {
        Debug.Log("Jumping");

        if (!isGrounded)
        {
            return;
        }

        rb2d.AddForce(Vector2.up * jumpForce * rb2d.mass, ForceMode2D.Impulse);

        // StartCoroutine(JumpCoroutine());
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

            rb2d.AddForce(Vector3.up * holdingJumpForce * rb2d.mass * maxJumpTime / jumpTimeCounter);

            yield return null;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        transform.Rotate(0f, 180f, 0f);
    }

}
