using UnityEngine;

/*
 * 
 *  The Octopus currently runs on all outer walls. Can have a variant which walks around platforms.
 *  NOTE: the octopus must start on the ground to work correctly.
 *  NOTE: Gravity must be turned off in the RigidBody to have the correct behaviour.
 * 
 */

public class OctopusEnemyMovement : MonoBehaviour
{

    [SerializeField]
    private readonly float acceleration = 5f;

    [SerializeField]
    private readonly float maxSpeedX = 2f;

    [SerializeField]
    private readonly float maxSpeedY = 2f;
    
    private bool onGround = false;
    private bool onCeiling = false;
    private bool onLeftWall = false;
    private bool onRightWall = false;

    public Rigidbody2D rb2d;
    public Transform wallDetection;

    void Awake()
    {
        rb2d.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        onGround = true;
    }

    void Update()
    {
        float velocityAbsoluteX = Mathf.Abs(rb2d.velocity.x);
        float velocityAbsoluteY = Mathf.Abs(rb2d.velocity.y);

        if (velocityAbsoluteX < maxSpeedX && velocityAbsoluteY < maxSpeedY)
        {
            Move();
        }

        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, Vector2.left, 0.01f);

        if (wallInfo.collider == true)
        {

            if (onGround)
            {
                Rotate(-90f);
            }
            else if (onLeftWall)
            {
                Rotate(-90f);
            }
            else if (onCeiling)
            {
                Rotate(-90f);
            }
            else if (onRightWall)
            {
                Rotate(-90f);
            }

        }
    }

    void Move()
    {

        if (onGround)
        {
            rb2d.AddForce(Vector2.left * acceleration);
        }

        if (onLeftWall)
        {
            rb2d.AddForce(Vector2.up * acceleration);
        }

        if (onCeiling)
        {
            rb2d.AddForce(Vector2.right * acceleration);
        }

        if (onRightWall)
        {
            rb2d.AddForce(Vector2.down * acceleration);
        }

    }

    void Rotate(float rotateBy)
    {

        if (onGround)
        {
            transform.Rotate(new Vector3(0, 0, rotateBy));
            onGround = false;
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
            onGround = true;
        }

    }

}
