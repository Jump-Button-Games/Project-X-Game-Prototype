using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	[Header("Player Health Stats")]

	[Tooltip("The amount of health the player has remaining")]
	[SerializeField] int health = 100;


	[Header("Player Invincibilty Controls")]

	[Tooltip("Tracks whether the player has invincibility or not")]
	[SerializeField] bool invincible = false;

	[Tooltip("The time in seconds which the player will be invincible for")]
	[SerializeField] float invincibilityDuration = 3f;

	[Tooltip("The time until the player is no longer invincible")]
	[SerializeField] float invicibilityEndTime = 0f;


	[Header("Game Time Clock")]

	[Tooltip("Temporary variable to show the overall game time")]
	[SerializeField] float overallGameTime = 0f;


	[Header("Prefab Containing Death Effect")]
	[Tooltip("The prefab which contains the death effect")]
	public GameObject deathEffect;


	[Header("Hurt Animation Controls")]

	[Tooltip("The length of time the animation can play for")]
	[SerializeField] float animationDuration = 0.2f;

	[Tooltip("The time the animation will stop playing")]
	[SerializeField] float animationEndTime = 0f;

	[Tooltip("The value to access the players animator")]
	Animator animator;


	[Header("Player Fade Controls")]

	[Tooltip("The script to access the player fade methods")]
	public PlayerFade playerFade;

	void Awake()
	{
		animator = GetComponent<Animator>();
		playerFade.GetComponent<PlayerFade>();
	}

	void Update()
	{
		// Temp code to show how burst shots works
		overallGameTime = Time.time;

		// Turn off invinvibilty after the invincibilityDuration time is up
		if (invincible && Time.time > invicibilityEndTime)
		{
			ResetInvincibility();
			StartCoroutine(playerFade.FadeIn());
		}
		else if (EnemyBullet.hitPlayer && !invincible && Time.time > invicibilityEndTime)
		{
			animator.SetBool("isHit", EnemyBullet.hitPlayer);

			invicibilityEndTime = Time.time + invincibilityDuration;
			animationEndTime = Time.time + animationDuration;

			Invincible();
			StartCoroutine(playerFade.FadeOut());
			ResetIsHit();
		}

		// When game time surpasses animationEndTime return to idle state
		if (!EnemyBullet.hitPlayer && Time.time > animationEndTime)
		{
			animator.SetBool("isHit", EnemyBullet.hitPlayer);
		}
		
	}

	// Player Hit And Damage Methods
	public void TakeDamage(int damage)
	{
		if (!invincible)
		{
			health -= damage;

			if (health <= 0)
			{
				Die();
			}
		}
	}

	void Die()
	{
		// Using the enemy death effect for now
		Instantiate(deathEffect, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

	void ResetIsHit()
	{
		EnemyBullet.hitPlayer = false;
	}

	// Player Invincibility Methods
	void Invincible()
	{
		invincible = true;
	}

	void ResetInvincibility()
	{
		invincible = false;
	}

}
