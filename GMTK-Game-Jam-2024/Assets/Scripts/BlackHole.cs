using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
        OrbitalBody body = other.GetComponent<OrbitalBody>();
		if (body != null)
		{
			if (other.CompareTag("CueBall")) //or whatever
			{
				HandleCueBall(body);
			}
			else if (other.CompareTag("Ball"))
			{
				HandleBall(body);
			}
			else
			{
				// TODO
			}
		}
	}

	private void HandleBall(OrbitalBody body)
	{
		GameManager.Instance.OnBallSunk(body.gameObject.GetComponent<Ball>());
	}

	private void HandleCueBall(OrbitalBody body)
	{
        GameManager.Instance.OnCueBallSunk(body.gameObject.GetComponent<Ball>());
    }
}
