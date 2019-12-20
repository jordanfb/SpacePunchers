using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starmanager : MonoBehaviour
{
    public List<GameObject> starPrefabs = new List<GameObject>();
    public Vector2 size = new Vector2(100, 100);
    public int numStars = 1000;
    public float starYHeight = 5;

    private List<GameObject> createdObjects = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        CreateField();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyAllStars()
    {
        foreach(GameObject g in createdObjects)
        {
            Destroy(g);
        }
        createdObjects = new List<GameObject>();
    }

    [ContextMenu("Create stars")]
    public void CreateField()
    {
        DestroyAllStars();
        for (int i = 0; i < numStars; i++) {
            GameObject g = Instantiate(starPrefabs[Random.Range(0, starPrefabs.Count)], transform);
            g.transform.localPosition = new Vector3(Random.Range(-size.x, size.x), starYHeight, Random.Range(-size.y, size.y));
        }
    }
}
