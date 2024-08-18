using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		var body = other.GetComponent<OrbitalBody>();
		if (body != null)
		{
			if (other.CompareTag("CueBall")) //or whatever
			{
				HandleCueBall(body);
			}
			else
			{
				HandleBall(body);
			}
		}
	}

	private void HandleBall(OrbitalBody body)
	{
		body.gameObject.SetActive(false);
		//communicate with score tracking somehow
	}

	private void HandleCueBall(OrbitalBody body)
	{
		//move to start or something
	}
}
