using UnityEngine;

public class CrabController : PhysicsObject
{

    [Header("Crab Controller Values", order = 0)]


    [Header("Movement Controls", order = 1)]

    [Tooltip("Determines the speed the crab moves at")]
    [SerializeField]
    float maxSpeed = 2f;


    [Header("Direction Controls")]

    [Tooltip("Tracks the direction the crab is facing")]
    [SerializeField]
    bool facingRight = false;


    [Header("Environment Checking Values")]

    [Tooltip("Determines the length of the raycast which is fired downwards towards the ground to check that ground still exists")]
    [SerializeField]
    float groundDistanceCheck = 2f;

    [Tooltip("Determines the length of the raycast which is fired sideways in front of the crab to check for walls")]
    [SerializeField]
    float wallDistanceCheck = 0.1f;


    [Header("Environment Checking Objects")]

    [Tooltip("The object from which the ground detection Raycast is fired")]
    public Transform groundDetection;

    [Tooltip("The object from which the wall detection Raycast is fired")]
    public Transform wallDetection;

    protected override void ComputeVelocity()
    {
        // Reset Vector to zero each frame
        Vector2 move = Vector2.zero;

        // Check For Ground And Walls
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, groundDistanceCheck);
        RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, wallDistanceCheck);

        GroundCheck(groundInfo);
        WallCheck(wallInfo);

        move = MoveDirection();

        targetVelocity = move * maxSpeed;
    }

    // Direction Methods

    Vector2 MoveDirection()
    {
        if (facingRight)
        {
            return Vector2.right;
        }

        if (!facingRight)
        {
            return Vector2.left;
        }

        return Vector2.zero;
    }

    void Flip()
    {

        if (facingRight == false)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
        }
        else if (facingRight == true)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

    }

    // Enviroment Checking Methods

    void GroundCheck(RaycastHit2D groundInfo)
    {
        if (groundInfo.collider == false)
        {
            if (facingRight == true)
            {
                Flip();
                facingRight = false;
            }
            else
            {
                Flip();
                facingRight = true;
            }
        }
    }

    void WallCheck(RaycastHit2D wallInfo)
    {
        if (wallInfo.collider == true)
        {
            if (facingRight == true)
            {
                Flip();
                facingRight = false;
            }
            else
            {
                Flip();
                facingRight = true;
            }
        }
    }

}
