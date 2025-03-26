using UnityEngine;

public class BowAndArrow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform shootOrigin;
    public GameObject heldArrow;
    public float rayDistance = 100f;
    public LayerMask targetLayer;
    public float arrowSpeed = 150f;
    public float arrowLifespan = 5f;
    public ParticleSystem impactEffect;
    public ParticleSystem muzzleFlash;
    public float pullBackOffset = 0.4f;

    public float swayIntensity = 1f;
    public float swaySmoothness = 5f;

    private Vector3 heldArrowStartPos;
    private bool isPullingBack = false;
    private Vector3 swayOffset;

    void Start()
    {
        if (heldArrow != null)
        {
            heldArrowStartPos = heldArrow.transform.localPosition;
        }
    }

    void Update()
    {
        HandleInput();
        ApplySway();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // Changed to left-click
        {
            isPullingBack = true;
            PullBackArrow();
        }

        if (Input.GetMouseButtonUp(0) && isPullingBack) // Changed to left-click
        {
            ShootArrowWithLogic();
            isPullingBack = false;
            ResetHeldArrow();
        }
    }

    private void PullBackArrow()
    {
        if (heldArrow != null)
        {
            Vector3 pullBackPos = heldArrowStartPos - new Vector3(0, 0, pullBackOffset);
            heldArrow.transform.localPosition = pullBackPos;
        }
    }

    private void ResetHeldArrow()
    {
        if (heldArrow != null)
        {
            heldArrow.transform.localPosition = heldArrowStartPos;
        }
    }

    private void ApplySway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 targetSway = new Vector3(-mouseY * swayIntensity, mouseX * swayIntensity, 0);
        swayOffset = Vector3.Lerp(swayOffset, targetSway, Time.deltaTime * swaySmoothness);

        transform.localRotation = Quaternion.Euler(swayOffset);
    }

    private void ShootArrowWithLogic()
    {
        RaycastHit hit;
        Vector3 shootDirection = shootOrigin.forward;

        if (Physics.Raycast(shootOrigin.position, shootOrigin.forward, out hit, rayDistance, targetLayer))
        {
            shootDirection = (hit.point - shootOrigin.position).normalized;

            if (impactEffect != null)
            {
                ParticleSystem effect = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }

            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            AiBehavior ai = hit.collider.GetComponent<AiBehavior>();
            if (ai != null)
            {
                ai.TakeDamage(10);
            }
        }

        InstantiateAndShootArrow(shootDirection);
    }

    private void InstantiateAndShootArrow(Vector3 direction)
    {
        if (arrowPrefab != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, shootOrigin.position, shootOrigin.rotation);

            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * arrowSpeed;
            }

            Destroy(arrow, arrowLifespan);
        }
    }
}
