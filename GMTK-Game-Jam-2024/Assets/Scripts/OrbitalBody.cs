using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum InitialVelocityType
{
	SetVelocity,
	AutoCircular
}

[RequireComponent(typeof(Rigidbody2D))]
public class OrbitalBody : MonoBehaviour
{
	private static float G = 10f; //make this handled in another script perhaps

	public bool IsAttractor;
	public bool IsAttractee;

	[SerializeField] private InitialVelocityType _initVelocityType = InitialVelocityType.SetVelocity;
	[SerializeField] private Vector2 _initVelocity;
	[SerializeField] private List<OrbitalBody> _initialOrbits;

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
		if (otherOrbitalBody && otherOrbitalBody.IsAttractee)
		{
			ApplyGravityTo(otherOrbitalBody);
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
		float distance = difference.magnitude;

		float forceMagnitude = G * body.mass * other.body.mass / Mathf.Pow(distance, 2);

		Vector2 forceDirection = difference.normalized;
		Vector2 forceVector = forceDirection * forceMagnitude;
		other.body.AddForce(forceVector);
	}
}
