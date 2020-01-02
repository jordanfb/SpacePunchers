using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class BoardingPirateControl : MonoBehaviour
{
    public float runSpeed = 10;
    public float jumpForce = 100;

    [SerializeField]
    private float defaultLinearDrag = 1;
    [SerializeField]
    private float noInputDrag = 2; // slow down the player faster when they aren't pressing anything


    [Space]
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private GameObject graphics;
    [SerializeField]
    private LayerMask environmentLayerMask;

    private Player playerInput;
    private CameraFocus cameraFocus;


    // JUMPING
    [Space]
    [Header("Jumping")]
    private bool isJumping = false; // from the button press to landing on the ground again
    private bool isJumpingForce = false;
    private bool isFalling = false;
    [SerializeField]
    private float maxJumpTime = 0.1f;
    private float jumpTimer = 0;
    [SerializeField]
    private float gravityScalarWhenJumping = 0;
    [SerializeField]
    private float gravityScalarWhenReleased = 3;
    [SerializeField]
    private float gravityScalarWhenPressed = 1;
    [SerializeField]
    private float defaultGravityScalar = 3;

    // Start is called before the first frame update
    void Start()
    {
        cameraFocus = FindObjectOfType<CameraFocus>();
        playerInput = ReInput.players.GetPlayer(0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAiming();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private bool CheckIfOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position - new Vector2(0f, 0.5f), Vector2.down, 0.2f, environmentLayerMask);
        // If the raycast hit something
        return hit;
    }

    private void UpdateMovement()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = Vector3.zero; // should really teleport to the starting location but this works for prototype testing
            rb.velocity = Vector2.zero;
        }

        // get horizontal input
        float xInput = playerInput.GetAxis("MoveHorizontal");

        bool onGround = CheckIfOnGround();

        // *************** MOVEMENT FRICTION ***************
        bool resetFriction = true;
        // figure out if you're on the floor or not
        if (xInput == 0)
        {
            if (onGround)
            {
                // then increase friction to slow down faster!
                resetFriction = false;
                rb.drag = noInputDrag;
            }
        }
        if (resetFriction)
        {
            rb.drag = defaultLinearDrag;
        }

        // *************** MOVEMENT FORCE ***************
        rb.AddForce(xInput * transform.right * runSpeed);


        if (rb.velocity.y < 0)
        {
            // then we're falling!
            isFalling = true;
        }


        // *************** JUMP ***************
        if (isJumping)
        {
            jumpTimer += Time.fixedDeltaTime;

            if (isJumpingForce)
            {
                Vector2 v = rb.velocity;
                v.y = jumpForce;
                rb.velocity = v;
            }

            // check if we should stop adding force
            if (isJumpingForce && (jumpTimer > maxJumpTime || playerInput.GetButtonUp("Jump")))
            {
                isJumpingForce = false;
                rb.gravityScale = gravityScalarWhenPressed;
                if (jumpTimer > maxJumpTime)
                {
                    Debug.Log("Hit max time for jump");
                }
            }
            // fall faster if they release the button
            if (playerInput.GetButtonUp("Jump"))
            {
                rb.gravityScale = gravityScalarWhenReleased;
            }
            // if landed then no longer jumping
            if (onGround)
            {
                isJumping = false;
                rb.gravityScale = defaultGravityScalar;
            }
        }
        else
        {
            if (playerInput.GetButtonDown("Jump"))
            {
                // then check if it's time to jump! I want the mario variable height jump for starters since that's solid
                // for now just default
                if (onGround)
                {
                    //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    Vector2 v = rb.velocity;
                    v.y = jumpForce;
                    rb.velocity = v;
                    isJumping = true;
                    isJumpingForce = true;
                    rb.gravityScale = gravityScalarWhenJumping;
                    jumpTimer = 0;

                    rb.drag = defaultLinearDrag; // make sure to have the lightweight drag
                }
            }
        }

        // "leftFist", "rightFist"
    }

    private void UpdateAiming()
    {
        Vector3 aim = Vector3.forward * playerInput.GetAxis("AimVertical") + Vector3.right * playerInput.GetAxis("AimHorizontal");
        if (aim.sqrMagnitude > 1)
        {
            aim.Normalize();
        }
    }
}
