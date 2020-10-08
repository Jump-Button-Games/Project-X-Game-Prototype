using UnityEngine;

/*
 *  This script may become obsolete over time but keeping it around for now.
 */

public class PlayerPlatformerController : PhysicsObject {

  /*  public float maxSpeed = 5;
    public float jumpTakeOffSpeed = 5;

    private bool facingRight = true;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis ("Horizontal");
       
        // Larger jump if holding down jump button
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            // Smaller jump if a single tap of the jump button
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        if (move.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (move.x < 0 && facingRight)
        {
            Flip();
        }


        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        // Move the character
        targetVelocity = move * maxSpeed;
    }

    void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        transform.Rotate(0f, 180f, 0f);
    }*/

}
