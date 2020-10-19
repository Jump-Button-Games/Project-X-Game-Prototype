using UnityEngine;

public class Shoot : MonoBehaviour
{
	[Header("Game Time Clock")]

	[Tooltip("Temporary variable to show the overall game time")]
	[SerializeField] float overallGameTime = 0;

	[Header("Shooting Animation Controls")]

	[Tooltip("Tracks when player is shooting so that animation can be triggered. When button is released then shoot animation is cancelled")]
	[SerializeField] bool isShooting = false;


	[Header("Shooting Direction")]

	[Tooltip("The point where the bullets instantiates when shooting upwards")]
	[SerializeField] bool isFacingUpwards = false;


	[Header("Single Shot Controls")]

	[Tooltip("Tracks when the single shot button is pressed and allows the action to complete")]
	[SerializeField] bool isSingleShooting = false;

	[Tooltip("The time is seconds before the next shot fires off")]
	[SerializeField] float singleShotTime = 1.5f;

	[Tooltip("The time when the next shot will be fired. This is used in combination with Time.time + singleShotTime to set the next time the shot be fired")]
	[SerializeField] float nextSingleShotTime = 0f;

	[Tooltip("The number of shots allowed in the single shot before action is complete")]
	[SerializeField] int singleShotsAllowed = 1;

	[Tooltip("The number of shots that remain in the single shot before action is complete")]
	[SerializeField] int singleShotsRemaining;


	[Header("Burst Shot Controls")]

	[Tooltip("Tracks when the burst shot button is pressed and allows the action to complete")]
	[SerializeField] bool isBurstShooting = false;

	[Tooltip("The time is seconds before the next shot fires off")]
	[SerializeField] float burstShotTime = 1f; 

	[Tooltip("The time when the next shot will be fired. This is used in combination with Time.time + burstShotTime to set the next time the shot be fired")]
	[SerializeField] float nextBurstShotTime = 0f;

	[Tooltip("The number of shots allowed in the burst shot before action is complete")]
	[SerializeField] int burstShotsAllowed = 3;

	[Tooltip("The number of shots that remain in the burst shot before action is complete")]
	[SerializeField] int burstShotsRemaining;


	[Header("Conintuous Shot Controls")]

	[Tooltip("Tracks whether the shoot button is held down. Variable is true is held down.")]
	[SerializeField] bool isContinuouslyShooting = false;

	[Tooltip("The time is seconds before the next shot fires off")]
	[SerializeField] float continousShotTime = 1f;

	[Tooltip("The time when the next shot will be fired. This is used in combination with Time.time + continousShotTime to set the next time the shot be fired")]
	[SerializeField] float nextContinuousShotTime = 0f;


	[Header("Point Of Sideways Firing Object")]

	[Tooltip("The point where the bullets instantiates when shooting sideways")]
	public Transform firePointSideways;


	[Header("Point Of Upwards Firing Object")]

	[Tooltip("The point where the bullets instantiates when shooting upwards")]
	public Transform firePointUpwards;


	[Header("Bullet To Be Fired")]

	[Tooltip("This bullet moves in a sideways direction")]
	public GameObject bulletSideways;

	[Header("This bullet moves in an upwards direction")]
	public GameObject bulletUpwards;


	[Header("Player Input Controls")]
	PlayerInputContoller playerInputController;

	[Header("Shooting Animation Controls")]
	Animator animator;

