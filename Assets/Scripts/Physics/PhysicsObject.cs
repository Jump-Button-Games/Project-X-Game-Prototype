using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://learn.unity.com/tutorial/live-session-2d-platformer-character-controller#5c7f8528edbc2a002053b68e")]
public class PhysicsObject : MonoBehaviour
{    
    [Header("Physics Object Values", order = 0)]


    [Header("Slope Controls", order = 1)]

    [Tooltip("Determines the steepness of slope the object can move on")]
    [SerializeField] float minGroundNormalY = .65f;


    [Header("Gravity Controls")]

    [Tooltip("Determines the amount that gravity affects the object")]
    [SerializeField] float gravityModifier = 1f;


    [Header("Ground Detection")]

    [Tooltip("Tracks if the object is considered to be on the ground or not")]
    [SerializeField] protected bool grounded;
    protected Vector2 groundNormal;

  
    [Header("Velocity Variables")]

    [Tooltip("The speed at which the object is moving")]
    [SerializeField] protected Vector2 velocity;
    protected Vector2 targetVelocity;

    [Header("Collision Detection Variables")]
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    [Header("Collision Detection Constants")]
    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    [Header("Objects Physics")]
    protected Rigidbody2D rb2d;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {

    }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;

        velocity.x = targetVelocity.x;

        grounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);

            hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;

                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }
}
