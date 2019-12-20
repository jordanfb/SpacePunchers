using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetGameManager : MonoBehaviour
{
    public static FleetGameManager instance;

    public GameObject fistPrefab; // so that we don't run into issues of cloning instead of correctly instantiating a different copy

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return; // instance already exists!
        }
    }
}
