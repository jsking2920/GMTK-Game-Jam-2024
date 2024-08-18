using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using Debug = FMOD.Debug;

public class BallCollisionSFXComponent : MonoBehaviour
{
    public static float minCollisionSpeed = 0;
    public static float maxCollisionSpeed = 25;
    public static float maxDistFromCamera = 15;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contactPoint = collision.contacts[0];
        float speed = collision.relativeVelocity.magnitude;
        float remappedSpeed = (speed - minCollisionSpeed) / (maxCollisionSpeed - minCollisionSpeed);
        Vector3 listenerPos = AudioManager.Instance.listener.attenuationObject.transform.position;
        float distance = (contactPoint.point - new Vector2(listenerPos.x, listenerPos.y)).magnitude;
        float attn = Mathf.Pow(Mathf.Clamp01((maxDistFromCamera - distance) / maxDistFromCamera), 2);
        
        EventInstance sound = RuntimeManager.CreateInstance("event:/SFX/Collision");
        sound.setParameterByName("ImpactSpeed", remappedSpeed);
        sound.set3DAttributes(RuntimeUtils.To3DAttributes(contactPoint.point));

        sound.setVolume(attn);
        sound.start();
        sound.release();
    }
}
