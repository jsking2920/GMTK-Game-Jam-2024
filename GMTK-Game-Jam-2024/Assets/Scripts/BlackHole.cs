using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera bhCamera;
	private static (BlackHole, Ball) currentAnim = (null, null);

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
		StartCoroutine(BlackHoleAnimation(body.gameObject.GetComponent<Ball>()));
	}

	private void HandleCueBall(OrbitalBody body)
	{
        GameManager.Instance.OnCueBallSunk(body.gameObject.GetComponent<Ball>());
    }

	private IEnumerator BlackHoleAnimation(Ball ball)
	{
		//I'm going to just hope this is atomic and Unity doesn't do anything too funky with coroutines...
		float timeOut = 30.0f;
		float waitingTimeElapsed = 0;
		while (waitingTimeElapsed <= timeOut)
		{
			if (currentAnim == (null, null))
			{
				if (ball != null)
				{
					currentAnim = (this, ball);
					break;
				}
				else
				{
					yield break;
				}

			}
			waitingTimeElapsed += Time.unscaledDeltaTime;
			yield return null;
		}

		if (waitingTimeElapsed > timeOut)
		{
			//timed out while waiting
			UnityEngine.Debug.Log("Timed out!");
			GameManager.Instance.OnBallSunk(ball);
			yield break;
		}

		float shrinkTime = 1.0f;
		
		bhCamera.enabled = true;
		Time.timeScale = 0.05f;
		yield return new WaitForSecondsRealtime(0.45f);
		FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/BlackHole");

		Vector3 startingScale = ball.transform.localScale;
		Vector3 startingPos = ball.transform.position;
		Vector3 endingScale = new Vector3(0.0f, 0.0f);
		Vector3 endingPos = transform.position;
		
		
		for (float t = 0; t < shrinkTime;)
		{
			ball.transform.localScale = EasingFunction.EaseVector(startingScale, endingScale, t / shrinkTime, 
				EasingFunction.Ease.EaseInBack);
			ball.transform.position = EasingFunction.EaseVector(startingPos, endingPos, t / shrinkTime, 
				EasingFunction.Ease.EaseInBack);
			t += Time.unscaledDeltaTime;
			yield return null;
		}

		bhCamera.enabled = false;
		Time.timeScale = 1;
		GameManager.Instance.OnBallSunk(ball);
		currentAnim = (null, null);
	}
}
