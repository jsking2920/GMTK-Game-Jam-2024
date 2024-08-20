using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarDisplay : MonoBehaviour
{
    [SerializeField] private Sprite[] _emptyStars;
    [SerializeField] private Sprite[] _filledStars;

    [SerializeField] private int index = 0;
    private Image image;
    private TextMeshProUGUI text;

    void Start()
    {
        image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        int threshold = 0; 
        switch(index)
        {
            case 0:
            threshold = GameManager.Instance.levels[GameManager.Instance.curLevelIndex].oneStarThreshold;
            text.text = GameManager.Instance.levels[GameManager.Instance.curLevelIndex].oneStarThreshold.ToString() +" Strokes";
            break;
            case 1:
            threshold = GameManager.Instance.levels[GameManager.Instance.curLevelIndex].twoStarThreshold;
            text.text = GameManager.Instance.levels[GameManager.Instance.curLevelIndex].twoStarThreshold.ToString() +" Strokes";
            break;
            case 2:
            threshold = GameManager.Instance.levels[GameManager.Instance.curLevelIndex].threeStarThreshold;
            text.text = GameManager.Instance.levels[GameManager.Instance.curLevelIndex].threeStarThreshold.ToString() +" Strokes";
            break;
        }
        if (GameManager.Instance.shotsTaken <= threshold)
        {
            image.sprite = _filledStars[index];
        }
        else
        {
            image.sprite = _emptyStars[index];
        }
    }
}
