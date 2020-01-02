using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallerEnemy : MonoBehaviour
{
    public float destroyedFreezeTime = .1f;
    public Starmanager prototypeStarManager;


    private Rigidbody rb;
    public bool isDestroyed = false;

    private FleetController fleet;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        fleet = FindObjectOfType<FleetController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if collided with player freezeframe!
        // probably should have a manager class freeze time and apply effects so we don't get weird overlaps but this works for now!
        if (other.gameObject.tag == "Player")
        {
            if (!isDestroyed)
            {
                StartCoroutine(FreezeFrame(destroyedFreezeTime, DestroySelf));
                isDestroyed = true;
            }
        }
    }

    public void DestroySelf()
    {
        // also create explosions somehow!

        // in this prototype we don't destroy ourselves we teleport elsewhere around the map so we always have enemies
        transform.localPosition = prototypeStarManager.GetNewPosition();
        transform.localRotation = prototypeStarManager.GetNewRotation();
        isDestroyed = false;
        fleet.DestroyedShip();
    }

    public IEnumerator FreezeFrame(float length, System.Action afterDelay)
    {
        if (Time.timeScale == 0)
        {
            // then someone else is already freezing time so don't do it yourself

            yield return new WaitUntil(() => Time.timeScale != 0);

            if (afterDelay != null)
            {
                afterDelay.Invoke();
            }
        }
        else
        {
            float timescale = Time.timeScale;
            float fixedtimestep = Time.fixedDeltaTime;
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;

            yield return new WaitForSecondsRealtime(length);

            Time.timeScale = timescale;
            Time.fixedDeltaTime = fixedtimestep;

            if (afterDelay != null)
            {
                afterDelay.Invoke();
            }
        }
    }
}
