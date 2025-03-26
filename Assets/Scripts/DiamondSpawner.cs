using UnityEngine;

public class DiamondSpawner : MonoBehaviour
{
    public GameObject diamondPrefab; // Assign the diamond prefab in the inspector
    public Transform spawnPoint;    // Assign the specific spawn point (collider) in the inspector
    public AiBehavior associatedEnemy; // Assign the enemy instance associated with this spawner
    public float minHeight = 1f;    // Minimum height offset for spawning
    public float maxHeight = 3f;    // Maximum height offset for spawning

    private void OnEnable()
    {
        AiBehavior.OnEnemyDied += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        AiBehavior.OnEnemyDied -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath(AiBehavior enemy)
    {
        // Check if the enemy that died matches this spawner's associated enemy
        if (enemy == associatedEnemy && diamondPrefab != null && spawnPoint != null)
        {
            SpawnDiamond();
        }
    }

    private void SpawnDiamond()
    {
        // Add a random height offset to the spawn position
        Vector3 spawnPosition = spawnPoint.position;
        spawnPosition.y += Random.Range(minHeight, maxHeight);

        // Instantiate the diamond at the modified position
        GameObject diamondInstance = Instantiate(diamondPrefab, spawnPosition, Quaternion.identity);

        // Apply physics forces for the bounce effect
        Rigidbody diamondRb = diamondInstance.GetComponent<Rigidbody>();
        if (diamondRb != null)
        {
            // Apply an upward and outward force
            Vector3 forceDirection = new Vector3(
                Random.Range(-0.5f, 0.5f), // Slight horizontal variation
                1f,                       // Upward force
                Random.Range(-0.5f, 0.5f) // Slight depth variation
            ).normalized;

            float forceStrength = Random.Range(5f, 10f); // Adjust force strength for variability
            diamondRb.AddForce(forceDirection * forceStrength, ForceMode.Impulse);

            // Removed torque for spinning
        }
    }

}
