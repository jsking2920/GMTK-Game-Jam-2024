using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private Scene currScene;
    [SerializeField] private EventReference BackgroundTrack;
    private EventInstance backgroundTrack;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void playBackgroundTrack()
    {
        backgroundTrack = RuntimeManager.CreateInstance(BackgroundTrack);
        currScene = SceneManager.GetActiveScene();
        backgroundTrack.start();
        if (currScene.name == "Lv1 - Intro")
        {
            backgroundTrack.setParameterByNameWithLabel("State", "Level1", true);
            Debug.Log("Level 1 music start");
        }
        else if (currScene.name == "Lv2 - Size")
        {
            backgroundTrack.setParameterByNameWithLabel("State", "Level2", true);
            Debug.Log("Level 2 music start");
        }
        else if (currScene.name == "Lv3 - Timing")
        {
            backgroundTrack.setParameterByNameWithLabel("State", "Level3", true);
            Debug.Log("Level 3 music start");
        }
    }
}
