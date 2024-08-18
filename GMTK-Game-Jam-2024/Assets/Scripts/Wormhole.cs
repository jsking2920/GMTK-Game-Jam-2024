using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wormhole : MonoBehaviour
{
	[SerializeField] private Wormhole _output;

	private HashSet<OrbitalBody> _ignoredBodies = new();

	private void OnTriggerEnter2D(Collider2D other)
	{
		var body = other.GetComponent<OrbitalBody>();
		if (body != null && !_ignoredBodies.Contains(body))
		{
			_output._ignoredBodies.Add(body);
			Vector3 offset = body.transform.position - transform.position;
			body.transform.position = offset + _output.transform.position;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		var body = other.GetComponent<OrbitalBody>();
		if (body != null && _ignoredBodies.Contains(body))
		{
			_ignoredBodies.Remove(body);
		}
	}
}
