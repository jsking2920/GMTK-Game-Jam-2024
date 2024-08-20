using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutronStar : MonoBehaviour
{
	[SerializeField] OrbitalBody _owningBody;
	private OrbitalBody _currentLockedBody = null;

	private void OnTriggerEnter2D(Collider2D other)
	{
		var otherBody = other.GetComponent<OrbitalBody>();
		if (otherBody != null)
		{
			_owningBody.StartIgnoringBody(otherBody);
			if (_currentLockedBody == null)
			{
				FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/NeutronSucc");
				otherBody.transform.position = transform.position;
				var rb = otherBody.GetComponent<Rigidbody2D>();
				rb.velocity = Vector2.zero;
				rb.Sleep();
				_currentLockedBody = otherBody;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		var otherBody = other.GetComponent<OrbitalBody>();
		if (otherBody != null && _currentLockedBody == otherBody)
		{
			_currentLockedBody = null;
		}
	}
}
