using UnityEngine;

public class JumperController : PhysicsObject
{
    [Header("Jumper Controller Values", order = 0)]


    [Header("Jump Controls", order = 1)]

    [Tooltip("Determines the force of the jump. Will jump higher and faster with larger jump force. Air time can be increased or reduced by changing the gravity modifier")]
    [SerializeField]
    float jumpForce = 5f;

    [Tooltip("Determines the length of time before the jumper starts jumping. This will also trigger the first jump.")]
    [SerializeField]
    float timeBeforeJumpStarts = 2f;

    [Tooltip("Determines how often the jumper jumps")]
    [SerializeField]
    float rateOfJump = 5f;

    void Start()
    {
        InvokeRepeating("Jump", timeBeforeJumpStarts, rateOfJump);
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
