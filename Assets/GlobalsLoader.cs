using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalsLoader : MonoBehaviour
{
    public int globalsSceneIndex = 0;

    [Space]
    public bool isGlobalsScene = false;
    public int aloneGlobalsSceneToLoad = 1; // if there's only one scene open and it's the globals scene what scene should be loaded?

    // Start is called before the first frame update
    void Start()
    {
        if (isGlobalsScene)
        {
            // you're the globals scene!
            if (SceneManager.sceneCount == 1)
            {
                // then load the additional helper scene so you aren't alone!
                SceneManager.LoadScene(aloneGlobalsSceneToLoad, LoadSceneMode.Additive);
            }
        }
        else
        {
            LoadGloablsIfUnloaded();

        }
    }

    public void LoadGloablsIfUnloaded()
    {
        if (!SceneManager.GetSceneByBuildIndex(globalsSceneIndex).isLoaded)
        {
            LoadGlobalsScene();
        }
    }

    private void LoadGlobalsScene()
    {
        SceneManager.LoadScene(globalsSceneIndex, LoadSceneMode.Additive);
        
    }
}
