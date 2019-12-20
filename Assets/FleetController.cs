using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class FleetController : MonoBehaviour
{
    public float fistSpeedForce = 100;
    public float rotationOffset = 90;
    public float inputRotationOffset = 45;

    public GameObject simpleBackFlames;
    [SerializeField]
    private float simpleBackFlamesTime = .1f;
    private float simpleBackFlamesTimer = 0;

    private Quaternion inputRotation;

    public List<GameObject> surroundingShips = new List<GameObject>();

    [SerializeField]
    private Rigidbody rb;

    private Player playerInput;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = ReInput.players.GetPlayer(0);
        UpdateInputRotation();
    }

    public void UpdateInputRotation()
    {
        inputRotation = Quaternion.Euler(0, inputRotationOffset, 0);
    }

    void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        Vector3 force = Vector3.forward * playerInput.GetAxis("FleetVertical") + Vector3.right * playerInput.GetAxis("FleetHorizontal");
        if (force.sqrMagnitude > 1)
        {
            force.Normalize();
        }
        force = inputRotation * force;
        force *= fistSpeedForce * Time.fixedDeltaTime;
        rb.AddForce(force);

        if (rb.velocity.sqrMagnitude > 0) {
            rb.rotation = Quaternion.Euler(0, Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg + rotationOffset, 0);
        }

        UpdateGrapics(force.sqrMagnitude > 0);
    }

    public void UpdateGrapics(bool visible)
    {
        simpleBackFlames.SetActive(visible);
        simpleBackFlamesTimer -= Time.deltaTime;
        if (simpleBackFlamesTimer <= 0)
        {
            simpleBackFlamesTimer = simpleBackFlamesTime;
            Vector3 scale = simpleBackFlames.transform.localScale;
            scale.y = -scale.y;
            simpleBackFlames.transform.localScale = scale;
        }
    }
}
