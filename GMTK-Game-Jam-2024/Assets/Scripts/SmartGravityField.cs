using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script only necessary for wormholes and any orbital body gravity field that needs to selectively ignore certain other bodies
public class SmartGravityField : MonoBehaviour
{
	[HideInInspector] public HashSet<OrbitalBody> IgnoredBodies = new();

	private void OnTriggerExit2D(Collider2D other)
	{
		var otherBody = other.GetComponent<OrbitalBody>();
		if (otherBody != null && IgnoredBodies.Contains(otherBody))
		{
			IgnoredBodies.Remove(otherBody);
		}
	}
}
