using UnityEngine;

/*
 
    NOTE: Enemy will turn away when reaching the player as it doesn't differentiate between any colliders 
    as of yet.
 
 */

public class CrabEnemyMovement : MonoBehaviour
{

    // Controls Enemy Movement Speed and Direction
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float maxSpeed = 2f;
    private bool facingRight = false;

    public Rigidbody2D rb2d;
    public Transform groundDetection;
    public Transform wallDetection;

    void Awake()
    {
        rb2d.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {

        float velocityAbsolute = Mathf.Abs(rb2d.velocity.x);

        if (velocityAbsolute < maxSpeed)
        {
            Move();
        }

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 2f);
        RaycastHit2D wallInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, 0.1f);

        if (groundInfo.collider == false)
        {
            if (facingRight == true)
            {
                Flip();
                facingRight = false;
            }
            else {
                Flip();
                facingRight = true;
            }
        }

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

    void Move()
    {

        if(facingRight)
        {
            rb2d.AddForce(Vector2.right * acceleration);
        }

        if (!facingRight)
        {
            rb2d.AddForce(Vector2.left * acceleration);
        }

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

}
