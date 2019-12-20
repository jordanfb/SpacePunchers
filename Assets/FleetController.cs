using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class FleetController : MonoBehaviour
{

    public GameObject fistPrefab;

    public int numShipsDestroyed = 0;
    public int numShipsPerFist = 10;

    public float attractForce = .5f;
    public float otherShipScalar = .5f;
    public Vector2 randomDragRange = new Vector2(-.1f, .1f);


    public float fistSpeedForce = 100;
    public float rotationOffset = 90;
    public float inputRotationOffset = 45;

    public GameObject simpleBackFlames;
    [SerializeField]
    private float simpleBackFlamesTime = .1f;
    private float simpleBackFlamesTimer = 0;

    private Quaternion inputRotation;

    public List<Rigidbody> surroundingShips = new List<Rigidbody>();

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

    public void DestroyedShip()
    {
        numShipsDestroyed++;
        if (numShipsDestroyed % numShipsPerFist == 0)
        {
            // then spawn a new fist!
            Vector3 offset = Random.onUnitSphere * 3;
            offset.y = 0;
            GameObject g = Instantiate(fistPrefab, transform.position + offset, transform.rotation);
            //Destroy(g.GetComponent<FleetController>()); // it's not the master so it doesn't get the fleet thing // wait that causes issues we still need to control it
            Rigidbody r = g.GetComponent<Rigidbody>();
            r.drag += Random.Range(randomDragRange.x, randomDragRange.y);
            surroundingShips.Add(r);
        }
    }

    void FixedUpdate()
    {
        UpdateMovement();
        UpdateSurroundingShips();
    }

    private void UpdateSurroundingShips()
    {
        foreach(Rigidbody r in surroundingShips)
        {
            Vector3 dpos = Vector3.zero;
            float d = Vector3.Distance(rb.position, r.position);
            if (d > 2.1f)
            {
                dpos += rb.position - r.position;
            }
            else if (d < 1.9f)
            {
                dpos -= rb.position - r.position; // move away from the player to give space
            }
            foreach (Rigidbody r2 in surroundingShips)
            {
                if (Vector3.Distance(r2.position, r.position) < 2)
                {
                    dpos -= (r2.position - r.position) * otherShipScalar;
                }
            }
            r.AddForce(dpos * attractForce);
        }
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
