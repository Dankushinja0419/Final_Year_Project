using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    // Reference to your black image
    public SpriteRenderer blackImage;

    // Fading duration
    public float fadeDuration = 1f;

    // Fade out (make image opaque)
    public void FadeOut()
    {
        StartCoroutine(FadeCoroutine(1f));
    }

    // Fade in (make image transparent)
    public void FadeIn()
    {
        StartCoroutine(FadeCoroutine(0f));
    }

    private IEnumerator FadeCoroutine(float targetAlpha)
    {
        float elapsedTime = 0f;
        Color startColor = blackImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / fadeDuration;

            blackImage.color = Color.Lerp(startColor, targetColor, normalizedTime);

            yield return null;
        }

        // Ensure final alpha is exactly the target
        blackImage.color = targetColor;
    }
}