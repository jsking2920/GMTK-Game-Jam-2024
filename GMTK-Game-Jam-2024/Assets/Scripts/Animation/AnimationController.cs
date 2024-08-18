using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Camera targetCamera;
    Transform AnimationTransform;
    float radius;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        targetCamera = Camera.main;
        AnimationTransform = transform.GetChild(0);
        animator = AnimationTransform.GetComponent<Animator>();
        radius = transform.GetComponent<Renderer>().bounds.extents.magnitude * 0.6f;
        
        moveAnimationTowardCamera();
    }

    private void Update()
    {
        // can move to a signal based call
        moveAnimationTowardCamera();
    }

    // moves the face towards the camera 
    void moveAnimationTowardCamera()
    {
        AnimationTransform.position = transform.position;
        AnimationTransform.LookAt(targetCamera.transform);
        AnimationTransform.position += radius * AnimationTransform.forward;
    }

    [ContextMenu("hit")]
    public void onCollision()
    {
        animator.SetTrigger("hit");
    }
    [ContextMenu("startMovement")]
    public void startMovement()
    {
        animator.SetBool("inCharge", true);
    }
    [ContextMenu("endMovement")]
    public void endMovement() 
    {
        animator.SetBool("inCharge", false);
    }
    [ContextMenu("death")]
    public void onDeath()
    {
        animator.SetTrigger("dead");
    }
    [ContextMenu("celebrate")]
    public void onOtherObjectDeath()
    {
        animator.SetTrigger("startDestruction");
    }
}
