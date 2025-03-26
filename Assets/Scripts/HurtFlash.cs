using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HurtFlash : MonoBehaviour
{
    private Image overlayImage; // Reference to the UI image
    private bool isFlashing = false;

    private void Awake()
    {
        overlayImage = GetComponent<Image>();
    }

    public void FlashScreen(float duration)
    {
        if (!isFlashing)
        {
            // Ensure the GameObject is active before starting the coroutine
            gameObject.SetActive(true);
            StartCoroutine(FlashCoroutine(duration));
        }
    }

    private IEnumerator FlashCoroutine(float duration)
    {
        isFlashing = true;

        // Set the initial red color with transparency
        overlayImage.color = new Color(1, 0, 0, 0.5f); // Half-transparent red

        // Fade out over time
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            overlayImage.color = new Color(1, 0, 0, Mathf.Lerp(0.5f, 0, elapsedTime / duration));
            yield return null;
        }

        // Disable the overlay after fading out
        overlayImage.color = new Color(1, 0, 0, 0); // Fully transparent
        gameObject.SetActive(false);
        isFlashing = false;
    }
}
