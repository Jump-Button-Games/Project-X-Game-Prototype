using UnityEngine;

public class InsectShoot : MonoBehaviour
{
    [Header("Game Time Clock")]

    [Tooltip("Temporary variable to show the overall game time")]
    [SerializeField] float overallGameTime = 0;


    [Header("Shooting Controls")]

    [Tooltip("The time when the next shot will be fired. This is used in combination with Time.time + singleShotTime to set the next time the shot be fired")]
    [SerializeField] float nextShotTime = 0f;

    [Tooltip("The time is seconds before the next shot fires off")]
    [SerializeField] float shotTime = 5f;


    [Header("Bullet Controls")]

    [Tooltip("The octopus bullet prefab")]
    public GameObject insectBullet;

    [Tooltip("Fires upwards to the right at a 60 degrees angle")]
    public Transform insectFirePointUpRightMinus60;

    [Tooltip("Fires upwards to the left at a 60 degrees angle")]
    public Transform insectFirePointUpLeft60;

    [Tooltip("Fires upwards to the right at a 120 degrees angle")]
    public Transform insectFirePointDownRightMinus120;

    [Tooltip("Fires upwards to the left at a 120 degrees angle")]
    public Transform insectFirePointDownLeft120;

    void Update()
    {
        // Temp code to show how burst shots works
        overallGameTime = Time.time;

        if (InsectController.allowedShoot && Time.time > nextShotTime)
        {
            nextShotTime = Time.time + shotTime;
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(insectBullet, insectFirePointUpRightMinus60.position, insectFirePointUpRightMinus60.rotation);
        Instantiate(insectBullet, insectFirePointUpLeft60.position, insectFirePointUpLeft60.rotation);
        Instantiate(insectBullet, insectFirePointDownLeft120.position, insectFirePointDownLeft120.rotation);
        Instantiate(insectBullet, insectFirePointDownRightMinus120.position, insectFirePointDownRightMinus120.rotation);
    }
}
