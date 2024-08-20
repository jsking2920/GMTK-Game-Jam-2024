using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject MainUICanvas;
    
    [SerializeField] private Button mainMenuStartButton;
    
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button levelsButton;
    [SerializeField] private Button playPauseButton;

    [SerializeField] private TextMeshProUGUI shotCountText;

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
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(btn_Quit);
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(btn_Restart);
        levelsButton.onClick.RemoveAllListeners();
        levelsButton.onClick.AddListener(btn_Levels);
        playPauseButton.onClick.RemoveAllListeners();
        playPauseButton.onClick.AddListener(btn_PlayPause);
//#if UNITY_WEBGL
        quitButton.gameObject.SetActive(false);
        //#endif
        
        mainMenuStartButton.onClick.RemoveAllListeners();
        mainMenuStartButton.onClick.AddListener(GameManager.Instance.StartGame);

        shotCountText.text = "Shots: 0";
        MainUICanvas.SetActive(false);
    }

    public void UpdateShotCount(int shots)
    {
        shotCountText.text = "Shots: " + shots.ToString();
    }

    public void btn_PlayPause()
    {
        // TODO: allow player to stop time?
    }

    public void btn_Levels()
    {
        // TODO
    }

    public void btn_Restart()
    {
        GameManager.Instance.RestartLevel();
    }

    public void btn_Quit()
    {
        Application.Quit();
    }

    public void SetMainMenuActive(bool active)
    {
        var camera = MainMenu.GetComponentInChildren<CinemachineVirtualCamera>();
        if (active)
        {
            //Turning on
            camera.enabled = true;
            MainUICanvas.SetActive(false);
            MainMenu.SetActive(true);
        }
        else
        {
            //Turning off
            camera.enabled = false;
            MainUICanvas.SetActive(true);
            MainMenu.SetActive(false);
        }
    }
}
