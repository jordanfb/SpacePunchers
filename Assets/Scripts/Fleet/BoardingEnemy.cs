using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardingEnemy : MonoBehaviour
{

    [SerializeField]
    private int health = 5;
    [SerializeField]
    private int sceneIndexToLoad = 2; // the boarding scene


    [Space]
    public Starmanager prototypeStarManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            FleetController ship = collision.gameObject.GetComponentInParent<FleetController>();
            if (ship != null)
            {
                // only do damage if it exists duh
                if (health <= 0 && ship.isMasterShip)
                {
                    OnShipDestroyed(); // only the master ship can board
                }

                // only the explosions do damage?
                if (!ship.isMasterShip)
                {
                    health--;
                    ship.DealDamage();
                }
            }
        }
    }

    private void OnShipDestroyed()
    {
        AsyncOperation unloadingOperation = SceneManager.UnloadSceneAsync(gameObject.scene.buildIndex);
        unloadingOperation.completed += (AsyncOperation a) => {
            Debug.Log("Unloaded scene");
            SceneManager.LoadScene(sceneIndexToLoad, LoadSceneMode.Additive);
            Debug.Log("Loaded new scene");
        };
    }
}
