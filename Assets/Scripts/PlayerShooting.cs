using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    public float rayDistance = 100f;
    public LayerMask targetLayer;
    public Transform shootOrigin;
    public float shootDelay = 0.5f;
    public ParticleSystem impactEffect;
    public ParticleSystem muzzleFlash;
    public GameObject[] cubePrefabs;
    public float projectileSpeed = 150f;
    public float cubeLifespan = 5f;
    public StaffAnimationController staffAnimationController;

    private bool canShoot = true;

    void Update()
    {
        // Trigger shooting when right mouse button is clicked
        if (Input.GetMouseButtonDown(1) && canShoot)
        {
            StartCoroutine(ShootWithAnimation());
        }
    }

    private IEnumerator ShootWithAnimation()
    {
        canShoot = false;

        if (staffAnimationController != null)
        {
            staffAnimationController.PlayAnimationAndParticles();
        }

        yield return new WaitForSeconds(0.3f);

        RaycastHit hit;
        Vector3 shootDirection = shootOrigin.forward;

        if (Physics.Raycast(shootOrigin.position, shootOrigin.forward, out hit, rayDistance, targetLayer))
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            shootDirection = (hit.point - shootOrigin.position).normalized;

            if (impactEffect != null)
            {
                ParticleSystem effect = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }

            AiBehavior ai = hit.collider.GetComponent<AiBehavior>();
            if (ai != null)
            {
                ai.TakeDamage(10);
            }
        }

        ShootRandomCube(shootDirection);

        yield return new WaitForSeconds(shootDelay - 0.1f);
        canShoot = true;
    }

    private void ShootRandomCube(Vector3 direction)
    {
        if (cubePrefabs.Length > 0)
        {
            GameObject selectedCube = cubePrefabs[Random.Range(0, cubePrefabs.Length)];
            GameObject cube = Instantiate(selectedCube, shootOrigin.position, shootOrigin.rotation);

            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }

            Destroy(cube, cubeLifespan);
        }
    }
}
