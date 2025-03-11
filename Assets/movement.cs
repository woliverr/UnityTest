using System.Collections;
using UnityEngine;

public class movement : MonoBehaviour
{
    private int remainingJumps = 0;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private BoxCollider2D box;
    public LayerMask groundLayer;

    private UnityEngine.Color myBlue = new UnityEngine.Color(52f / 255f, 161f / 255f, 235f / 255f);
    private UnityEngine.Color myRed = new UnityEngine.Color(200f / 255f, 50f / 255f, 50f / 255f);
    private UnityEngine.Color debug;

    // Dash Variables
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private bool canDash = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && remainingJumps > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10);
            remainingJumps--;
        }

        if (isGrounded())
        {
            remainingJumps = 1;
            canDash = true;
            debug = UnityEngine.Color.green;
        }
        else
        {
            debug = UnityEngine.Color.red;
        }
        Debug.DrawRay(box.bounds.center, Vector3.down, debug);

        // Handle Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }

        colorCorrection();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            lateralMovement();
        }
    }

    public bool isGrounded()
    {
        if (Physics2D.Raycast(box.bounds.center, Vector2.down, box.size[1] - 0.45f, groundLayer))
        {
            return true;
        }
        else { return false; }
    }

    private void lateralMovement()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocity = new Vector2(-5f, rb.linearVelocity.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocity = new Vector2(5f, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    // Dash Coroutine to handle dash mechanics
    private IEnumerator Dash()
    {
        isDashing = true;

        // Set the velocity to dash direction
        float dashDirection = Mathf.Sign(rb.linearVelocity.x);  // Dash direction based on current movement direction
        if (dashDirection == 0) dashDirection = 1;  // If no horizontal movement, default to right

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);  // Apply dash velocity in x-axis

        yield return new WaitForSeconds(dashDuration);  // Wait for the dash duration

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);  // Stop dashing (return to normal horizontal velocity)

        canDash = false;  // Disable dashing until the player touches the ground
        isDashing = false;  // Dash is over
    }

    void colorCorrection()
    {
        if (remainingJumps == 0)
        {
            spriteRenderer.color = myBlue;
        }
        else
        {
            spriteRenderer.color = myRed;
        }
    }
}