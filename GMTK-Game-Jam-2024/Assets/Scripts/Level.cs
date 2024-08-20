using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public string levelName;
    public int sceneBuildIndex;
    public int totalBallsToSink;
    public int oneStarThreshold;
    public int twoStarThreshold;
    public int threeStarThreshold;

    [HideInInspector] public int bestStarRankAchieved = 0;
}
