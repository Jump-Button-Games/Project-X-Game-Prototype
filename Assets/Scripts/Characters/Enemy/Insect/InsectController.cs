using UnityEngine;

public class InsectController : PhysicsObject
{
    [Header("Insect Controller Values", order = 0)]


    [Header("Insect Controls", order = 1)]

    [Tooltip("Determines the force of the jump. Will jump higher and faster with larger jump force. Air time can be increased or reduced by changing the gravity modifier")]
    [SerializeField]
    float jumpForce = 6f;

    [Tooltip("Determines the length of time before the insect starts jumping. This will also trigger the first jump.")]
    [SerializeField]
    float timeBeforeJumpStarts = 2f;

    [Tooltip("Determines how often the insect jumps")]
    [SerializeField]
    float rateOfJump = 5f;


    [Header("Shoot Controls")]

    [Tooltip("The insect will shoot only when it has jumped")]
    public static bool allowedShoot = false;

    void Start()
    {
        InvokeRepeating("Jump", timeBeforeJumpStarts, rateOfJump);
    }

    void Update()
    {
        if (grounded)
        {
            allowedShoot = false;
        }

        // If peak of jump has been reached
        if (!grounded && velocity.y < 0)
        {
            allowedShoot = true;
        }
    }

    // Jump Methods

    void Jump()
    {
        if (grounded)
        {
            velocity.y = jumpForce;
        }
    }
}
