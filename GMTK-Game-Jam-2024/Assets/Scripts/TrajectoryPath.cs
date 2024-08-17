using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class TrajectoryPath : MonoBehaviour
{
    [SerializeField] int dotsNumber;
    [SerializeField] GameObject dotsLine;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] float spacing;
    [SerializeField] [Range(0.01f, 0.3f)] float dotMinScale;
    [SerializeField] [Range(0.3f, 0.5f)] float dotMaxScale;
    Transform[] dotsList;
    Vector2 pos;
    float timeStamp;

    void Start()
    {
        HideDots();
        PrepareDots();
    }

    void PrepareDots()
    {
        dotsList = new Transform[dotsNumber];
        dotPrefab.transform.localScale = Vector3.one * dotMaxScale;

        float scale = dotMaxScale;
        float scaleFactor = scale / dotsNumber;

        for(int i = 0; i < dotsNumber; i++) {
            dotsList[i] = Instantiate(dotPrefab, null).transform;
            dotsList[i].parent = dotsLine.transform;

            dotsList[i].localScale = Vector3.one * scale;
            if(scale > dotMinScale)
            {
                scale -= scaleFactor;
            }
        }
    }

    public void UpdateDots(Vector3 ballPosition, Vector2 forceApplied)
    {
        timeStamp = spacing;
        for(int i = 0;i < dotsNumber; i++)
        {
            pos.x = (ballPosition.x + forceApplied.x * timeStamp);
            pos.y = (ballPosition.y + forceApplied.y * timeStamp) - (Physics2D.gravity.magnitude*timeStamp*timeStamp)/2f;

            dotsList[i].position = pos;
            timeStamp += spacing;
        }
    }

    public void ShowDots()
    {
        dotsLine.SetActive(true);
    }
    public void HideDots()
    {
        dotsLine.SetActive(false);
    }
}
