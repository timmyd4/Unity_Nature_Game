using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Assignables")]
    public Transform playerCam;
    public Transform orientation;
    private Collider playerCollider;
    public Rigidbody rb;

    [Space(10)]
    public LayerMask whatIsGround;

    [Header("MovementSettings")]
    public float sensitivity = 50f;
    public float moveSpeed = 2500f;
    public float walkSpeed = 20f;
    public float runSpeed = 10f;
    public bool grounded;
    public float maxSpeed = 10f; // Maximum allowed speed

    // Private Floats
    private float maxSlopeAngle = 35f;
    
    private float desiredX;
    private float xRotation;
    private float sensMultiplier = 1f;
    private float jumpCooldown = 0.25f;
    private float jumpForce = 250f;
    private float x;
    private float y;

    // Private bools
    private bool readyToJump;
    private bool jumping;
    private bool crouching;
    private bool cancellingGrounded;

    // Private Vector3's
    private Vector3 normalVector;

    // Instance
    public static PlayerMovement Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerCollider = GetComponent<Collider>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        readyToJump = true;
        normalVector = Vector3.up;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        MyInput();
        Look();
    }

    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.C);

        // Detect if the Shift key is held for running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            maxSpeed = 20f; // Increase speed cap
            moveSpeed = 3500f; // Increase movement force for faster walking
        }
        else
        {
            maxSpeed = 10f; // Default walking speed cap
            moveSpeed = 2500f; // Default movement force
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCrouch();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            StopCrouch();
        }
    }

    private void StartCrouch()
    {
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }

    private void StopCrouch()
    {
        transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement()
    {
        ApplyGroundStickiness();

        Vector2 mag = FindVelRelativeToLook();
        float num = mag.x;
        float num2 = mag.y;

        CounterMovement(x, y, mag);

        if (readyToJump && jumping)
        {
            Jump();
        }

        if (crouching) return;

        float speed = walkSpeed;
        if (x > 0f && num > speed) x = 0f;
        if (x < 0f && num < -speed) x = 0f;
        if (y > 0f && num2 > speed) y = 0f;
        if (y < 0f && num2 < -speed) y = 0f;

        PreventSliding();

        float multiplier = grounded ? 1f : 0.5f;
        rb.AddForce(orientation.forward * y * moveSpeed * Time.deltaTime * multiplier);
        rb.AddForce(orientation.right * x * moveSpeed * Time.deltaTime * multiplier);

        // Cap speed
        Vector3 velocity = rb.velocity;
        float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude; // Ignore vertical velocity

        if (horizontalSpeed > maxSpeed)
        {
            Vector3 cappedVelocity = velocity.normalized * maxSpeed;
            cappedVelocity.y = velocity.y; // Preserve vertical velocity for jumping
            rb.velocity = cappedVelocity;
        }
    }

    private void ApplyGroundStickiness()
    {
        if (grounded)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 10f); // Moderate downward force to avoid excessive stickiness
        }
    }

    private void PreventSliding()
    {
        if (!grounded) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, whatIsGround))
        {
            if (Vector3.Angle(hit.normal, Vector3.up) > maxSlopeAngle)
            {
                // Apply force opposite to the sliding direction
                rb.AddForce(-hit.normal * 50f);
            }
        }
    }



    private void Jump()
    {
        if (crouching) return;

        if (grounded && readyToJump)
        {
            readyToJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical velocity
            rb.AddForce(Vector3.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            Invoke("ResetJump", jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Look()
    {
        float num = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float num2 = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        desiredX = playerCam.transform.localRotation.eulerAngles.y + num;
        xRotation -= num2;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0f);
        orientation.transform.localRotation = Quaternion.Euler(0f, desiredX, 0f);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        float num = 0.16f;
        if (Math.Abs(mag.x) > 0.01f && Math.Abs(x) < 0.05f)
        {
            rb.AddForce(-mag.x * moveSpeed * Time.deltaTime * num * orientation.right);
        }
        if (Math.Abs(mag.y) > 0.01f && Math.Abs(y) < 0.05f)
        {
            rb.AddForce(-mag.y * moveSpeed * Time.deltaTime * num * orientation.forward);
        }
    }

    public Vector2 FindVelRelativeToLook()
    {
        float current = orientation.transform.eulerAngles.y;
        float target = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;
        float num = Mathf.DeltaAngle(current, target);
        float num2 = 90f - num;
        float magnitude = rb.velocity.magnitude;
        return new Vector2(
            y: magnitude * Mathf.Cos(num * Mathf.Deg2Rad),
            x: magnitude * Mathf.Cos(num2 * Mathf.Deg2Rad)
        );
    }

    private bool IsFloor(Vector3 v)
    {
        return Vector3.Angle(Vector3.up, v) < maxSlopeAngle;
    }

    private void OnCollisionStay(Collision other)
    {
        int layer = other.gameObject.layer;
        if ((int)whatIsGround != ((int)whatIsGround | (1 << layer))) return;

        foreach (var contact in other.contacts)
        {
            Vector3 normal = contact.normal;
            if (IsFloor(normal))
            {
                grounded = true;
                normalVector = normal;
                cancellingGrounded = false;
                CancelInvoke("StopGrounded");
            }
        }

        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke("StopGrounded", 0.2f);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }
}
