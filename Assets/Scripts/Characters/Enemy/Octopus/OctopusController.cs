using UnityEngine;

public class OctopusController : PhysicsObject
{
    [Header("Octopus Controller Values", order = 0)]


    [Header("Behaviour Controls", order = 1)]

    [Tooltip("Determines when the Octopus movement behaviour will start. E.G Choosing different direction to move, stopping etc.")]
    [SerializeField]
    float timeUntilStart = 3f;

    [Tooltip("Determines how often the octopus will change behaviour. E.G Stops every X amount of seconds.")]
    [SerializeField]
    float timeUntilNextBehaviour = 5f;


    [Tooltip("Determines how often the octopus will choose to go left or right. Can change the value to improve chance of moving in 1 particular direction")]
    [SerializeField]
    float percentageChance = 0.5f;


    [Header("Movement Controls")]

    [Tooltip("Determines the speed the Octopus moves at while on the ground or ceiling")]
    [SerializeField]
    float maxSpeedX = 3f;

    [Tooltip("Determines the speed the Octopus moves at while on the walls")]
    [SerializeField]
    float maxSpeedY = 3f;

    [Tooltip("Determines the speed the Octopus moves at while on the walls")]
    [SerializeField]
    bool allowedMove = true;

    [Tooltip("Indicates the Octopus will move in a leftwards direction")]
    [SerializeField]
    readonly float moveLeft = -1f;

    [Tooltip("Indicates the Octopus will move in a rightwards direction")]
    [SerializeField]
    readonly float moveRight = 1f;

    [Tooltip("Indicates the Octopus will move in an upwards direction")]
    [SerializeField]
    readonly float moveUp = 1f;

    [Tooltip("Indicates the Octopus will move in a downwards direction")]
    [SerializeField]
    readonly float moveDown = -1f;


    [Header("Direction Controls")]

    [Tooltip("Tracks the direction the octopus is facing")]
    [SerializeField]
    bool facingRight = false;

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

    [Tooltip("Stores the direction the octupus should travelling when moving in a clockwise direction. E.G Move right, move up etc")]
    Vector2 moveClockwise;

    [Tooltip("Stores the direction the octupus should travelling when moving in a anti-clockwise direction. E.G Move right, move up etc")]
    Vector2 moveAntiClockwise;
    

    [Header("Environment Controls")]

    [Tooltip("The object which fires the raycast to determine if an environmental structure is in front of the octopus")]
    public Transform environmentDetection;

    [Tooltip("Determines length of raycast which is fired in front of octopus to check for environment structures (walls, ceilings etc.). Value of 0.075 is tested and works")]
    [SerializeField]
    float environmentDistanceCheck = 0.075f;

    [Tooltip("The raycast will return information about the environment. Currently used to detect the walls, ceiling and ground")]
    RaycastHit2D environmentInfo;

    
    [Header("Shooting Controls")]

    [Tooltip("The octopus is only allowed shoot when it has stopped moving. This bool lets the OctopusShoot script that movement has stopped")]
    public static bool allowedShoot = false;

    void Start()
    {
        // Once scene starts changing these values will not change behaviour
        InvokeRepeating("FreezeMovement", timeUntilStart, timeUntilNextBehaviour);
    }

    protected override void ComputeVelocity()
    {
        if (!facingRight && allowedMove)
        {
            environmentInfo = Physics2D.Raycast(environmentDetection.position, Vector2.left, environmentDistanceCheck);
            EnvirionmentCheck(environmentInfo);

            moveClockwise = MoveClockwiseDirection(); 

            targetVelocity.x = moveClockwise.x * maxSpeedX;
            velocity.y = moveClockwise.y * maxSpeedY;
        }
        else if (facingRight && allowedMove)
        {
            environmentInfo = Physics2D.Raycast(environmentDetection.position, Vector2.right, environmentDistanceCheck);
            EnvirionmentCheck(environmentInfo);

            moveAntiClockwise = MoveAntiClockwiseDirection();

            targetVelocity.x = moveAntiClockwise.x * maxSpeedX;
            velocity.y = moveAntiClockwise.y * maxSpeedY;
        }
    }


    // Movement Methods

