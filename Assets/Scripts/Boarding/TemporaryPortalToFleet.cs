using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TemporaryPortalToFleet : MonoBehaviour
{
    // Editable Variables
    [SerializeField]
    private int playerLayer = 10;
    [SerializeField]
    private int sceneToLoad = 1;



    // Runtime Variables
    private int thisSceneIndex;
    private AsyncOperation unloadingOperation;



    private void Start()
    {
        thisSceneIndex = gameObject.scene.buildIndex;
        unloadingOperation = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayer && unloadingOperation == null)
        {
            unloadingOperation = SceneManager.UnloadSceneAsync(thisSceneIndex);
            unloadingOperation.completed += (AsyncOperation a) => {
                Debug.Log("Unloaded scene");
                SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
                Debug.Log("Loaded new scene");
            };
        }
    }
}
