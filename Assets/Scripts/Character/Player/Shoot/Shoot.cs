using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Transform firePoint;
	public GameObject bulletPrefab;

	private bool isShooting = false;

	private Animator animator;

	[Header("Player Input Controls")]
	private PlayerInputContoller playerInputController;

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
