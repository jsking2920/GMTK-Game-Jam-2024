using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelSelectButton : MonoBehaviour
{
    private int level;

    public void setLevel(int newLevel)
    {
        level = newLevel;
    }
    public void btn_LevelSelect()
    {

        GameManager.Instance.StartGame();
        GameManager.Instance.LoadNewLevel(level);
    }
}
