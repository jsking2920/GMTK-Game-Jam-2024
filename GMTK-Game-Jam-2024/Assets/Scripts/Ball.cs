using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Camera mainCam;

    private LineRenderer lineRenderer;
    private Transform t;
    private TrajectoryRenderer trajectoryPathRenderer;
    [HideInInspector] public Rigidbody2D rb;

    [SerializeField] private float maxPullBackDist = 4;
    [SerializeField] private float maxForceMagnitude = 25;
    
    // Unit vector pointing in direction the ball is aiming
    private Vector2 currentForceDir;
    // Magnitude of force to be applied to ball based on far player is pulling back 
    private float currentForceMagnitude;

    // Ball velocity gets clamped to zero below this threshold to shorten time to wait for next shot
    public float minVel = 0.3f;

    // Is this ball shootable by the player
    public bool isCueBall = true; 
    // Can this ball be shot right now
    private bool isShootable = false;

    public float radius = 0.5f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        t = transform;
        trajectoryPathRenderer = GetComponent<TrajectoryRenderer>();
    }

    private void Start()
    {
        lineRenderer.enabled = false;
        isShootable = false;
        currentForceDir = Vector2.zero;
        currentForceMagnitude = 0.0f;
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude < minVel)
        {
            rb.velocity = Vector2.zero;

            if (isCueBall && !isShootable)
            {
                // Cueball has come to rest, make it shootable again
                isShootable = true;
            }
        }
    }

    #region Event Callbacks
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnMouseDown()
    {
        if (isCueBall)
        {
            DrawLine(t.position, GetMousePos());
            lineRenderer.enabled = true;

            currentForceMagnitude = 0.0f;
            currentForceDir = Vector2.zero;

            trajectoryPathRenderer.DrawPath(currentForceDir, currentForceMagnitude);
            trajectoryPathRenderer.ShowPath();
        }
    }

    private void OnMouseDrag()
    {
        if (isCueBall)
        {
            Vector3 mousePos = GetMousePos();
            Vector2 pullBackVector = Vector2.ClampMagnitude(t.position - mousePos, maxPullBackDist);

            currentForceMagnitude = Mathf.Lerp(0.0f, maxForceMagnitude, Mathf.InverseLerp(0.0f, maxPullBackDist, pullBackVector.magnitude));
            currentForceDir = pullBackVector.normalized;

            DrawLine(t.position, (Vector2)t.position - pullBackVector);
            trajectoryPathRenderer.DrawPath(currentForceDir, currentForceMagnitude);
        }
    }

    private void OnMouseUp()
    {
        if (isCueBall)
        {
            lineRenderer.enabled = false;
            trajectoryPathRenderer.HidePath();

            rb.velocity = currentForceDir * currentForceMagnitude;
            
            //rb.velocity = Vector2.zero;
            //rb.AddForce(currentForceDir * currentForceMagnitude, ForceMode2D.Impulse);

            currentForceMagnitude = 0.0f;
            currentForceDir = Vector2.zero;

            // Can't shoot ball again until it comes to a rest
            isShootable = false;
            
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Shoot");
        }
    }
    #endregion

    #region Helpers
    private Vector3 GetMousePos()
    {
        return mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCam.transform.position.z));
    }

    // Draws line between two points on XY plane with this ball's line renderer, ignoring z
    private void DrawLine(Vector2 point1, Vector2 point2)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, point1);
        lineRenderer.SetPosition(1, point2);
    }
    #endregion
}
