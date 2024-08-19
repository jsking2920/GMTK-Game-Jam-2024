using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] Transform AnimationTransform;
    float radius;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = AnimationTransform.GetComponent<Animator>();
        radius = transform.GetComponent<Renderer>().bounds.extents.magnitude * 0.6f;
        
        MoveAnimationTowardCamera();
    }

    private void Update()
    {
        // can move to a signal based call
        MoveAnimationTowardCamera();
    }

    // moves the face towards the camera 
    void MoveAnimationTowardCamera()
    {
        AnimationTransform.position = transform.position;
        AnimationTransform.LookAt(CameraManager.Instance.mainCam.transform);
        AnimationTransform.position += radius * AnimationTransform.forward;
    }

    [ContextMenu("hit")]
    public void OnCollision()
    {
        animator.SetTrigger("hit");
    }
    [ContextMenu("startMovement")]
    public void StartMovement()
    {
        animator.SetBool("inCharge", true);
    }
    [ContextMenu("endMovement")]
    public void EndMovement() 
    {
        animator.SetBool("inCharge", false);
    }
    [ContextMenu("death")]
    public void OnDeath()
    {
        animator.SetTrigger("dead");
    }
    [ContextMenu("celebrate")]
    public void OnOtherObjectDeath()
    {
        animator.SetTrigger("startDestruction");
    }

    [ContextMenu("startAim")]
    public void StartAim()
    {
        animator.SetTrigger("startAim");
    }
    [ContextMenu("endAim")]
    public void EndAim()
    {
        animator.SetTrigger("endAim");
    }

    public void startEat()
    {
        animator.SetTrigger("eat");
    }
    public void startSpit()
    {
        animator.SetTrigger("spit");
    }
}
