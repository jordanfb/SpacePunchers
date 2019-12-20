using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{

    public Transform focus;
    public Vector3 offset;
    public bool followRotation;
    public Quaternion offsetRotation;

    // Update is called once per frame
    void Update()
    {
        Align();
    }

    [ContextMenu("Align")]
    public void Align()
    {
        transform.position = focus.position + offset;
        if (followRotation)
        {
            transform.rotation = focus.rotation * offsetRotation;
        }
    }
}
