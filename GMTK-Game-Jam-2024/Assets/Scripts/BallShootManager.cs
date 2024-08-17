using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class BallShootManager : MonoBehaviour
{
    #region Singleton class: BallShootManager
        public static BallShootManager Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    #endregion

    Camera cam;

    public Ball ball;
    public TrajectoryPath trajectory;

    [SerializeField] float pushForce = 4f;

    bool isDragging = false;
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 direction;
    Vector2 force;
    float distance;

    void Start()
    {
        cam = Camera.main;
        ball.DesactivateRb();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            OnDragStart();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            OnDragEnd();
        }
        if (isDragging)
        {
            OnDrag();
        }
    }

    void OnDragStart()
    {
        ball.DesactivateRb();
        startPoint = cam.ScreenToWorldPoint(Input.mousePosition);

        trajectory.ShowDots();
    }

    void OnDrag()
    {
        endPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        force = direction * distance * pushForce;

        //Debug.DrawLine(startPoint, endPoint);

        trajectory.UpdateDots(ball.pos, force);
    }

    void OnDragEnd()
    {
        ball.ActivateRb();
        ball.Push(force);
        trajectory.HideDots();
    }

}
