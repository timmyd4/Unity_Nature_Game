using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    public float floatAmplitude = 0.5f; // How high the object floats
    public float floatFrequency = 1f;  // How fast the object floats

    private Vector3 startPosition; // Starting position of the object

    void Start()
    {
        // Record the starting position
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate new position
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
