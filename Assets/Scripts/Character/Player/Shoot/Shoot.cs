using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Transform firePoint;
	public GameObject bulletPrefab;

	private bool isShooting = false;

	private Animator animator;

	void Awake()
	{
		animator = GetComponent<Animator> ();
	}
	
	void Update () {

		/*if (Input.GetButtonDown("Fire1"))
		{
			ShootWeapon();
		}
		else {
			isShooting = false;
			animator.SetBool("shooting", isShooting);
		}
*/
	}

	void ShootWeapon ()
	{
		isShooting = true;
		animator.SetBool("shooting", isShooting);
		Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
	}
}
