using System.Collections;
using UnityEngine;

[HelpURL("https://www.youtube.com/watch?v=9A9yj8KnM8c")]
public class CameraShaker : MonoBehaviour
{
    [Header("Random Number Generator Controls")]

    [Tooltip("The lowest number the random number generator can choose")]
    [SerializeField] float lowBoundNumber = -1f;

    [Tooltip("The highest number the random number generator can choose")]
    [SerializeField] float highBoundNumber = 1f;

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        { 
            float x = Random.Range(lowBoundNumber, highBoundNumber) * magnitude;
            float y = Random.Range(lowBoundNumber, highBoundNumber) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
