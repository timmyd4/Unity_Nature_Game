using UnityEngine;

public class BatSwing : MonoBehaviour
{
    private Animator animator;
    public int damage = 10;

    private bool isSwinging = false; // Optional: prevent damage outside swing

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            Debug.Log("Swing triggered!");
            animator.SetTrigger("Swing");
            isSwinging = true;

            // Optional: auto-stop swing after short delay
            Invoke(nameof(StopSwing), 0.3f); // adjust based on animation speed
        }
    }

    void StopSwing()
    {
        isSwinging = false;
    }

    private void OnTriggerEnter(Collider other)
{
    Debug.Log("Touched: " + other.name); // <--- See what you're hitting

    if (!isSwinging) return;

    if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    {
        Debug.Log("Enemy tag match!");

        AiBehavior enemy = other.GetComponent<AiBehavior>();
        if (enemy != null)
        {
            Debug.Log("Calling TakeDamage");
            enemy.TakeDamage(damage);
        }
        else
        {
            Debug.Log("AiBehavior NOT found on " + other.name);
        }
    }
}
}
