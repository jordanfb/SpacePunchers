using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starmanager : MonoBehaviour
{
    public List<GameObject> starPrefabs = new List<GameObject>();
    public Vector2 size = new Vector2(100, 100);
    public int numStars = 1000;
    public float starYHeight = 5;

    public bool rotateAngle = false;

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

            if (g.GetComponent<SmallerEnemy>())
            {
                // for now I'm just abusing this spawning system to spawn the enemies as well so we have to stick this cludge in here.
                g.GetComponent<SmallerEnemy>().prototypeStarManager = this;
            }

            g.transform.localPosition = GetNewPosition();
            if (rotateAngle)
            {
                g.transform.localRotation = GetNewRotation();
            }
        }
    }

    public Vector3 GetNewPosition()
    {
        return new Vector3(Random.Range(-size.x, size.x), starYHeight, Random.Range(-size.y, size.y));
    }

    public Quaternion GetNewRotation()
    {
        return Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }
}
