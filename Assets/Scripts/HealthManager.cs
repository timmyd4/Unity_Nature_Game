using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public GameObject firstHeart;
    public GameObject middleHeart;
    public GameObject lastHeart;

    public HurtFlash hurtFlash; // Reference to the HurtFlash script

    private int health = 3;

    public void LoseHeart()
    {
        health--;

        // Trigger the screen flash effect
        if (hurtFlash != null)
        {
            hurtFlash.FlashScreen(0.5f); // 0.5 seconds flash duration
        }

        if (health == 2)
        {
            firstHeart.SetActive(false);
        }
        else if (health == 1)
        {
            middleHeart.SetActive(false);
        }
        else if (health <= 0)
        {
            lastHeart.SetActive(false);
            Debug.Log("Player is out of health!");
        }
    }
}
