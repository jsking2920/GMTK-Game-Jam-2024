using UnityEngine;

public class TrajectoryPath : MonoBehaviour
{
    [SerializeField] private int numDots;
    [SerializeField] private GameObject dotsParent;
    [SerializeField] private GameObject dotPrefab;

    [SerializeField] private float spacing;
    [SerializeField] [Range(0.01f, 0.3f)] float dotMinScale;
    [SerializeField] [Range(0.3f, 0.5f)] float dotMaxScale;

    private Transform[] dotsList;

    void Start()
    {
        HidePath();
        PrepareDots();
    }

    private void PrepareDots()
    {
        dotsList = new Transform[numDots];
        dotPrefab.transform.localScale = Vector3.one * dotMaxScale;

        float scale = dotMaxScale;
        float scaleFactor = scale / numDots;

        for (int i = 0; i < numDots; i++) 
        {
            dotsList[i] = Instantiate(dotPrefab, null).transform;
            dotsList[i].parent = dotsParent.transform;

            dotsList[i].localScale = Vector3.one * scale;
            if (scale > dotMinScale)
            {
                scale -= scaleFactor;
            }
        }
    }

    public void UpdatePath(Vector3 ballPosition, Vector2 forceApplied)
    {
        float curDotOffset = spacing;
        Vector2 dotPos = new Vector2();

        for (int i = 0; i < numDots; i++)
        {
            curDotOffset = spacing * i;
            dotPos.x = ballPosition.x + forceApplied.x * curDotOffset;
            dotPos.y = ballPosition.y + forceApplied.y * curDotOffset; // - (Physics2D.gravity.magnitude * curDotOffset * curDotOffset) / 2f;

            dotsList[i].position = dotPos;
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
