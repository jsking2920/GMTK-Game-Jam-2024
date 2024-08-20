using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour
{
    private int level;

    public void setLevel(int newLevel)
    {
        level = newLevel;
    }
    public void btn_LevelSelect()
    {
        GameManager.Instance.LoadNewLevel(level);
    }
}
