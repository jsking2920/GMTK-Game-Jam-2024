using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

enum InitialVelocityType
{
	SetVelocity,
	AutoCircular
}

[RequireComponent(typeof(Rigidbody2D))]
public class OrbitalBody : MonoBehaviour
{
	private static float G = 10f; //make this handled in another script perhaps
	private static float minGravDist = 0.4f;

	public bool IsAttractor;
	public bool IsAttractee;

	[SerializeField] private InitialVelocityType _initVelocityType = InitialVelocityType.SetVelocity;
	[SerializeField] private Vector2 _initVelocity;
	[SerializeField] private List<OrbitalBody> _initialOrbits;

	public HashSet<OrbitalBody> IgnoredBodies = new();

	private Rigidbody2D _body;
	public Rigidbody2D body
	{
		get
		{
			if (_body) return _body;
			return _body = GetComponent<Rigidbody2D>();
		}
	}

	// Start is called before the first frame update
	private void Start()
	{
		InitialLaunch();
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (!IsAttractor)
		{
			return;
		}
		var otherOrbitalBody = other.GetComponent<OrbitalBody>();
		if (otherOrbitalBody != null && otherOrbitalBody.IsAttractee && !IgnoredBodies.Contains(otherOrbitalBody))
		{
			ApplyGravityTo(otherOrbitalBody);
		}
	}

	//this is for wormholes, when object TPs we temporarily ignore gravity from destination wormhole
	private void OnTriggerExit2D(Collider2D other)
	{
		var otherBody = other.GetComponent<OrbitalBody>();
		if (otherBody != null && IgnoredBodies.Contains(otherBody))
		{
			IgnoredBodies.Remove(otherBody);
		}
	}

	public void InitialLaunch()
	{
		if (_initialOrbits.Count == 0 || _initVelocityType == InitialVelocityType.SetVelocity)
		{
			body.velocity += _initVelocity;
		}
		else
		{
			foreach (var parent in _initialOrbits)
			{
				Vector2 difference = parent.body.position - body.position;
				Vector2 gravityDirection = difference.normalized;
				float distance = difference.magnitude;
				Vector2 direction = new Vector2(gravityDirection.y, -gravityDirection.x);
				body.velocity += direction * Mathf.Sqrt(G * parent.body.mass / distance);
			}
		}
	}

	private void ApplyGravityTo(OrbitalBody other)
	{
		Vector2 difference = body.position - other.body.position;
		float distance = Mathf.Max(difference.magnitude, minGravDist);

		float forceMagnitude = G * body.mass * other.body.mass / Mathf.Pow(distance, 2);

		Vector2 forceDirection = difference.normalized;
		Vector2 forceVector = forceDirection * forceMagnitude;
		other.body.AddForce(forceVector);
	}
}
