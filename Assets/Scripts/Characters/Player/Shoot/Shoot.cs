using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UIElements;

public class Shoot : MonoBehaviour
{
	[Header("Game Time Clock")]

	[Tooltip("Temporary variable to show the overall game time")]
	[SerializeField] float overallGameTime = 0;

	[Header("Shooting Animation Controls")]

	[Tooltip("Tracks when player is shooting so that animation can be triggered. When button is released then shoot animation is cancelled")]
	[SerializeField] bool isShooting = false;


	[Header("Player Shooting Position")]

	[Tooltip("Tracks whether the player is facing upwards. The value for this variable originates in the PlayerController script")]
	[SerializeField] bool isFacingUpwards = false;

	[Tooltip("Tracks whether the player is crouching. The value for this variable originates in the PlayerController script")]
	[SerializeField] bool isCrouching = false;


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


	[Header("Firepoints")]

	[Tooltip("The point where the bullets instantiates when shooting sideways")]
	public Transform firePointSideways;

	[Tooltip("The point where the bullets instantiates when shooting upwards")]
	public Transform firePointUpwards;

	[Tooltip("The point where the bullets instantiates when shooting in a crouched position")]
	public Transform firePointCrouching;


	[Header("Bullets To Be Fired")]

	[Tooltip("This bullet moves in a sideways direction")]
	public GameObject bulletSideways;

	[Tooltip("This bullet moves in an upwards direction")]
	public GameObject bulletUpwards;

	[Tooltip("This bullet moves in an sideways direction but from a lower point")]
	public GameObject bulletCrouching;


	[Header("Camera Shake Controls")]

	[Tooltip("The camera object which contains the script CameraShaker")]
	public CameraShaker cameraShaker;

	[Tooltip("The length of time in seconds the camera should shake for")]
	[SerializeField] float shakeDuration = 0.1f;

	[Tooltip("The distance the camera should shake for on each axis")]
	[SerializeField] float shakeMagnitude = 0.1f;


	[Header("Player Input Controls")]
	PlayerInputContoller playerInputController;

	[Header("Shooting Animation Controls")]
	Animator animator;


	[Header("Gun System Controls")]

	[Tooltip("The id for the gun. Allows player to switch in game")]
	[SerializeField] int gunId;

	[Tooltip("The gun which the has switched too")]
	[SerializeField]  Gun newGun;


	[Header("UI Controls")]

	[Tooltip("The UI Document contains information about the visual elments shown on screen")]
	public UIDocument uiDocument;

	[Tooltip("The label gets updated when the players health changes")]
	Label selectedGunLabel;

	enum Gun
	{
		Pistol,
		Semi,
		Auto
	}

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

		// Select Gun
		playerInputController.Player.SelectGun.performed += _ => CycleThroughGuns();

		// Controls Shooting Animation. Release Button To Cancel Shooting Animation
		playerInputController.Player.SingleShot.canceled += ctx => isShooting = false; // For burst shot this causes the animation to end before 2nd shot is triggered

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