	void Awake()
	{
		// Add controls to the registry
		playerInputController = new PlayerInputContoller();

		// Single Shot
		playerInputController.Player.SingleShot.performed += _ => ActivateSingleShot();

		// Burst Shot
		playerInputController.Player.BurstShot.performed += _ => AcivateBurstShot();

		// Continuous Shot
		playerInputController.Player.ContinuousShot.performed += _ => ActivateContinuousShot();


		// Controls Shooting Animation. Release Button To Cancel Shooting Animation
		playerInputController.Player.SingleShot.canceled += ctx => isShooting = false;

		// When Conintuous Shot Button Is Released The Shot Is Finished
		playerInputController.Player.ContinuousShot.canceled += ctx => isContinuouslyShooting = false;

		animator = GetComponent<Animator> ();

		// Set Number Of Shots Allowed For Each Type Of Shot
		singleShotsRemaining = singleShotsAllowed;
		burstShotsRemaining = burstShotsAllowed;

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
		// Temp code to show how burst shots works
		overallGameTime = Time.time;

		// Continously Check If Player Is Facing Upwards Or Not
		isFacingUpwards = PlayerController.facingUpwards;

		// Single Shot
		if (isSingleShooting && Time.time > nextSingleShotTime && singleShotTime > 0 && !isFacingUpwards)
		{
			if (singleShotsRemaining > 0)
			{
				nextSingleShotTime = Time.time + singleShotTime;
				SingleShot();
				singleShotsRemaining--;
			}
			else
			{
				animator.SetBool("shooting", false);
				ResetSingleShot();
			}
		}
		else if (isSingleShooting && Time.time > nextSingleShotTime && singleShotTime > 0 && isFacingUpwards)
		{
			if (singleShotsRemaining > 0)
			{
				nextSingleShotTime = Time.time + singleShotTime;
				SingleShotUpwards();
				singleShotsRemaining--;
			}
			else
			{
				ResetSingleShot();
			}
		}

		// Burst Shot
		if (isBurstShooting && Time.time > nextBurstShotTime && burstShotTime > 0 && !isFacingUpwards)
		{
			if (burstShotsRemaining > 0)
			{
				nextBurstShotTime = Time.time + burstShotTime;
				BurstShot();
				burstShotsRemaining--;
			}
			else
			{
				animator.SetBool("shooting", false);
				ResetBurstShot();
			}
		}
		else if (isBurstShooting && Time.time > nextBurstShotTime && burstShotTime > 0 && isFacingUpwards)
		{
			if (burstShotsRemaining > 0)
			{
				nextBurstShotTime = Time.time + burstShotTime;
				BurstShotUpwards();
				burstShotsRemaining--;
			}
			else
			{
				ResetBurstShot();
			}
		}

		// Continuous Shot
		if (isContinuouslyShooting && Time.time > nextContinuousShotTime && continousShotTime > 0 && !isFacingUpwards)
		{

			nextContinuousShotTime = Time.time + continousShotTime;
			ContinuousShot();
		}
		else if (isContinuouslyShooting && Time.time > nextContinuousShotTime && continousShotTime > 0 && isFacingUpwards)
		{
			nextContinuousShotTime = Time.time + continousShotTime;
			ContinuousShotUpwards();
		}

		if (isShooting == false)
		{
			animator.SetBool("shooting", isShooting);
		}

	}

	// Single Shot Methods
	void ActivateSingleShot()
	{
		isSingleShooting = true;
	}

	// Single Shot Methods
	void SingleShot()
    {
        Shot();
    }

	void SingleShotUpwards()
	{
		ShotUpwards();
	}

	void ResetSingleShot()
	{
		isSingleShooting = false;
		singleShotsRemaining = singleShotsAllowed;
	}

	// Burst Shot Methods
	void AcivateBurstShot()
	{
		isBurstShooting = true;
	}

	void BurstShot()
	{
		Shot();
	}

	void BurstShotUpwards()
	{
		ShotUpwards();
	}

	void ResetBurstShot()
	{
		isBurstShooting = false;
		burstShotsRemaining = burstShotsAllowed;
	}

	// Continuous Shot Methods
	void ActivateContinuousShot()
	{
		isContinuouslyShooting = true;
	}

	void ContinuousShot()
	{
		Shot();
	}

	void ContinuousShotUpwards()
	{
		ShotUpwards();
	}

	// Generic Methods
	void Shot()
	{
		isShooting = true;
		animator.SetBool("shooting", isShooting);
		Instantiate(bulletSideways, firePointSideways.position, firePointSideways.rotation);
	}

	void ShotUpwards()
	{
		Instantiate(bulletUpwards, firePointUpwards.position, firePointUpwards.rotation);
	}
}
