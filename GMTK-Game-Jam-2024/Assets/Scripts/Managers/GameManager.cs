using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Level[] levels;
    private int curLevelIndex = 0;

    private int shotsTaken = 0;
    private int ballsSunk = 0;

    public int defaultLevelIndex = 0;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SceneManager.LoadScene(levels[defaultLevelIndex].sceneBuildIndex, LoadSceneMode.Additive);
        curLevelIndex = defaultLevelIndex;
        shotsTaken = 0;
        ballsSunk = 0;
        MusicManager.Instance.loadBackgroundTrack();
    }

    private void LoadNewLevel(int levelIndex)
    {
        SceneManager.UnloadSceneAsync(levels[curLevelIndex].sceneBuildIndex);
        SceneManager.LoadScene(levels[levelIndex].sceneBuildIndex, LoadSceneMode.Additive);
        curLevelIndex = levelIndex;
        shotsTaken = 0;
        ballsSunk = 0;
        MusicManager.Instance.changeBackgroundTrack(curLevelIndex);
    }

    public void RestartLevel()
    {
        LoadNewLevel(curLevelIndex);
    }

    public void OnPlayerShot(Ball cueBall)
    {
        shotsTaken += 1;
        UIManager.Instance.UpdateShotCount(shotsTaken);
    }

    public void OnBallSunk(Ball ball)
    {
        ballsSunk += 1;
        Destroy(ball.gameObject);

        if (ballsSunk >= levels[curLevelIndex].totalBallsToSink)
        {
            LoadNewLevel(curLevelIndex + 1 >= levels.Length ? 0 : curLevelIndex + 1);
        }
    }

    public void OnCueBallSunk(Ball ball)
    {
        // TODO: Let player reposition cue ball and shoot again
    }
}
