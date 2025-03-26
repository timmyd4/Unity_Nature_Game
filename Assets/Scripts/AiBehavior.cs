using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AiBehavior : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Rigidbody rb;

    public ParticleSystem particle;
    public Material whiteFlashMaterial;
    private Material[] originalMaterials;
    private Renderer[] renderers;

    public int maxHealth = 30;
    private int currentHealth;

    private bool isDead = false;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    public delegate void EnemyDiedHandler(AiBehavior enemy);
    public static event EnemyDiedHandler OnEnemyDied;



    // Add a field for the death model
    public Mesh deathMesh;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogWarning("Player not found. Make sure the Player GameObject is tagged as 'Player'.");
            }
        }

        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // Get all renderers and store original materials
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Find all Rigidbody and Collider components in children
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        foreach (Rigidbody body in ragdollBodies)
        {
            if (body != rb)
            {
                body.isKinematic = true;
            }
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col != GetComponent<Collider>())
            {
                col.enabled = false;
            }
        }

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!isDead && agent != null && agent.enabled && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > 2.0f)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                agent.ResetPath();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead && collision.gameObject.CompareTag("Player"))
        {
            HealthBarScript healthBar = collision.gameObject.GetComponent<HealthBarScript>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(10);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Ensure HealthBarScript exists before calling its methods
        HealthBarScript healthBar = GetComponent<HealthBarScript>();
        if (healthBar != null)
        {
            healthBar.TakeDamage(damage);
            StartCoroutine(FlashWhite()); // Flash the enemy when damaged
            StartCoroutine(SquashAndStretchEffect());
        }
        else
        {
            StartCoroutine(FlashWhite()); // Still flash even without a health bar
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log($"{gameObject.name} has died!");
        gameObject.tag = "Untagged";

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (particle != null)
        {
            particle.Play();
        }

        // Change the model on death
        ChangeToDeathModel();

        // Change layer to DeadEnemy
        ChangeLayerRecursively(gameObject, LayerMask.NameToLayer("DeadEnemy"));

        EnableFlopEffect();

        // Notify listeners with the reference to this instance
        OnEnemyDied?.Invoke(this);
    }



    private void ChangeToDeathModel()
    {
        // Find the MeshFilter of the child object
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter != null && deathMesh != null)
        {
            meshFilter.mesh = deathMesh;
        }
        else
        {
            Debug.LogWarning("MeshFilter or deathMesh not found!");
        }
    }

    /// <summary>
    /// Recursively changes the layer of a GameObject and all its children.
    /// </summary>
    private void ChangeLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void EnableFlopEffect()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            Vector3 randomForce = new Vector3(
                Random.Range(-3f, 3f),
                Random.Range(10f, 15f),
                Random.Range(-3f, 3f)
            );
            rb.AddForce(randomForce, ForceMode.Impulse);

            Vector3 randomTorque = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f)
            );
            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }

        Collider mainCollider = GetComponent<Collider>();
        if (mainCollider != null)
        {
            mainCollider.enabled = false;
        }

        StartCoroutine(ReEnableCollidersAfterDelay());
        StartCoroutine(RemoveAfterFlop());
    }

    private IEnumerator ReEnableCollidersAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = true;
        }

        Collider mainCollider = GetComponent<Collider>();
        if (mainCollider != null)
        {
            mainCollider.enabled = true;
        }
    }

    private IEnumerator RemoveAfterFlop()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private IEnumerator FlashWhite()
    {
        if (isDead) yield break;

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = whiteFlashMaterial;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }

    private IEnumerator SquashAndStretchEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 squashedScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z * 1.2f);

        // Squash
        transform.localScale = squashedScale;
        yield return new WaitForSeconds(0.1f);

        // Stretch back
        transform.localScale = originalScale;
    }
}
