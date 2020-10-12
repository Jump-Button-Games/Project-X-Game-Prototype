using System.Collections;
using UnityEngine;

public class Shoot : MonoBehaviour
{
	[Header("Shooting Controls")]

	[Tooltip("Tracks when player is shooting so that animation can be triggered")]
	[SerializeField] bool isShooting = false;


	[Header("Shooting Cooldown")]

	[Tooltip("When cooldown is active the player can not shoot.")]
	[SerializeField] bool isCooldownActive = false;

	[Tooltip("The number of seconds the cooldown is active for")]
	[SerializeField] float cooldownSeconds = 1.5f;

	[Tooltip("The amount of time remaining before the player can shoot again")]
	[SerializeField] float timer = 0f;


	[SerializeField] bool burstFire = false;
	[SerializeField] int magazineCapacity = 9;
	[SerializeField] float fireRate = 10f; // How often in seconds the bullet fires
	[SerializeField] float nextFire = 0f;

	[Header("Player Input Controls")]
	PlayerInputContoller playerInputController;

	[Header("Shooting Animation Controls")]
	Animator animator;

	[Header("Point Of Firing Object")]
	public Transform firePoint;

	[Header("Bullet To Be Fired")]
	public GameObject bulletPrefab;

	void Awake()
	{
		// Add controls to the registry
		playerInputController = new PlayerInputContoller();
		playerInputController.Player.Shoot.performed += _ => ShootWeapon();
		playerInputController.Player.Shoot.canceled += ctx => isShooting = false;

		animator = GetComponent<Animator> ();
	}

	void OnEnable()
	{
		playerInputController.Enable();
	}

	void OnDisable()
	{
		playerInputController.Disable();
	}

	void Update()
	{
		if (isCooldownActive)
		{
			timer -= Time.deltaTime;
		}

		if (timer <= 0)
		{
			timer = 0;
			isCooldownActive = false;
		}

		if (isShooting == false)
		{
			animator.SetBool("shooting", isShooting);
		}

		if (Time.time > nextFire && fireRate > 0)
		{
			nextFire = Time.time + fireRate;
			ShootWeapon();
		}
	}

	void ShootWeapon()
	{

		if (burstFire)
		{
			BurstFire();
		}
		else
		{
			// If timer equals 0 then shooting is enabled
			if (timer == 0)
			{
				isShooting = true;
				animator.SetBool("shooting", isShooting);
				Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
				timer = cooldownSeconds;
				isCooldownActive = true;
			}
		}

	}

	void BurstFire()
	{
		// if burst fire is true
		// fire bullet 
		// wait a certain time and fire again
		// if magazine is empty start cooldown time
		isShooting = true;
		animator.SetBool("shooting", isShooting);
		Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

	}
	
}
