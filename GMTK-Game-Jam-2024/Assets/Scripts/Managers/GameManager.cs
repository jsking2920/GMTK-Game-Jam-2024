using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        MainMenu,
        Playing
    }
    public GameState gameState;

    public Level[] levels;
    [HideInInspector] public int curLevelIndex = 0;

    [HideInInspector] public int shotsTaken = 0;
    private int ballsSunk = 0;

    [SerializeField] private GameObject levelEndScreen;

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
        ToMenu();
    }

    private IEnumerator WaitForMainMenuCameraMove()
    {
	    yield return new WaitForSeconds(3);
        UIManager.Instance.TurnMenuPlanetsOff();
    }

    public void LoadNewLevel(int levelIndex)
    {
        if (SceneManager.GetSceneByBuildIndex(levels[curLevelIndex].sceneBuildIndex).isLoaded)
            SceneManager.UnloadSceneAsync(levels[curLevelIndex].sceneBuildIndex);
        SceneManager.LoadScene(levels[levelIndex].sceneBuildIndex, LoadSceneMode.Additive);
        curLevelIndex = levelIndex;

        Time.timeScale = 1;
        shotsTaken = 0;
        ballsSunk = 0;
        UIManager.Instance.UpdateShotCount(shotsTaken);

        MusicManager.Instance.changeBackgroundTrack(curLevelIndex);

        UIManager.Instance.SetMainMenuActive(false);

        gameState = GameState.Playing;

        StartCoroutine(WaitForMainMenuCameraMove());
    }

    public void RestartLevel()
    {
        levelEndScreen.SetActive(false);

        if (SceneManager.GetSceneByBuildIndex(levels[curLevelIndex].sceneBuildIndex).isLoaded)
            SceneManager.UnloadSceneAsync(levels[curLevelIndex].sceneBuildIndex);
        SceneManager.LoadScene(levels[curLevelIndex].sceneBuildIndex, LoadSceneMode.Additive);

        Time.timeScale = 1;
        shotsTaken = 0;
        ballsSunk = 0;
        UIManager.Instance.UpdateShotCount(shotsTaken);

        gameState = GameState.Playing;
    }

    public void ToMenu()
    {
        if (SceneManager.GetSceneByBuildIndex(levels[curLevelIndex].sceneBuildIndex).isLoaded)
            SceneManager.UnloadSceneAsync(levels[curLevelIndex].sceneBuildIndex);
        Time.timeScale = 1;
        levelEndScreen.SetActive(false);

        MusicManager.Instance.loadBackgroundTrack();
        UIManager.Instance.SetMainMenuActive(true);

        gameState = GameState.MainMenu;
    }

    public void MenuPlayButton()
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
            if (shotsTaken <= levels[curLevelIndex].threeStarThreshold)
            {
                if (levels[curLevelIndex].bestStarRankAchieved < 3) 
                    levels[curLevelIndex].bestStarRankAchieved = 3;
            }
            else if (shotsTaken <= levels[curLevelIndex].twoStarThreshold)
            {
                if (levels[curLevelIndex].bestStarRankAchieved < 2)
                    levels[curLevelIndex].bestStarRankAchieved = 2;
            }
            else if (shotsTaken <= levels[curLevelIndex].oneStarThreshold)
            {
                if (levels[curLevelIndex].bestStarRankAchieved < 1)
                    levels[curLevelIndex].bestStarRankAchieved = 1;
            }
            else
            {
                levels[curLevelIndex].bestStarRankAchieved = 0;
            }
            
            levelEndScreen.SetActive(true);
        }
    }

    public void GoToNextLevel()
    {
        levelEndScreen.SetActive(false);
        LoadNewLevel(curLevelIndex + 1 >= levels.Length ? 0 : curLevelIndex + 1);
    }

    public void OnCueBallSunk(Ball ball)
    {
        
    }
}
