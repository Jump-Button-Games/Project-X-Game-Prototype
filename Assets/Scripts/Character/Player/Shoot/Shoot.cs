using UnityEngine;

public class Shoot : MonoBehaviour
{
	[Header("Shooting Controls")]
	[SerializeField] bool isShooting = false;

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

	void Update () 
	{
		if (isShooting == false)
		{
			animator.SetBool("shooting", isShooting);
		}
    }

	void ShootWeapon ()
	{
		isShooting = true;
		animator.SetBool("shooting", isShooting);
		Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
	}
}
