using System.Collections;
using UnityEngine;

public class PlayerFade : MonoBehaviour
{
    [Header("Player Fade Controls", order = 0)]


    [Header("Player Alpha Fade Configuration", order = 1)]

    [Tooltip("The higher the number the more opaque the sprite. Maximum value is 1 meaning the sprite is opaque and fully visible")]
    [SerializeField] float maximumAlpha = 1f;

    [Tooltip("The lower the number the more transparent the sprite. Minimum value is 0 meaning the sprite is transparent and invisible")]
    [SerializeField] float minimumAlpha = 0.7f;

    [Tooltip("The current visisbility of the sprite")]
    [SerializeField] float currentAplha;


    [Header("Player Alpha Time Configuration", order = 1)]

    [Tooltip("The rate at which the sprite becomes less or more visisble. The higher the value the more gradual the fade")]
    [SerializeField] float fadeDampener = 4f;

    [Tooltip("The maximum time the fade can last. Fading may stop before this time is reached.")]
    [SerializeField] float fadeDuration = 2f;

    [Tooltip("The time when the fading will stop")]
    [SerializeField] float fadeEndTime = 0f;


    [Header("Sprite Controls")]


    [Tooltip("The value to access the objects sprite renderer controls")]
    SpriteRenderer spriteRenderer;

    [Tooltip("The value of the sprites red, green and blue component")]
    const int rbgValue = 1;


    [Header("Game Time Clock")]

    [Tooltip("Temporary variable to show the overall game time")]
    [SerializeField] float overallGameTime = 0f;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    

    void Start()
    {
        currentAplha = maximumAlpha;
    }

    void Update()
    {
        overallGameTime = Time.time;
    }

    public IEnumerator FadeOut()
    {
        // Set the timer
        fadeEndTime = Time.time + fadeDuration;

        while (Time.time < fadeEndTime)
        {
            currentAplha -= Time.deltaTime / fadeDampener;

            if (currentAplha >= minimumAlpha)
            {
                spriteRenderer.color = new Color(rbgValue, rbgValue, rbgValue, currentAplha);
            }
           
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        // Set the timer
        fadeEndTime = Time.time + fadeDuration;

        while (Time.time < fadeEndTime)
        {
            currentAplha += Time.deltaTime / fadeDampener;

            if (currentAplha <= maximumAlpha)
            { 
                spriteRenderer.color = new Color(rbgValue, rbgValue, rbgValue, currentAplha);
            }

            yield return null;
        }
    }
}