    Vector2 MoveClockwiseDirection()
    {
        // The grounded variable is derivived from the PhysicsObject Script
        if (grounded)
        {
            return new Vector2(moveLeft, 0);
        }
        else if (onLeftWall)
        {
            return new Vector2(0, moveUp);
        }
        else if (onCeiling)
        {
            return new Vector2(moveRight, moveUp);
        }
        else
        {
            // On way own, must apply pressure towards the wall
            return new Vector2(moveRight, moveDown);
        }
    }

    Vector2 MoveAntiClockwiseDirection()
    {
        // The grounded variable is derivived from the PhysicsObject Script
        if (grounded)
        {
            return new Vector2(moveRight, 0);
        }
        else if (onRightWall)
        {
            return new Vector2(0, moveUp);
        }
        else if (onCeiling)
        {
            return new Vector2(moveLeft, moveUp);
        }
        else 
        {
            // On way own, must apply pressure towards the wall
            return new Vector2(moveLeft, moveDown);
        }
    }

    // Enviroment Detection Methods

    void EnvirionmentCheck(RaycastHit2D environmentInfo)
    {
        if (environmentInfo.collider == true)
        {
            if (grounded)
            {
                Rotation(clockwiseRotationInDegrees);
            }
            else if (onLeftWall)
            {
                Rotation(clockwiseRotationInDegrees);
            }
            else if (onCeiling)
            {
                Rotation(clockwiseRotationInDegrees);
            }
            else if (onRightWall)
            {
                Rotation(clockwiseRotationInDegrees);
            }
        }
    }

    // Direction Methods

    void Rotation(float rotateByDegrees)
    {
        if (!facingRight)
        {
            if (grounded)
            {
                transform.Rotate(new Vector3(0, 0, rotateByDegrees));
                grounded = false;
                onLeftWall = true;
            }
            else if (onLeftWall)
            {
                transform.Rotate(new Vector3(0, 0, rotateByDegrees));
                onLeftWall = false;
                onCeiling = true;
            }
            else if (onCeiling)
            {
                transform.Rotate(new Vector3(0, 0, rotateByDegrees));
                onCeiling = false;
                onRightWall = true;
            }
            else if (onRightWall)
            {
                transform.Rotate(new Vector3(0, 0, rotateByDegrees));
                onRightWall = false;
                grounded = true;
            }
        }

        if (facingRight)
        {
            if (grounded)
            {
                transform.Rotate(new Vector3(0, 0, rotateByDegrees));
                grounded = false;
                onRightWall = true;
            }
            else if (onLeftWall)
            {
                transform.Rotate(new Vector3(0, 0, rotateByDegrees));
                onLeftWall = false;
                grounded = true;
            }
            else if (onCeiling)
            {
                transform.Rotate(new Vector3(0, 0, rotateByDegrees));
                onCeiling = false;
                onLeftWall = true;
            }
            else if (onRightWall)
            {
                transform.Rotate(new Vector3(0, 0, rotateByDegrees));
                onRightWall = false;
                onCeiling = true;
            }
        }
        
    }

    void Flip()
    {
        // If onGround
        if (!facingRight && !onCeiling)
        {
            facingRight = true;
            transform.eulerAngles = new Vector3(0, -180, 0);
        }
        else if (facingRight && !onCeiling)
        {
            facingRight = false;
            transform.eulerAngles = new Vector3(0, 0, 0);
        }


        // If onCeiling
        if (!facingRight && onCeiling)
        {
            facingRight = true;
            transform.eulerAngles = new Vector3(0, -180, 180);
        }
        else if (facingRight && onCeiling)
        {
            facingRight = false;
            transform.eulerAngles = new Vector3(0, 0, 180);
        }
    }

    

    void RandomizeFacingDirection()
    {
        if (Random.value >= percentageChance)
        {
            facingRight = true;
            Flip();
        }
        else 
        {
            facingRight = false;
            Flip();
        }
    }

    // Behavioural Methods

    void FreezeMovement()
    {
        // Start moving
        if (gravityModifier == 0 && allowedMove == false)
        {
            RandomizeFacingDirection();
            gravityModifier = 1;
            allowedMove = true;
            allowedShoot = false;
        }
        else if (gravityModifier == 1 && allowedMove == true) // Freeze Movement
        {
            // Not needed but adds a little more life to the octupus
            RandomizeFacingDirection();
            gravityModifier = 0;
            allowedMove = false;
            allowedShoot = true;
        }
    }
}
