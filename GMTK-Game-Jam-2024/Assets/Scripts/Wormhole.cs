using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wormhole : MonoBehaviour
{
	[SerializeField] private Wormhole _output;

	public OrbitalBody OwningBody;

	private void OnTriggerEnter2D(Collider2D other)
	{
		var otherBody = other.GetComponent<OrbitalBody>();
		if (otherBody != null && !OwningBody.IgnoredBodies.Contains(otherBody))
		{
			_output.OwningBody.IgnoredBodies.Add(otherBody);
			Vector3 offset = otherBody.transform.position - transform.position;
			otherBody.transform.position = offset + _output.transform.position;
		}
	}
}
