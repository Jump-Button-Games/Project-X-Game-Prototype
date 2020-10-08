using UnityEngine;

/*
 
	This current script allows the enemy to shoot at the player when within a given range.
	There is a timer before the enemy can start to shoot as well as a timer for how often 
	the enemy can shoot.
 
 */

public class EnemyShoot : MonoBehaviour
{
	
	public Transform firePoint;
	public GameObject bulletPrefab;

	[SerializeField]
	private readonly float shotDectectionRange = 10f;

	[SerializeField]
	private readonly float timeBeforeShootingBegins = 2f;

	[SerializeField]
	private readonly float rateOfShooting = 4f;

	private bool canShoot = false;

	void Start()
	{
		InvokeRepeating("AllowedShoot", timeBeforeShootingBegins, rateOfShooting); 
	}

	void Update()
	{

		// Fire raycast in the direction the Transform firePoint is facing
		// This will be the direction the enemy is facing
		RaycastHit2D playerInfo = Physics2D.Raycast(firePoint.position, firePoint.right, shotDectectionRange);

		// If enemy detects player with raycast then shoot
		if (playerInfo.collider == true && playerInfo.collider.gameObject.tag == "Player")
		{

			if (canShoot)
			{
				Shoot();
				canShoot = false;
			}

		}

	}

	void Shoot()
	{
		Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
	}

	void AllowedShoot() 
	{
		canShoot = true;
	}

}
