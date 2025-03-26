using System.Collections;
using UnityEngine;
using TMPro;

public class DiamondCollectible : MonoBehaviour
{
    [Header("Diamond Settings")]
    public GameObject diamond;

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;

    [Header("Jump Settings")]
    public float jumpForce = 5f; // The force applied for the jump
    public float jumpInterval = 2f; // Time between jumps

    [Header("GUI Settings")]
    public TextMeshProUGUI diamondsCollectedGUI; // Assign in the Inspector for better performance

    private Rigidbody diamondRb;

    private bool isCollected = false; // Flag to ensure only one collection

    void Start()
    {
        // Cache the Rigidbody component
        diamondRb = diamond.GetComponent<Rigidbody>();

        if (diamondRb == null)
        {
            Debug.LogError("Rigidbody component is missing on the diamond object!");
            return;
        }

        // Ensure gravity is enabled
        diamondRb.useGravity = true;

        // Optionally freeze unwanted rotation axes
        diamondRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Find and cache the GUI reference if not assigned in the Inspector
        if (diamondsCollectedGUI == null)
        {
            diamondsCollectedGUI = GameObject.Find("DiamondsCollected")?.GetComponent<TextMeshProUGUI>();
            if (diamondsCollectedGUI == null)
            {
                Debug.LogWarning("DiamondsCollected GUI not found in the scene.");
            }
        }

        // Start the jumping coroutine
        StartCoroutine(JumpPeriodically());
    }

    void Update()
    {
        // Rotate the diamond for visual appeal
        diamond.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true; // Mark as collected

            // Update the score
            if (diamondsCollectedGUI != null)
            {
                int currentScore = int.Parse(diamondsCollectedGUI.text);
                currentScore++;
                diamondsCollectedGUI.text = currentScore.ToString();
            }

            // Stop jumping when collected
            StopCoroutine(JumpPeriodically());

            // Play a collection effect before destroying the diamond
            StartCoroutine(CollectDiamond());
        }
    }

    private IEnumerator CollectDiamond()
    {
        // Optional: Add a visual or sound effect here (e.g., shrinking)
        Vector3 originalScale = diamond.transform.localScale;

        float shrinkDuration = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < shrinkDuration)
        {
            float scale = Mathf.Lerp(1f, 0f, elapsedTime / shrinkDuration);
            diamond.transform.localScale = originalScale * scale;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy the diamond
        Destroy(diamond);
    }

    private IEnumerator JumpPeriodically()
    {
        while (!isCollected) // Keep jumping until the diamond is collected
        {
            // Apply an upward force to the Rigidbody
            diamondRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Wait for the next jump
            yield return new WaitForSeconds(jumpInterval);
        }
    }
}
