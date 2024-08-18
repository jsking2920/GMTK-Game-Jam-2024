using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Camera targetCamera;
    Transform AnimationTransform;
    float radius;
    // Start is called before the first frame update
    void Start()
    {
        targetCamera = Camera.main;
        AnimationTransform = transform.GetChild(0);
        radius = transform.GetComponent<Renderer>().bounds.extents.magnitude * 0.6f;
        
        moveAnimationTowardCamera();
    }

    private void Update()
    {
        moveAnimationTowardCamera();
    }

    void moveAnimationTowardCamera()
    {
        AnimationTransform.position = transform.position;
        AnimationTransform.LookAt(targetCamera.transform);
        AnimationTransform.position += radius * AnimationTransform.forward;
    }
}
