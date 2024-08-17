using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueBall : MonoBehaviour
{
    [SerializeField] private Camera mainCam;

    [SerializeField] private float maxPullBackDist;
    [SerializeField] private float maxForceMagnitude;

    private LineRenderer lineRenderer;
    private Transform t;
    private Rigidbody2D rb;

    // Normalized Vector pointing in direction the ball is aiming
    private Vector2 currentForceDir;
    private float currentForceMagnitude;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        t = transform;
    }

    private void Start()
    {
        lineRenderer.enabled = false;
    }

    private void OnMouseDown()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, t.position);
        lineRenderer.SetPosition(1, GetMousePos());
        lineRenderer.enabled = true;

        currentForceMagnitude = 0.0f;
        currentForceDir = Vector2.zero;
    }

    private void OnMouseDrag()
    {
        Vector3 mousePos = GetMousePos();

        Vector2 pullBackVector = Vector2.ClampMagnitude(t.position - mousePos, maxPullBackDist);

        lineRenderer.SetPosition(0, t.position);
        lineRenderer.SetPosition(1, (Vector2)t.position - pullBackVector);

        currentForceMagnitude = Mathf.Lerp(0.0f, maxForceMagnitude, Mathf.InverseLerp(0.0f, maxPullBackDist, pullBackVector.magnitude));
        currentForceDir = pullBackVector.normalized;
    }

    private void OnMouseUp()
    {
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;

        rb.velocity = Vector2.zero;//currentForceDir * currentForceMagnitude;
        rb.AddForce(currentForceDir * currentForceMagnitude, ForceMode2D.Impulse);

        currentForceMagnitude = 0.0f;
        currentForceDir = Vector2.zero;
    }

    private Vector3 GetMousePos()
    {
        return mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCam.transform.position.z));
    }
}
