using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wormhole : MonoBehaviour
{
	[SerializeField] private Wormhole _output;

	public OrbitalBody OwningBody;

	private HashSet<OrbitalBody> ballsImTransportingRn = new HashSet<OrbitalBody>();

	private void OnTriggerEnter2D(Collider2D other)
	{
		var otherBody = other.GetComponent<OrbitalBody>();
		if (otherBody != null && !OwningBody.IgnoredBodies.Contains(otherBody) 
		                      && !ballsImTransportingRn.Contains(otherBody))
		{
			_output.OwningBody.IgnoredBodies.Add(otherBody);
			
			// Vector3 offset = otherBody.transform.position - transform.position;
			// otherBody.transform.position = offset + _output.transform.position;
			ballsImTransportingRn.Add(otherBody);
			StartCoroutine(WormholeEat(otherBody));
		}
	}

	private IEnumerator WormholeEat(OrbitalBody otherBody)
	{
		//TODO: trigger face animation
		AudioManager.PlaySpatializedSFX("event:/SFX/Wormhole", otherBody.transform.position);

		Vector3 originalScale = otherBody.transform.localScale;
		Vector2 originalVelocity = otherBody.body.velocity;

		//Enter the wormhole
		{
			float shrinkTime = 0.4f;
			Vector3 startingScale = otherBody.transform.localScale;
			Vector3 startingPos = otherBody.transform.position;
			Vector3 endingScale = new Vector3(0.0f, 0.0f);
			Vector3 endingPos = transform.position;
			for (float t = 0; t < shrinkTime;)
			{
				otherBody.transform.localScale = EasingFunction.EaseVector(startingScale, endingScale, t / shrinkTime,
					EasingFunction.Ease.EaseInCubic);
				otherBody.transform.position = EasingFunction.EaseVector(startingPos, endingPos, t / shrinkTime,
					EasingFunction.Ease.EaseInCubic);
				t += Time.deltaTime;
				yield return null;
			}
		}
		Vector3 offset = otherBody.transform.position - transform.position;
		otherBody.transform.position = offset + _output.transform.position;
		otherBody.gameObject.SetActive(false);

		//delay
		yield return new WaitForSeconds(0.8f);
		
		//exit
		{
			//teleport
			otherBody.gameObject.SetActive(true);
			otherBody.body.velocity = originalVelocity * 0.75f; //prevent launch from gravity
			
			float growTime = 0.3f;
			Vector3 startingScale = new Vector3(0.0f, 0.0f);
			Vector3 endingScale =  originalScale;
			for (float t = 0; t < growTime;)
			{
				otherBody.transform.localScale = EasingFunction.EaseVector(startingScale, endingScale, t / growTime,
					EasingFunction.Ease.EaseInCubic);
				Debug.Log(t / growTime);
				t += Time.deltaTime;
				yield return null;
			}

			otherBody.transform.localScale = originalScale;
			ballsImTransportingRn.Remove(otherBody);
		}
	}
}
