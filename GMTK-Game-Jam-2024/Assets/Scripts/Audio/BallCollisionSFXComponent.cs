using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;

public class BallCollisionSFXComponent : MonoBehaviour
{
    public static float minCollisionSpeed = 0;
    public static float maxCollisionSpeed = 25;
    public static float maxDistFromCamera = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contactPoint = collision.contacts[0];
        float speed = collision.relativeVelocity.magnitude;
        float remappedSpeed = (speed - minCollisionSpeed) / (maxCollisionSpeed - minCollisionSpeed);
        
        EventInstance sound = RuntimeManager.CreateInstance("event:/SFX/Collision");
        sound.setParameterByName("ImpactSpeed", remappedSpeed);
        sound.set3DAttributes(RuntimeUtils.To3DAttributes(contactPoint.point));
        sound.start();
        sound.release();
    }
}
