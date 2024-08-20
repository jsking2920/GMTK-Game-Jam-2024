using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = FMOD.Debug;

public class wiggle : MonoBehaviour
{
    private Vector3 startpos;
    private float timeOffset;
    private float timething = 4.0f;
    
    
    private float wigglefactor = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position;
        timeOffset = Random.Range(0.0f, 1.0f) * 5.0f;
        timething += Random.Range(0.0f, 1.0f) * 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Debug.Log(timeOffset);
        transform.position = startpos + new Vector3(0, (Mathf.Sin(timeOffset + Time.time / timething) * wigglefactor), 0);
    }
}
