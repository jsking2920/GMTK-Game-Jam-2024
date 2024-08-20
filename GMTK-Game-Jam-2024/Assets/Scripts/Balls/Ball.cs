using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    protected Transform t;
    protected TrajectoryRenderer trajectoryPathRenderer;
    [HideInInspector] public Rigidbody2D rb;
 
    // Ball velocity gets clamped to zero below this threshold to shorten time to wait for next shot
    public float minVel = 0.5f;
    // Ball velocity drops sharply below this threshold
    [HideInInspector] public float highVelThreshold = 6.0f;
    // Linear drag applied at normal speed
    [HideInInspector] public float standardDrag = 0.4f;

    public float radius = 0.5f;

    protected AnimationController _animationController;

    protected virtual void Awake()
    {
        _animationController = GetComponent<AnimationController>();
        rb = GetComponent<Rigidbody2D>();
        t = transform;
        trajectoryPathRenderer = GetComponent<TrajectoryRenderer>();
    }

    void FixedUpdate()
    {
        ApplyManualDrag();
    }

    protected virtual void ApplyManualDrag()
    {
        float curSpeed = rb.velocity.magnitude;

        // normal 0.4 linear drag at speed
        if (curSpeed > highVelThreshold)
        {
            rb.velocity = rb.velocity * (1.0f - Time.fixedDeltaTime * standardDrag);
        }
        // Increase drag as speed slows to make ball stop quicker
        else if (curSpeed <= highVelThreshold && curSpeed > minVel)
        {
            float lerpedDrag = Mathf.Lerp(standardDrag, standardDrag * 6.0f, Mathf.InverseLerp(highVelThreshold, minVel, curSpeed));
            rb.velocity = rb.velocity * (1.0f - Time.fixedDeltaTime * lerpedDrag);
        }
        // Clamp to zero
        else if (curSpeed <= minVel)
        {
            rb.velocity = Vector2.zero;
            //_animationController.EndMovement();
        }
    }

    public void Die()
    {
        _animationController.OnDeath();
        CueBall.Instance.Celebrate();
    }   

    #region Event Callbacks
    private void OnCollisionEnter2D(Collision2D collision)
    {
	    _animationController.OnCollision();
    }
    #endregion
}
