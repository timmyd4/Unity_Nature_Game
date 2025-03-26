using UnityEngine;

public class WeaponLock : MonoBehaviour
{
    public Transform playerCamera; // Reference to the player's camera
    public Vector3 offset;         // Offset to position the weapon relative to the camera

    void Update()
    {
        // Lock position
        transform.position = playerCamera.position + playerCamera.TransformDirection(offset);

        // Lock rotation
        transform.rotation = playerCamera.rotation;
    }
}
