using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class CueBallSFXComponent : MonoBehaviour
{
    private EventInstance sparkle;
    public float maxSparkleSpeed = 25;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sparkle = RuntimeManager.CreateInstance("event:/SFX/Sparkle");
        sparkle.setParameterByName("SparklePercent", 0);
        sparkle.start();
    }

    // Update is called once per frame
    void Update()
    {
        float sparklePercent = Mathf.Clamp01(rb.velocity.magnitude / maxSparkleSpeed);
        sparkle.setParameterByName("SparklePercent", sparklePercent);
    }
}
