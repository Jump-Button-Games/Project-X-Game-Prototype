using UnityEngine;

public class Bullet : MonoBehaviour
{ 
	[Header("Bullet Movement")]

	[Tooltip("The velocity of the bullet")]
	[SerializeField] float speed = 60f;


	[Header("Bullet Damage Dealt")]

	[Tooltip("The amount of damage the bullet does to the recieving character")]
	[SerializeField] int damage = 40;

	[Header("Bullet Physics")]

	[Tooltip("Attach the rigidbody of the bullet here")]
	public Rigidbody2D rb2d;

	[Header("Impact Effect")]
	[Tooltip("The object which contains the bullet impact effect animation")]
	public GameObject bulletImpactEffect;

	void Start () 
	{
		rb2d.velocity = transform.right * speed;
	}

	// For later on, have a onCollide method which checks if the walls or ceiling was hit
	// then play impact animation and destroy the bullet

	void OnTriggerEnter2D (Collider2D hitInfo)
	{
		Enemy enemy = hitInfo.GetComponent<Enemy>();
        
		if (enemy != null)
		{
			enemy.TakeDamage(damage);
			Instantiate(bulletImpactEffect, transform.position, transform.rotation);
			Destroy(gameObject);
		}
	}
}
