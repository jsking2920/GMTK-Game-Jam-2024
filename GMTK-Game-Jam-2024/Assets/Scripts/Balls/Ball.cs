using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    protected Transform t;
    protected TrajectoryRenderer trajectoryPathRenderer;
    [HideInInspector] public Rigidbody2D rb;
 
    // Ball velocity gets clamped to zero below this threshold to shorten time to wait for next shot
    public float minVel = 0.3f; // Make cue ball minVel a little higher

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
        if (rb.velocity.magnitude > 0 && rb.velocity.magnitude < minVel)
        {
            rb.velocity = Vector2.zero;
            _animationController.EndMovement();
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
