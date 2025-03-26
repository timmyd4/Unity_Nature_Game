using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider; // Reference to the Slider component
    public TextMeshProUGUI healthText; // Reference to the Text component
    public float maxHealth = 30f; // Default maximum health
    private float currentHealth; // Current health value

    public HurtFlash hurtFlash; // Optional: For flashing effect
    public Gradient healthGradient; // Gradient for the health bar color

    private void Start()
    {
        InitializeHealthBar(); // Initialize with max health
    }

    // Initializes the health bar
    public void InitializeHealthBar()
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        currentHealth = maxHealth;

        UpdateHealthBarText(); // Initialize the health text
        UpdateHealthBarColor(); // Initialize the health bar color
    }

    // Method to take damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Decrease health
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure it's within range

        slider.value = currentHealth; // Update the slider
        UpdateHealthBarText(); // Update the health text
        UpdateHealthBarColor(); // Update the color based on health

        if (hurtFlash != null)
        {
            hurtFlash.FlashScreen(0.5f); // Trigger screen flash
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Player is out of health!");
            // Add logic for player death
        }
    }

    // Updates the health bar color based on current health
    private void UpdateHealthBarColor()
    {
        // Calculate normalized health value (0 to 1)
        float normalizedHealth = currentHealth / maxHealth;

        // Get the color from the gradient
        Color healthColor = healthGradient.Evaluate(normalizedHealth);

        // Assign the color to the Slider's handle, background, or wherever necessary
        // Example: Change the handle color
        slider.fillRect.GetComponentInChildren<Image>().color = healthColor;
    }

    // Updates the health text to show current/max health
    private void UpdateHealthBarText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}"; // e.g., "25/30"
        }
    }
}
