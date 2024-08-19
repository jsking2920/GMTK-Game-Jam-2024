using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public StudioListener listener;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void PlaySpatializedSFX(string path, Vector2 position, float maxDistFromCamera = 15)
    {
        
        Vector3 listenerPos = Instance.listener.attenuationObject.transform.position;
        float distance = (position - new Vector2(listenerPos.x, listenerPos.y)).magnitude;
        float attn = Mathf.Pow(Mathf.Clamp01((maxDistFromCamera - distance) / maxDistFromCamera), 2);
        
        EventInstance sound = RuntimeManager.CreateInstance(path);
        sound.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        sound.setVolume(attn);
        sound.start();
        sound.release();
    }

}
