using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour
{
	public float animVal = 0f;

	public bool updateShader = false;
	private Material _material;
	private Animator _animator;

	[SerializeField] private GameObject _wallParent;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
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

    public void PlayFade()
    {
		_animator.SetBool("fadePlaying", true);
    }

    public void FadeDone()
    {
		_animator.SetBool("fadePlaying", false);
    }

    public void HideWalls()
    {
	    FadeDone();
	    _material.SetFloat("_Fade", 0f);
	}
}
