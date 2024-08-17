using System.Collections.Generic;
using UnityEngine;

public class TrajectoryRenderer : MonoBehaviour
{
    [SerializeField] private GameObject dotsParent;
    [SerializeField] private GameObject dotPrefab;

    private float simulationStep = 0.05f;
    private float maxSimulationTime = 1.0f;

    private Transform[] dotsList;
    private int numPooledDots = 20;

    private void Start()
    {
        PrepareDots();
        HidePath();
    }

    private void PrepareDots()
    {
        dotsList = new Transform[numPooledDots];

        for (int i = 0; i < numPooledDots; i++)
        {
            dotsList[i] = Instantiate(dotPrefab, dotsParent.transform).transform;
            dotsList[i].gameObject.SetActive(false);
        }
    }

    public void DrawPath(Vector2 startPos, Vector2 forceDir, float initialVel, float minVel, float mass, float linearDrag)
    {
        float curSimulatedVel = initialVel;
        Vector2 curSimulatedDir = forceDir;
        Vector2 curSimulatedPos = startPos;
        int curDotIndex = 0;

        Vector2 nextSimulatedPos;

        while (curSimulatedVel > minVel)
        {
            nextSimulatedPos = curSimulatedPos + (curSimulatedDir * curSimulatedVel * simulationStep);

            dotsList[curDotIndex].position = nextSimulatedPos;
            dotsList[curDotIndex].gameObject.SetActive(true);

            curDotIndex += 1;
            //curSimulatedVel -= simulationStep * linearDrag;
            curSimulatedPos = nextSimulatedPos;
            
            // Check for collision

            if (curDotIndex >= dotsList.Length)
            {
                break;
            }
        }

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
}
