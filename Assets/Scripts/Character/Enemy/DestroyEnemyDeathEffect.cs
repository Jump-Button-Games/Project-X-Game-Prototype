using UnityEngine;

public class DestroyEnemyDeathEffect : MonoBehaviour
{
    void Update()
    {
        Destroy(gameObject,0.5f);
    }
}
