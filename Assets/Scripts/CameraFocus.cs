using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{

    public Transform focus;
    public float distance = 10;
    public float goalDistance = 10;
    public float lerpingSpeed = .1f;

    // Update is called once per frame
    void Update()
    {
        Align();
        UpdateDistance();
    }

    public void UpdateDistance()
    {
        float delta = goalDistance - distance;
        delta *= lerpingSpeed;
        distance += delta;
    }

    [ContextMenu("Align")]
    public void Align()
    {
        transform.position = focus.position + transform.forward * -distance;
    }
}
