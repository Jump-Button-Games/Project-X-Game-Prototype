using UnityEngine;

public class OctopusShoot : MonoBehaviour
{
    [Header("Game Time Clock")]

    [Tooltip("Temporary variable to show the overall game time")]
    [SerializeField] float overallGameTime = 0;


    [Header("Shooting Controls")]

    [Tooltip("The time when the next shot will be fired. This is used in combination with Time.time + singleShotTime to set the next time the shot be fired")]
    [SerializeField] float nextShotTime = 0f;

    [Tooltip("The time is seconds before the next shot fires off")]
    [SerializeField] float shotTime = 1.5f;


    [Header("Bullet Controls")]

    [Tooltip("The octopus bullet prefab")]
    public GameObject octopusBullet;

    [Tooltip("Point from where the center bullet fires. Fires in straight line away from the octupus head")]
    public Transform octopusFirePointCenter;

    [Tooltip("Point from where the left bullet fires. Fires in a 45 degree angle from the left of the octupus head")]
    public Transform octopusFirePointLeft45;

    [Tooltip("Point from where the right bullet fires. Fires in a 45 degree angle from the right of the octupus head")]
    public Transform octopusFirePointRight45;

    void Update()
    {
        // Temp code to show how burst shots works
        overallGameTime = Time.time;

        if (OctopusController.allowedShoot && Time.time > nextShotTime) 
        {
            nextShotTime = Time.time + shotTime;
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(octopusBullet, octopusFirePointCenter.position, octopusFirePointCenter.rotation);
        Instantiate(octopusBullet, octopusFirePointLeft45.position, octopusFirePointLeft45.rotation);
        Instantiate(octopusBullet, octopusFirePointRight45.position, octopusFirePointRight45.rotation);
    }
}
