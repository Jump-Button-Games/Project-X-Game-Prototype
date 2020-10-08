using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField]
    private readonly float speed = 15f;

	[SerializeField]
	private readonly int damage = 40;

	public Rigidbody2D rb2d;
	public GameObject impactEffect;

	private Animator animator;

	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	void Start () {
		rb2d.velocity = transform.right * speed;
	}

	void OnTriggerEnter2D (Collider2D hitInfo)
	{
		Enemy enemy = hitInfo.GetComponent<Enemy>();
        
		if (enemy != null)
		{
			enemy.TakeDamage(damage);
		}

		Instantiate(impactEffect, transform.position, transform.rotation);

		Destroy(gameObject);
	}
}
