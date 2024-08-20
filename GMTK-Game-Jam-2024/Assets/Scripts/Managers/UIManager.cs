using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Metadata;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject MainUICanvas;
    [SerializeField] private GameObject LevelSelectCanvas;
    [SerializeField] private GameObject wallParent;
    
    [SerializeField] private Button mainMenuStartButton;
    
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button levelsButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button settingsButton;

    [SerializeField] private TextMeshProUGUI shotCountText;
    [SerializeField] private Button levelBackToMenuButton;

    bool isSettingsOpen = false;
    [SerializeField] private List<Button> ListSelectButtons;

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
//#if UNITY_WEBGL
        quitButton.gameObject.SetActive(false);
        //#endif
        
        mainMenuStartButton.onClick.RemoveAllListeners();
        mainMenuStartButton.onClick.AddListener(GameManager.Instance.StartGame);
        mainMenuStartButton.onClick.AddListener(playSFX);
        ;

        homeButton.onClick.RemoveAllListeners();
        homeButton.onClick.AddListener(btn_Home);

        settingsButton.onClick.RemoveAllListeners();
        settingsButton.onClick.AddListener(btn_Settings);

        homeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        shotCountText.text = "Shots: 0";
        MainUICanvas.SetActive(false);
        LevelSelectCanvas.SetActive(false);

        for (int i = 0; i < ListSelectButtons.Count; ++i)
        {
            Button button = ListSelectButtons[i].GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => LevelSelectCanvas.SetActive(false));
            LevelSelectButton buttonScript = button.GetComponent<LevelSelectButton>();
            buttonScript.setLevel(i);
            button.onClick.AddListener(buttonScript.btn_LevelSelect);
        }
    }

    public void UpdateShotCount(int shots)
    {
        shotCountText.text = "Shots: " + shots.ToString();
    }

    private void playSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ButtonClick");
    }

    public void btn_Levels()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ButtonClick");   
        LevelSelectCanvas.SetActive(true);
    }

    public void btn_Restart()
    {
        GameManager.Instance.RestartLevel();
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ButtonClick");
    }

    public void btn_Quit()
    {
        Application.Quit();
    }

    public void btn_Home()
    {
        // TODO
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ButtonClick");
    }

    public void btn_Settings()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ButtonClick");
        isSettingsOpen = !isSettingsOpen;

        restartButton.gameObject.SetActive(isSettingsOpen);
        homeButton.gameObject.SetActive(isSettingsOpen);
        if (isSettingsOpen) settingsButton.targetGraphic.color = Color.gray;
        else settingsButton.targetGraphic.color = Color.white;
    }

    public void btn_LevelSelect(int level)
    {
        Debug.Log(level);
        
        
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
            wallParent.SetActive(false);
        }
        else
        {
            //Turning off
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ZoomOut");
            camera.enabled = false;
            MainUICanvas.SetActive(true);
            MainMenu.SetActive(false);
            wallParent.SetActive(true);
        }
    }
}
