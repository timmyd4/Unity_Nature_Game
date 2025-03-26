using UnityEngine;
using UnityEngine.AI;

public class EnemyActivation : MonoBehaviour
{
    public AiBehavior aiBehavior; // Reference to the AiBehavior script
    public Collider activationCollider; // Optional: Reference to the activation collider
    public NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component
    public Canvas canvas; // Reference to the Canvas
    public MeshFilter meshFilter; // Reference to the MeshFilter
    public Mesh activatedMesh; // The new mesh to display when activated

    private bool isActivated = false;

    private void Start()
    {
        Debug.Log("EnemyActivation script initialized!");

        // Validate references
        if (aiBehavior == null)
        {
            aiBehavior = GetComponent<AiBehavior>();
            if (aiBehavior == null)
                Debug.LogError("AiBehavior component not found on this GameObject!");
        }

        if (activationCollider == null)
        {
            activationCollider = GetComponent<Collider>();
            if (activationCollider == null)
                Debug.LogError("Collider component not found on this GameObject!");
        }

        if (activationCollider != null)
        {
            activationCollider.isTrigger = true;
            Debug.Log("Activation collider is set and ready!");
        }

        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
                Debug.LogError("NavMeshAgent component not found on this GameObject!");
        }

        if (aiBehavior != null)
        {
            aiBehavior.enabled = false;
            Debug.Log("AiBehavior has been disabled.");
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true; // Ensure the enemy starts stationary
            navMeshAgent.ResetPath();
        }

        if (canvas != null)
        {
            canvas.enabled = false; // Ensure the canvas is disabled initially
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger detected with {other.gameObject.name}");

        if (!isActivated && other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the activation area!");
            ActivateEnemy();
        }
    }

    private void ActivateEnemy()
    {
        if (aiBehavior != null)
        {
            aiBehavior.enabled = true; // Enable AI behavior
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false; // Allow movement
        }

        if (meshFilter != null && activatedMesh != null)
        {
            meshFilter.mesh = activatedMesh; // Change the mesh
            Debug.Log("Mesh has been updated!");
        }

        if (canvas != null)
        {
            canvas.enabled = true; // Activate the canvas
            Debug.Log("Canvas has been activated!");
        }

        isActivated = true; // Prevent reactivation
    }

    public void DeactivateEnemy()
    {
        if (aiBehavior != null)
        {
            aiBehavior.enabled = false; // Disable AI behavior
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true; // Stop movement
            navMeshAgent.ResetPath();
        }

        if (canvas != null)
        {
            canvas.enabled = false; // Deactivate the canvas
        }

        isActivated = false;
    }
}
