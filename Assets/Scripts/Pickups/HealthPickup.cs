using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	[Tooltip("The amount by which player health will increase")]
	[SerializeField] int healthIncrease = 20;

	void OnTriggerEnter2D(Collider2D collideInfo)
	{
		PlayerHealth player = collideInfo.GetComponent<PlayerHealth>();

		if (player != null)
		{
			player.IncreaseHealth(healthIncrease);
			Destroy(gameObject);
		}
	}
}
