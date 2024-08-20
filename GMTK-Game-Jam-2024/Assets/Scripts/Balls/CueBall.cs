using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueBall : Ball
{
    public static CueBall Instance;

    [SerializeField]
    private ParticleSystem readyBurst;
    private LineRenderer lineRenderer;

    // Unit vector pointing in direction the ball is aiming
    private Vector2 currentForceDir;
    // Magnitude of force to be applied to ball based on far player is pulling back 
    private float currentForceMagnitude;

    [SerializeField] private float maxPullBackDist = 4;
    [SerializeField] private float maxForceMagnitude = 25;

    private FMOD.Studio.EventInstance chargeUpSFX;

    bool isAiming = false;
    private bool isMoving;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != null)
        {
            Debug.LogError("Two cue balls!!");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        lineRenderer.enabled = false;
        currentForceDir = Vector2.zero;
        currentForceMagnitude = 0.0f;
        isAiming = false;

        chargeUpSFX = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/ChargeUp");
        chargeUpSFX.setParameterByName("ChargePercent", 0, true);
        chargeUpSFX.start();
    }

    private void Update()
    {
        if (isAiming && Input.GetMouseButtonDown(1))
        {
            isAiming = false;
            _animationController.EndAim();
            lineRenderer.enabled = false;
            trajectoryPathRenderer.HidePath();

            currentForceMagnitude = 0.0f;
            currentForceDir = Vector2.zero;

            chargeUpSFX.setParameterByName("ChargePercent", 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
	    _animationController.OnCollision();
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.gameState == GameManager.GameState.Playing && rb.velocity.magnitude < minVel)
        {
            isAiming = true;

            _animationController.StartAim();
            DrawLine(t.position, GetMousePos());
            lineRenderer.enabled = true;

            currentForceMagnitude = 0.0f;
            currentForceDir = Vector2.zero;

            trajectoryPathRenderer.DrawPath(currentForceDir, currentForceMagnitude);
            trajectoryPathRenderer.ShowPath();

            chargeUpSFX.setParameterByName("ChargePercent", 0);
        }
    }

    private void OnMouseDrag()
    {
        if (isAiming && GameManager.Instance.gameState == GameManager.GameState.Playing)
        {
            Vector3 mousePos = GetMousePos();
            Vector2 pullBackVector = Vector2.ClampMagnitude(t.position - mousePos, maxPullBackDist);

            currentForceMagnitude = Mathf.Lerp(0.0f, maxForceMagnitude, Mathf.InverseLerp(0.0f, maxPullBackDist, pullBackVector.magnitude));
            currentForceDir = pullBackVector.normalized;

            DrawLine(t.position, (Vector2)t.position - pullBackVector);
            trajectoryPathRenderer.DrawPath(currentForceDir, currentForceMagnitude);

            chargeUpSFX.setParameterByName("ChargePercent", currentForceMagnitude / maxForceMagnitude);
        }
    }

    private void OnMouseUp()
    {
        if (isAiming && GameManager.Instance.gameState == GameManager.GameState.Playing)
        {
            _animationController.EndAim();

            if (currentForceMagnitude > 3.0f)
            {
                _animationController.StartMovement();
                rb.velocity = currentForceDir * currentForceMagnitude;
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Shoot");
                GameManager.Instance.OnPlayerShot(this);
            }

            lineRenderer.enabled = false;
            trajectoryPathRenderer.HidePath();

            currentForceMagnitude = 0.0f;
            currentForceDir = Vector2.zero;
            
            chargeUpSFX.setParameterByName("ChargePercent", 0);

            isAiming = false;
        }
    }

    public void Celebrate()
    {
        _animationController.OnOtherObjectDeath();
    }

    // Draws line between two points on XY plane with this ball's line renderer, ignoring z
    private void DrawLine(Vector2 point1, Vector2 point2)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, point1);
        lineRenderer.SetPosition(1, point2);
    }

    protected override void ApplyManualDrag()
    {
        base.ApplyManualDrag();
        
        float curSpeed = rb.velocity.magnitude;
        if (curSpeed <= minVel)
        {
            if (isMoving)
            {
                isMoving = false;
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ReadyToShoot");
                readyBurst.Play();
                rb.velocity = Vector2.zero;
                _animationController.EndMovement();
            }

        }
        else
        {
            isMoving = true;
        }
    }

    private Vector3 GetMousePos()
    {
        return CameraManager.Instance.mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -CameraManager.Instance.mainCam.transform.position.z));
    }
}
