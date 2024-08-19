using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private Scene currScene;
    [SerializeField] private EventReference BackgroundTrack;
    private EventInstance backgroundTrack;

    //private void awake()
    //{
    //    backgroundTrack = RuntimeManager.CreateInstance(BackgroundTrack);
    //}
    // Start is called before the first frame update
    void Start()
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