		VisualElement rootVisualElement = uiDocument.rootVisualElement;
		selectedGunLabel = rootVisualElement.Q<Label>("Gun-Select-Label");
		selectedGunLabel.text = $"Gun: {Gun.Pistol.ToString()}";
	}

	void OnDisable()
	{
		playerInputController.Disable();
	}

	void Update()
	{
		overallGameTime = Time.time;

		isFacingUpwards = PlayerController.facingUpwards;
		isCrouching = PlayerController.crouching;

		if (newGun == Gun.Pistol)
		{
			PistolShot();
		}

        if (newGun == Gun.Semi)
        {
            SemiAutomaticShot();
		}

        if (newGun == Gun.Auto)
        {
            AutomaticShot();
		}

        if (isShooting == false)
		{
			animator.SetBool("shooting", isShooting);
		}
	}

    // Gun Shooting Methods

    void PistolShot()
    {
        // Single Shot
        if (isSingleShooting && Time.time > nextSingleShotTime && singleShotTime > 0 && isFacingUpwards)
        {
            if (singleShotsRemaining > 0)
            {
                nextSingleShotTime = Time.time + singleShotTime;
                SingleShotUpwards();
                CameraShake(shakeDuration, shakeMagnitude);
                singleShotsRemaining--;
            }
            else
            {
                ResetSingleShot();
            }
        }
        else if (isSingleShooting && Time.time > nextSingleShotTime && singleShotTime > 0 && isCrouching)
        {
            if (singleShotsRemaining > 0)
            {

                nextSingleShotTime = Time.time + singleShotTime;
                SingleShotCrouching();
                CameraShake(shakeDuration, shakeMagnitude);
                singleShotsRemaining--;
            }
            else
            {
                ResetSingleShot();
            }
        }
        else if (isSingleShooting && Time.time > nextSingleShotTime && singleShotTime > 0)
        {
            if (singleShotsRemaining > 0)
            {
                nextSingleShotTime = Time.time + singleShotTime;
                SingleShot();
                CameraShake(shakeDuration, shakeMagnitude);
                singleShotsRemaining--;
            }
            else
            {
                animator.SetBool("shooting", false);
                ResetSingleShot();
            }
        }

		//isSingleShooting = false;
    }

	void SemiAutomaticShot()
	{
		// Burst Shot
		if (isBurstShooting && Time.time > nextBurstShotTime && burstShotTime > 0 && isFacingUpwards)
		{
			if (burstShotsRemaining > 0)
			{
				nextBurstShotTime = Time.time + burstShotTime;
				BurstShotUpwards();
				CameraShake(shakeDuration, shakeMagnitude);
				burstShotsRemaining--;
			}
			else
			{
				ResetBurstShot();
			}
		}
		else if (isBurstShooting && Time.time > nextBurstShotTime && burstShotTime > 0 && isCrouching)
		{
			if (burstShotsRemaining > 0)
			{
				nextBurstShotTime = Time.time + burstShotTime;
				BurstShotCrouching();
				CameraShake(shakeDuration, shakeMagnitude);
				burstShotsRemaining--;
			}
			else
			{
				ResetBurstShot();
			}
		}
		else if (isBurstShooting && Time.time > nextBurstShotTime && burstShotTime > 0)
		{
			if (burstShotsRemaining > 0)
			{
				nextBurstShotTime = Time.time + burstShotTime;
				BurstShot();
				CameraShake(shakeDuration, shakeMagnitude);
				burstShotsRemaining--;
			}
			else
			{
				animator.SetBool("shooting", false);
				ResetBurstShot();
			}
		}
	}

	void AutomaticShot()
	{
		// Continuous Shot
		if (isContinuouslyShooting && Time.time > nextContinuousShotTime && continousShotTime > 0 && isFacingUpwards)
		{
			nextContinuousShotTime = Time.time + continousShotTime;
			ContinuousShotUpwards();
			CameraShake(shakeDuration, shakeMagnitude);
		}
		else if (isContinuouslyShooting && Time.time > nextContinuousShotTime && continousShotTime > 0 && isCrouching)
		{
			nextContinuousShotTime = Time.time + continousShotTime;
			ContinuousShotCrounching();
			CameraShake(shakeDuration, shakeMagnitude);
		}
		else if (isContinuouslyShooting && Time.time > nextContinuousShotTime && continousShotTime > 0)
		{
			nextContinuousShotTime = Time.time + continousShotTime;
			ContinuousShot();
			CameraShake(shakeDuration, shakeMagnitude);
		}
	}

	// Single Shot Methods
	void ActivateSingleShot()
	{
		if (newGun == Gun.Pistol)
		{
			isSingleShooting = true;
		}
	}

	void SingleShot()
    {
        Shot();
    }

	void SingleShotUpwards()
	{
		ShotUpwards();
	}

	// SingleShotCrouching
	void SingleShotCrouching()
	{
		ShotCrouching();
	}

	void ResetSingleShot()
	{
		isSingleShooting = false;
		singleShotsRemaining = singleShotsAllowed;
	}

	// Burst Shot Methods
	void AcivateBurstShot()
	{
		if (newGun == Gun.Semi)
		{
			isBurstShooting = true;
		}
	}

	void BurstShot()
	{
		Shot();
	}

	void BurstShotUpwards()
	{
		ShotUpwards();
	}

	void BurstShotCrouching()
	{
		ShotCrouching();
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

	void ContinuousShotCrounching()
	{
		ShotCrouching();
	}

	// Shot Methods
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

	void ShotCrouching()
	{
		Instantiate(bulletCrouching, firePointCrouching.position, firePointCrouching.rotation);
	}

	// Camera Shake Methods
	void CameraShake(float shakeDuration, float shakeMagnitude)
	{
		StartCoroutine(cameraShaker.Shake(shakeDuration, shakeMagnitude));
	}

	// Gun System Methods
	void CycleThroughGuns()
	{
		if (gunId == 2)
		{
			gunId = 0;
		}
		else 
		{
			gunId++;
		}

		SwitchGun(gunId);
	}

    void SwitchGun(int gunId)
    {
		switch (gunId)
		{
			case 0:
				newGun = Gun.Pistol;
				selectedGunLabel.text = $"Gun: {newGun.ToString()}";
				break;
			case 1:
				newGun = Gun.Semi;
				selectedGunLabel.text = $"Gun: {newGun.ToString()}";
				break;
			case 2:
				newGun = Gun.Auto;
				selectedGunLabel.text = $"Gun: {newGun.ToString()}";
				break;
		}
    }
}
