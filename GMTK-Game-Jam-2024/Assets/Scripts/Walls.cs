using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour
{
	public float animVal = 0f;

	public bool updateShader = false;
	private Material _material;

	private void Awake()
	{
		_material = GetComponent<Renderer>().material;
		_material.SetFloat("_Fade", 0f);
	}

	// Update is called once per frame
    private void Update()
    {
	    if (updateShader)
	    {
			_material.SetFloat("_Fade", animVal);
	    }
    }
}
