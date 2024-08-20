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

    public void loadBackgroundTrack()
    {
        backgroundTrack = RuntimeManager.CreateInstance(BackgroundTrack);
        backgroundTrack.start();
    }


    public void changeBackgroundTrack(int curLevelIndex)
    {
        //Debug.Log("is this called");
        //currScene = SceneManager.GetActiveScene();
        //Debug.Log("the current scene is " + currScene);
        if (curLevelIndex <= 2)
        {
            backgroundTrack.setParameterByNameWithLabel("State", "Level1", true);
            Debug.Log("Level 1 music start");
        }
        else if (curLevelIndex <= 5)
        {
            backgroundTrack.setParameterByNameWithLabel("State", "Level2", true);
            Debug.Log("Level 2 music start");
        }
        else if (curLevelIndex <= 9)
        {
            backgroundTrack.setParameterByNameWithLabel("State", "Level3", true);
            Debug.Log("Level 3 music start");
        }
    }
}
