using UnityEngine;

public class CrabShoot : MonoBehaviour
{
	[Header("Shooting Controls")]

	[Tooltip("Tracks whether the crab is allowed shoot or not")]
	[SerializeField]
	bool canShoot = false;

	[Tooltip("Determines when the crabs shooting behaviour will start")]
	[SerializeField]
	float timeUntilShootingStarts = 2f;

	[Tooltip("Determines how often the crab will shoot. E.G shoot every X amount of seconds.")]
	[SerializeField]
	float rateOfShooting = 2f;

	[Tooltip("Determines from what distance the crab can see the player")]
	[SerializeField]
	float shotDectectionRange = 10f;


	[Header("Bullet Controls")]

	[Tooltip("The crab bullet prefab")]
	public GameObject crabBulletPrefab;

	[Tooltip("Point from where the crab bullet fires.")]
	public Transform crabFirePoint;

	void Start()
	{
		InvokeRepeating("AllowedShoot", timeUntilShootingStarts, rateOfShooting);
	}

	void Update()
	{
		RaycastHit2D playerInfo = Physics2D.Raycast(crabFirePoint.position, crabFirePoint.right, shotDectectionRange);

		if (playerInfo.collider == true && playerInfo.collider.gameObject.tag == "Player")
		{
			if (canShoot)
			{
				Shoot();
				canShoot = false;
			}
		}
	}

	// Shoot Methods
	
	void AllowedShoot()
	{
		canShoot = true;
	}

	void Shoot()
	{
		Instantiate(crabBulletPrefab, crabFirePoint.position, crabFirePoint.rotation);
	}

	
}
