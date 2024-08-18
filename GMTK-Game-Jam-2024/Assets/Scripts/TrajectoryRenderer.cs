using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.UI.Image;

public class TrajectoryRenderer : MonoBehaviour
{
    private Ball ball;
    
    [SerializeField] private GameObject dotsParent;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject collisionCircleSpritePrefab;

    private float simulationStep = 0.02f;

    private Transform[] dotsList;
    private int maxDots = 50; // simulationStep * max dots == max simulated time
    private float dotRadius = 0.25f; // Used to prevent dot's from being drawn overlapping

    private GameObject collisionSprite;

    private void Start()
    {
        ball = GetComponent<Ball>();
        PrepareDots();
        HidePath();
    }

    private void PrepareDots()
    {
        dotsList = new Transform[maxDots];

        for (int i = 0; i < maxDots; i++)
        {
            dotsList[i] = Instantiate(dotPrefab, dotsParent.transform).transform;
            dotsList[i].gameObject.SetActive(false);
        }

        collisionSprite = Instantiate(collisionCircleSpritePrefab, dotsParent.transform);
        collisionSprite.gameObject.SetActive(false);
    }

    public void DrawPath(Vector2 forceDir, float initialVel)
    {
        Vector2 curSimulatedVel = initialVel * forceDir;
        Vector2 curSimulatedPos = ball.transform.position;
        int curDotIndex = 0;

        Vector2 lastDotDrawnPos = curSimulatedPos;
        Vector2 nextSimulatedPos;

        while (curSimulatedVel.magnitude > ball.minVel && curDotIndex < dotsList.Length)
        {
            nextSimulatedPos = curSimulatedPos + (curSimulatedVel * simulationStep);

            // Check for collision
            bool foundCollision = false;
            Collider2D[] results = Physics2D.OverlapCircleAll(nextSimulatedPos, ball.radius); // Make this check a bit fuzzier to account for simulation step

            foreach (Collider2D collisionResult in results)
            {
                if (collisionResult != null && collisionResult.gameObject != ball.gameObject)
                {
                    Ball otherBall = collisionResult.GetComponent<Ball>();

                    if (otherBall != null)
                    {
                        // Find point of collision between the two circles. Draw circle at that point of contact
                        Tuple<bool, Vector2> doCirclesCollide = GetPositionAtTimeOfCollision(otherBall, forceDir * initialVel);

                        if (doCirclesCollide.Item1)
                        {
                            collisionSprite.transform.position = doCirclesCollide.Item2;
                            collisionSprite.transform.localScale = new Vector3(ball.radius * 2.0f, ball.radius * 2.0f, 1.0f);
                            collisionSprite.SetActive(true);
                            foundCollision = true;
                            // TODO: Add line for otherBall
                        }
                    }
                    else
                    {
                        // TODO: Handle collisions with non-balls better
                        collisionSprite.transform.position = nextSimulatedPos;
                        collisionSprite.transform.localScale = new Vector3(ball.radius * 2.0f, ball.radius * 2.0f, 1.0f);
                        collisionSprite.SetActive(true);
                        foundCollision = true;
                    }
                    // TODO: update velocity after collision and simulate only a little further

                    // Only consider the first collision
                    if (foundCollision)
                        break;
                }
            }

            if (foundCollision)
            {
                // Turn off the last dot before the collision circle if it overlaps with the collision circle
                if (curDotIndex > 0 && (lastDotDrawnPos - nextSimulatedPos).magnitude < ball.radius)
                    dotsList[curDotIndex - 1].gameObject.SetActive(false);
                break;
            }
            else
            {
                collisionSprite.SetActive(false);
            }

            // If we've traveled far enough for a dot, draw a new one
            if ((curSimulatedPos - lastDotDrawnPos).magnitude > dotRadius)
            {
                dotsList[curDotIndex].position = nextSimulatedPos;
                dotsList[curDotIndex].gameObject.SetActive(true);
                curDotIndex += 1;
                lastDotDrawnPos = nextSimulatedPos;
            }

            // Apply Drag
            curSimulatedVel = curSimulatedVel * (1 - simulationStep * ball.rb.drag);
            curSimulatedPos = nextSimulatedPos;
        }

        // Make sure any unneeded pooled dots are turned off
        for (int i = curDotIndex; i < dotsList.Length; i++)
        {
            dotsList[i].gameObject.SetActive(false);
        }
    }

    public void ShowPath()
    {
        dotsParent.SetActive(true);
    }

    public void HidePath()
    {
        dotsParent.SetActive(false);
    }

    // Based on: https://ericleong.me/research/circle-circle/ and https://stackoverflow.com/questions/51905268/how-to-find-closest-point-on-line
    private Tuple<bool, Vector2> GetPositionAtTimeOfCollision(Ball otherBall, Vector2 vel)
    {
        Vector2 lhs = otherBall.transform.position - ball.transform.position;
        Vector2 closestPointOnLine = (Vector2)ball.transform.position + vel.normalized * Vector2.Dot(lhs, vel.normalized);

        float closestDistSq = Mathf.Pow(otherBall.transform.position.x - closestPointOnLine.x, 2) + Mathf.Pow(otherBall.transform.position.y - closestPointOnLine.y, 2);

        if (closestDistSq <= Mathf.Pow(ball.radius + otherBall.radius, 2))
        {
            float backdist = Mathf.Sqrt(Mathf.Pow(ball.radius + otherBall.radius, 2) - closestDistSq);
            float movementVectorLength = Mathf.Sqrt(Mathf.Pow(vel.x, 2) + Mathf.Pow(vel.y, 2));
            float x = closestPointOnLine.x - backdist * (vel.x / movementVectorLength);
            float y = closestPointOnLine.y - backdist * (vel.y / movementVectorLength);

            return new Tuple<bool, Vector2>(true, new Vector2(x, y));
        }

        return new Tuple<bool, Vector2>(false, Vector2.zero);
    }
}
