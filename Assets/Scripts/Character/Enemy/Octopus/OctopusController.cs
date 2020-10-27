using UnityEngine;

public class OctopusController : PhysicsObject
{
    [Header("Octopus Controller Values", order = 0)]


    [Header("Movement Controls", order = 1)]

    [Tooltip("Determines the speed the Octopus moves at while on the ground or ceiling")]
    [SerializeField]
    float maxSpeedX = 3f;

    [Tooltip("Determines the speed the Octopus moves at while on the walls")]
    [SerializeField]
    float maxSpeedY = 3f;


    [Header("Direction Controls")]

    [Tooltip("Tracks whether the octopus is on the ceiling or not")]
    [SerializeField]
    bool onCeiling = false;

    [Tooltip("Tracks whether the octopus is on the left wall or not")]
    [SerializeField]
    bool onLeftWall = false;

    [Tooltip("Tracks whether the octopus is on the right wall or not")]
    [SerializeField]
    bool onRightWall = false;

    [Tooltip("The degrees at which to rotate the sprite. This rotates in a clockwise fashion")]
    [SerializeField]
    float clockwiseRotationInDegrees = -90f;


    [Header("Environment Controls")]

    [Tooltip("The object which fires the raycast to determine if an environmental structure is in front of the octopus")]
    public Transform environmentDetection;

    [Tooltip("Determines length of raycast which is fired in front of octopus to check for environment structures (walls, ceilings etc.). Value of 0.075 is tested and works")]
    [SerializeField]
    float environmentDistanceCheck = 0.075f;

    protected override void ComputeVelocity()
    {
        RaycastHit2D environmentInfo = Physics2D.Raycast(environmentDetection.position, Vector2.left, environmentDistanceCheck);

        EnvirionmentCheck(environmentInfo);

        Vector2 move = MoveDirection();

        targetVelocity.x = move.x * maxSpeedX;
        velocity.y = move.y * maxSpeedY;
    }

    Vector2 MoveDirection()
    {
        // The grounded variable is derivived from the PhysicsObject Script
        if (grounded)
        {
            return new Vector2(-1f, 0);
        }
        else if (onLeftWall)
        {
            return new Vector2(0, 1f);
        }
        else if (onCeiling)
        {
            return new Vector2(1f, 1f);
        }
        else
        {
            return new Vector2(1f, -1f);
        }
    }

    void EnvirionmentCheck(RaycastHit2D wallInfo)
    {
        if (wallInfo.collider == true)
        {
            if (grounded)
            {
                RotateClockwise(clockwiseRotationInDegrees);
            }
            else if (onLeftWall)
            {
                RotateClockwise(clockwiseRotationInDegrees);
            }
            else if (onCeiling)
            {
                RotateClockwise(clockwiseRotationInDegrees);
            }
            else if (onRightWall)
            {
                RotateClockwise(clockwiseRotationInDegrees);
            }
        }
    }

    void RotateClockwise(float rotateBy)
    {
        if (grounded)
        {
            transform.Rotate(new Vector3(0, 0, rotateBy));
            grounded = false;
            onLeftWall = true;
        }
        else if (onLeftWall)
        {
            transform.Rotate(new Vector3(0, 0, rotateBy));
            onLeftWall = false;
            onCeiling = true;
        }
        else if (onCeiling)
        {
            transform.Rotate(new Vector3(0, 0, rotateBy));
            onCeiling = false;
            onRightWall = true;
        }
        else if (onRightWall)
        {
            transform.Rotate(new Vector3(0, 0, rotateBy));
            onRightWall = false;
            grounded = true;
        }
    }
}
