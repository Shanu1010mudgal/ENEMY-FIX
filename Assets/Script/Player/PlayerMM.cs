//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerMM : MonoBehaviour
//{
//    public float speed = 0;
//    public float jumpForce = 0;

//    float initialPlayerPosition;

//    bool isJump;

//    Rigidbody2D rb;

//    public Animator animator;

//    private void Start()
//    {
//        initialPlayerPosition = gameObject.transform.localPosition.x;
//        rb = GetComponent<Rigidbody2D>();
//        animator = GetComponent<Animator>();
//    }

//    void Update()
//    {
//        Vector2 Move = new Vector2(Input.GetAxis("Horizontal"), 0);

//        if (Input.GetKey(KeyCode.A))
//        {
//            transform.rotation = Quaternion.Euler(0, 180f, 0);
//            transform.Translate(-Move * speed * Time.deltaTime);
//            animator.SetFloat("Speed", Mathf.Abs(-Move.x));
//        }
//        else if (Input.GetKey(KeyCode.D))
//        {
//            transform.rotation = Quaternion.Euler(0, 0, 0);
//            transform.Translate(Move * speed * Time.deltaTime);
//            animator.SetFloat("Speed", Mathf.Abs(Move.x));
//        }
//        else
//        {
//            animator.SetFloat("Speed", 0);
//        }

//        if (Input.GetKeyDown(KeyCode.Space) && isJump)
//        {
//            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
//            animator.SetBool("IsJumping", true);
//        }
//    }

//    private void OnCollisionStay2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag("Ground"))
//        {
//            isJump = true;

//        }
//    }

//    private void OnCollisionExit2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag("Ground"))
//        {
//            isJump = false;
//            animator.SetBool("IsJumping", false);
//        }
//    }

//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMM : MonoBehaviour
{
    public float speed = 5f; // Speed of movement
    public float jumpForce = 10f; // Force for jumping
    public float airDrag = 0.5f; // Slow forward motion when airborne

    private int jumpCount = 0; // Track the number of jumps
    private const int maxJumps = 1; // Allow up to double jumps
    private float horizontalInput; // Store horizontal movement input
    private Rigidbody2D rb;
    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get horizontal input
        horizontalInput = Input.GetAxis("Horizontal");

        // Movement logic
        if (horizontalInput < 0) // Moving left
        {
            transform.rotation = Quaternion.Euler(0, 180f, 0);
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        }
        else if (horizontalInput > 0) // Moving right
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Add upward velocity
            jumpCount++;
            animator.SetBool("IsJumping", true);
        }
    }

    private void FixedUpdate()
    {
        // Apply horizontal movement in FixedUpdate for smooth physics
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        // Apply a slight forward force if in air and no horizontal input
        if (jumpCount > 0 && Mathf.Abs(horizontalInput) < 0.01f)
        {
            float forwardMotion = transform.localRotation.y == 0 ? airDrag : -airDrag;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x + forwardMotion, rb.linearVelocity.y);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0; // Reset jump count on landing
            animator.SetBool("IsJumping", false); // Reset jumping animation
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Do nothing specific here for double jump
        }
    }
}


