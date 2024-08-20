using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    private int level;

    [SerializeField] private Image[] stars;

    public void setLevel(int newLevel)
    {
        level = newLevel;
    }
    public void btn_LevelSelect()
    {
        GameManager.Instance.LoadNewLevel(level, true);
    }

    public void SetStars()
    {
        int rank = GameManager.Instance.levels[level].bestStarRankAchieved;

        for (int i = 0; i < stars.Length; i++)
        {
            if (i < rank)
            {
                stars[i].enabled = true;
            }
            else
            {
                stars[i].enabled = false;
            }
        }
    }
}
