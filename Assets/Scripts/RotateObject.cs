using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Rotation speed on each axis
    public Vector3 rotationSpeed = new Vector3(0, 100, 0);

    void Update()
    {
        // Rotate the object
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
