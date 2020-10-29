using UnityEngine;

public class JumperEnemyMovement : MonoBehaviour
{

    [SerializeField]
    private float jumpForce = 100f;
    
    [SerializeField]
    private float timeBeforeJumpStarts = 0.1f;

    [SerializeField]
    private float rateOfJump = 2f;

    public Rigidbody2D rb2d;

    void Awake()
    {
        rb2d.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        InvokeRepeating("Jump", timeBeforeJumpStarts, rateOfJump);
    }

    void Jump()
    {
        rb2d.AddForce(Vector2.up * jumpForce);
    }
}
