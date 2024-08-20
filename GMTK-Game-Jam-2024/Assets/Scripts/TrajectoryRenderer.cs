using System;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryRenderer : MonoBehaviour
{
    private CueBall ball;
    
    [SerializeField] private GameObject dotsParent;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject collisionCircleSpritePrefab;

    private float simulationStep = 0.015f;

    private Transform[] dotsList;
    private int maxDots = 50; // simulationStep * max dots == max simulated time
    private Transform[] secondaryDotsList;
    private int maxDotsDrawnAfterCollision = 3;
    
    private float dotRadius = 0.25f; // Used to prevent dot's from being drawn overlapping
    
    private GameObject collisionSprite;

    private void Start()
    {
        ball = GetComponent<CueBall>();
        PrepareDots();
        HidePath();
    }

    private void PrepareDots()
    {
        dotsList = new Transform[maxDots];
        secondaryDotsList = new Transform[maxDotsDrawnAfterCollision];

        for (int i = 0; i < maxDots; i++)
        {
            dotsList[i] = Instantiate(dotPrefab, dotsParent.transform).transform;
            dotsList[i].gameObject.SetActive(false);
        }
        for (int j = 0; j < maxDotsDrawnAfterCollision; j++)
        {
            secondaryDotsList[j] = Instantiate(dotPrefab, dotsParent.transform).transform;
            secondaryDotsList[j].gameObject.SetActive(false);
        }

        collisionSprite = Instantiate(collisionCircleSpritePrefab, dotsParent.transform);
        collisionSprite.gameObject.SetActive(false);
    }

    public void DrawPath(Vector2 forceDir, float initialVel)
    {
        collisionSprite.SetActive(false);
        DisableSecondaryDots();

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
            Collider2D[] results = Physics2D.OverlapCircleAll(nextSimulatedPos, ball.radius); // TODO: Make this check a bit fuzzier to account for simulation step

            foreach (Collider2D collisionResult in results)
            {
                if (collisionResult != null && !collisionResult.isTrigger && collisionResult.gameObject != ball.gameObject)
                {
                    Ball otherBall = collisionResult.GetComponent<Ball>();

                    if (otherBall != null)
                    {
                        // Find point of collision between the two circles. Draw circle at that point of contact
                        //Vector2[] collisionPositionAndVel = GetPositionAtTimeOfCollision(otherBall, forceDir * initialVel, curSimulatedVel);
                        Vector2[] collisionInfo = GetCollisionInfo(otherBall, forceDir * initialVel);

                        if (collisionInfo != null)
                        {
                            collisionSprite.transform.position = collisionInfo[0];
                            collisionSprite.transform.localScale = new Vector3(ball.radius * 2.0f, ball.radius * 2.0f, 1.0f);
                            collisionSprite.SetActive(true);
                            foundCollision = true;

                            // Draw short path after collision
                            //DrawSecondaryPath(collisionPositionAndVel[0], collisionPositionAndVel[1]);
                            // Draw path for other ball
                            DrawSecondaryPath(otherBall, otherBall.transform.position, collisionInfo[1] * curSimulatedVel.magnitude);
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
                DisableSecondaryDots();
            }

            // If we've traveled far enough for a dot, draw a new one
            if ((curSimulatedPos - lastDotDrawnPos).magnitude > dotRadius && (curSimulatedPos - (Vector2)ball.transform.position).magnitude > ball.radius)
            {
                dotsList[curDotIndex].position = nextSimulatedPos;
                dotsList[curDotIndex].gameObject.SetActive(true);
                curDotIndex += 1;
                lastDotDrawnPos = nextSimulatedPos;
            }

            // Apply Drag
            if (curSimulatedVel.magnitude > ball.highVelThreshold)
            {
                curSimulatedVel = curSimulatedVel * (1 - simulationStep * ball.standardDrag);
            }
            // Increase drag as speed slows to make ball stop quicker
            else if (curSimulatedVel.magnitude <= ball.highVelThreshold && curSimulatedVel.magnitude > ball.minVel)
            {
                float lerpedDrag = Mathf.Lerp(ball.standardDrag, ball.standardDrag * 6.0f, Mathf.InverseLerp(ball.highVelThreshold, ball.minVel, curSimulatedVel.magnitude));
                curSimulatedVel = curSimulatedVel * (1.0f - simulationStep * lerpedDrag);
            }
            curSimulatedPos = nextSimulatedPos;
        }

        // Make sure any unneeded pooled dots are turned off
        for (int i = curDotIndex; i < dotsList.Length; i++)
        {
            dotsList[i].gameObject.SetActive(false);
        }
    }

    // Draws short trajectory path after collision, ignores any further collision
    public void DrawSecondaryPath(Ball b, Vector2 startPos, Vector2 initialVel)
    {
        Vector2 curSimulatedVel = initialVel;
        Vector2 curSimulatedPos = startPos;
        int curDotIndex = 0;

        Vector2 lastDotDrawnPos = curSimulatedPos;
        Vector2 nextSimulatedPos;

        while (curSimulatedVel.magnitude > b.minVel && curDotIndex < dotsList.Length && curDotIndex < maxDotsDrawnAfterCollision)
        {
            nextSimulatedPos = curSimulatedPos + (curSimulatedVel * simulationStep);

            // If we've traveled far enough for a dot, draw a new one
            if ((curSimulatedPos - lastDotDrawnPos).magnitude > dotRadius && (curSimulatedPos - startPos).magnitude > b.radius + dotRadius)
            {
                secondaryDotsList[curDotIndex].position = nextSimulatedPos;
                secondaryDotsList[curDotIndex].gameObject.SetActive(true);
                curDotIndex += 1;
                lastDotDrawnPos = nextSimulatedPos;
            }

            // Apply Drag
            curSimulatedVel = curSimulatedVel * (1 - simulationStep * b.rb.drag);
            curSimulatedPos = nextSimulatedPos;
        }

        // Make sure any unneeded pooled dots are turned off
        for (int i = curDotIndex; i < secondaryDotsList.Length; i++)
        {
            secondaryDotsList[i].gameObject.SetActive(false);
        }
    }

    private void DisableSecondaryDots()
    {
        for (int i = 0; i < secondaryDotsList.Length; i++)
        {
            secondaryDotsList[i].gameObject.SetActive(false);
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

    // Tried to math out what the resulting velocity would be but couldn't get it to work reliably
    // Used these sources:
    // https://www.euclideanspace.com/physics/dynamics/collision/twod/index.htm#code
    // https://www.youtube.com/watch?v=guWIF87CmBg
    // https://www.real-world-physics-problems.com/physics-of-billiards.html
    //
    // Instead should do this:
    // https://www.youtube.com/watch?v=p8e4Kpl9b28
    // https://ximer.itch.io/peggle-unity
    //
    // Based on: https://ericleong.me/research/circle-circle/ and https://stackoverflow.com/questions/51905268/how-to-find-closest-point-on-line
    /*
    private Vector2[] GetPositionAtTimeOfCollision(Ball otherBall, Vector2 initialVel, Vector2 velAroundTimeOfImpact)
    {
        Vector2 lhs = otherBall.transform.position - ball.transform.position;
        Vector2 closestPointOnLine = (Vector2)ball.transform.position + initialVel.normalized * Vector2.Dot(lhs, initialVel.normalized);

        float closestDistSq = Mathf.Pow(otherBall.transform.position.x - closestPointOnLine.x, 2) + Mathf.Pow(otherBall.transform.position.y - closestPointOnLine.y, 2);

        if (closestDistSq <= Mathf.Pow(ball.radius + otherBall.radius, 2))
        {
            float backdist = Mathf.Sqrt(Mathf.Pow(ball.radius + otherBall.radius, 2) - closestDistSq);
            float movementVectorLength = Mathf.Sqrt(Mathf.Pow(initialVel.x, 2) + Mathf.Pow(initialVel.y, 2));
            float x = closestPointOnLine.x - backdist * (initialVel.x / movementVectorLength);
            float y = closestPointOnLine.y - backdist * (initialVel.y / movementVectorLength);

            // Figure out resulting velocity (assumes otherBall is static for simplicity, good enough)
            Vector2 norm = ((Vector2)otherBall.transform.position - new Vector2(x, y)).normalized;
            //float p = (2 * Vector2.Dot(velAroundTimeOfImpact, norm)) / (ball.rb.mass + otherBall.rb.mass);
            //float vx = velAroundTimeOfImpact.x - p * ball.rb.mass * norm.x - p * otherBall.rb.mass * norm.x;
            //float vy = velAroundTimeOfImpact.y - p * ball.rb.mass * norm.y - p * otherBall.rb.mass * norm.y;
            //Vector2 v = velAroundTimeOfImpact - (p * ((ball.rb.mass * norm) - (otherBall.rb.mass * norm)));
            Vector2 v = 2 * ((ball.rb.mass * otherBall.rb.mass) / (ball.rb.mass + otherBall.rb.mass)) 
                * Vector2.Dot(velAroundTimeOfImpact - Vector2.zero, norm) * norm;

            Vector2[] res = { new Vector2(x, y), -v, norm };
            return res;
        }
        return null;
    }
    */

    // See above
    // Returns this ball's position at time of collision and norm of that collision
    private Vector2[] GetCollisionInfo(Ball otherBall, Vector2 initialVel)
    {
        Vector2 lhs = otherBall.transform.position - ball.transform.position;
        Vector2 closestPointOnLine = (Vector2)ball.transform.position + initialVel.normalized * Vector2.Dot(lhs, initialVel.normalized);
        float closestDistSq = Mathf.Pow(otherBall.transform.position.x - closestPointOnLine.x, 2) + Mathf.Pow(otherBall.transform.position.y - closestPointOnLine.y, 2);

        if (closestDistSq <= Mathf.Pow(ball.radius + otherBall.radius, 2))
        {
            float backdist = Mathf.Sqrt(Mathf.Pow(ball.radius + otherBall.radius, 2) - closestDistSq);
            float movementVectorLength = Mathf.Sqrt(Mathf.Pow(initialVel.x, 2) + Mathf.Pow(initialVel.y, 2));
            float x = closestPointOnLine.x - backdist * (initialVel.x / movementVectorLength);
            float y = closestPointOnLine.y - backdist * (initialVel.y / movementVectorLength);

            // Get norm of collision to predict where ball getting hit will go
            Vector2 norm = ((Vector2)otherBall.transform.position - new Vector2(x, y)).normalized;

            return new Vector2[2] { new Vector2(x, y), norm };
        }
        return null;
    }
}
