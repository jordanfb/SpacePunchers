using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class FleetController : MonoBehaviour
{
    public bool isMasterShip = false;

    public int numShipsDestroyed = 0;
    public int numShipsPerFist = 10;

    [Space]
    public float attractForce = .5f;
    public float otherShipScalar = .5f;
    public float extraRepulsionForce = 2f;
    public Vector2 distanceGoal = new Vector2(3f, 5f);

    [Space]
    public Vector2 randomDragRange = new Vector2(-.1f, .1f);
    public float randomRotationOffset = 10f;

    [Space]
    public float fistSpeedForce = 100;
    public float rotationOffset = 90;
    public float inputRotationOffset = 45;

    public GameObject simpleBackFlames;
    [SerializeField]
    private float simpleBackFlamesTime = .1f;
    private float simpleBackFlamesTimer = 0;

    private Quaternion inputRotation;
    private FleetController masterShip;
    public List<Rigidbody> surroundingShips = new List<Rigidbody>();

    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private GameObject graphics;


    private Player playerInput;
    private CameraFocus cameraFocus;

    // Start is called before the first frame update
    void Start()
    {
        cameraFocus = FindObjectOfType<CameraFocus>();
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
            CreateNewFist();
        }
    }

    public void DealDamage()
    {
        // when this ship destroys another ship it takes damage (unless it's the player's special ship)
        // if it takes too much damage (or for now, just one damage) it'll be destroyed
        if (!isMasterShip)
        {
            masterShip.ShipDestroyed(this); // remove us from the list of existing ships!
            Destroy(gameObject);
        }
    }

    [ContextMenu("Create New Fist")]
    public void CreateNewFist()
    {
        // then spawn a new fist!
        Vector3 offset = Random.onUnitSphere * 3;
        offset.y = 0;
        GameObject g = Instantiate(FleetGameManager.instance.fistPrefab, transform.position + offset, transform.rotation, FleetGameManager.instance.transform);
        FleetController otherShip = g.GetComponent<FleetController>();
        otherShip.inputRotationOffset += Random.Range(-randomRotationOffset, randomRotationOffset);
        Vector3 localPosGraphics = otherShip.graphics.transform.localPosition;
        localPosGraphics.y += Random.Range(-0.5f, .5f);
        otherShip.graphics.transform.localPosition += localPosGraphics;

        otherShip.masterShip = isMasterShip ? this : masterShip; // set the master ship based on who my master is

        //Destroy(otherShip); // it's not the master so it doesn't get the fleet thing // wait that causes issues we still need to control it
        Rigidbody r = g.GetComponent<Rigidbody>();
        r.drag += Random.Range(randomDragRange.x, randomDragRange.y);
        surroundingShips.Add(r);

        cameraFocus.goalDistance += 1; // slowly zoom out for more ships?
    }

    void FixedUpdate()
    {
        UpdateMovement();
        UpdateSurroundingShips();
    }

    private void UpdateSurroundingShips()
    {
        foreach (Rigidbody r in surroundingShips)
        {
            Vector3 dpos = Vector3.zero;
            float d = Vector3.Distance(rb.position, r.position);
            if (d > distanceGoal.y)
            {
                dpos += rb.position - r.position;
            }
            else if (d < distanceGoal.x)
            {
                dpos -= (rb.position - r.position) * extraRepulsionForce; // move away from the player to give space
            }
            foreach (Rigidbody r2 in surroundingShips)
            {
                if (r2 == r)
                {
                    continue; // skip yourself
                }
                if (Vector3.Distance(r2.position, r.position) < distanceGoal.x)
                {
                    dpos -= (r2.position - r.position) * otherShipScalar * extraRepulsionForce;
                }
                //else
                //{
                //    dpos += (r2.position - r.position) * otherShipScalar;
                //}
            }
            r.AddForce(dpos * attractForce);
        }
    }

    private void ShipDestroyed(FleetController s)
    {
        surroundingShips.Remove(s.rb);
    }

    private void UpdateMovement()
    {
        if (isMasterShip)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = Vector3.zero;
                rb.velocity = Vector3.zero;
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                CreateNewFist();
            }
        }

        Vector3 force = Vector3.forward * playerInput.GetAxis("FleetVertical") + Vector3.right * playerInput.GetAxis("FleetHorizontal");
        if (force.sqrMagnitude > 1)
        {
            force.Normalize();
        }
        force = inputRotation * force;
        force *= fistSpeedForce * Time.fixedDeltaTime;
        rb.AddForce(force);

        if (rb.velocity.sqrMagnitude > .1f) {
            // only rotate when you're moving at least somewhat to prevent twitchy rotation
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
