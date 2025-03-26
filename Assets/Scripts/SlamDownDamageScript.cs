using UnityEngine;

public class PlayerSlam : MonoBehaviour
{
    public float slamForce = 50f;
    public float minimumHeight = 8f;
    public int slamDamage = 20;
    public LayerMask enemyLayer;
    public float damageRadius = 5f;

    private Rigidbody rb;
    private bool isSlamming = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Check for slam input and height condition
        if (Input.GetKeyDown(KeyCode.V) && transform.position.y >= minimumHeight)
        {
            StartSlam();
        }
    }

    private void StartSlam()
    {
        if (isSlamming) return;

        isSlamming = true;
        rb.velocity = Vector3.zero; // Reset velocity
        rb.AddForce(Vector3.down * slamForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isSlamming)
        {
            PerformSlamDamage();
            isSlamming = false;
        }
    }

    private void PerformSlamDamage()
    {
        // Detect enemies within the damage radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius, enemyLayer);
        foreach (Collider hitCollider in hitColliders)
        {
            AiBehavior enemy = hitCollider.GetComponent<AiBehavior>();
            if (enemy != null)
            {
                enemy.TakeDamage(slamDamage);
            }
        }

        // Optional: Add particle effects or camera shake for impact feedback
        Debug.Log("Slam impact!");
    }

    private void OnDrawGizmos()
    {
        // Visualize damage radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
