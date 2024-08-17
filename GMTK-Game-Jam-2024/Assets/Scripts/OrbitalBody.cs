using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

enum InitialVelocityType
{
	AutoCircular,
	SetSpeed, //uses just x comp of initVelocity, moves perpendicular to gravity
	SetVelocity
}

[RequireComponent(typeof(Rigidbody2D))]
public class OrbitalBody : MonoBehaviour
{
	private static float G = 0.0001f; //make this handled in another script perhaps

	[SerializeField] private List<OrbitalBody> _parents;

	[SerializeField] private InitialVelocityType _initVelocityType = InitialVelocityType.AutoCircular;
	[SerializeField] private Vector2 _initVelocity;

	private Rigidbody2D _body;
	public Rigidbody2D body
	{
		get
		{
			if (_body) return _body;
			return _body = GetComponent<Rigidbody2D>();
		}
	}

	private bool _initialLaunchHandled = false;
	[DoNotSerialize] public Vector2 InitialLaunchVelocity;

	// Start is called before the first frame update
	private void Start()
	{
		foreach (var parent in _parents)
		{
			parent.InitialLaunch();
		}

		InitialLaunch();
	}

	public void InitialLaunch()
	{
		if (_initialLaunchHandled)
		{
			return;
		}
		if (_parents.Count == 0)
		{
			if (_initVelocityType != InitialVelocityType.SetVelocity)
			{
				Debug.LogWarning("Parentless OrbitalBody requires an initial velocity type of SetVelocity");
				_initVelocityType = InitialVelocityType.SetVelocity;
			}
			InitialLaunchVelocity = _initVelocity;
			body.velocity += InitialLaunchVelocity;
		}
		else
		{
			switch (_initVelocityType)
			{
				case InitialVelocityType.AutoCircular:
				{
					foreach (var parent in _parents)
					{
						Vector2 difference = parent.body.position - body.position;
						Vector2 gravityDirection = difference.normalized;
						float distance = difference.magnitude;
						Vector2 direction = new Vector2(gravityDirection.y, -gravityDirection.x);
						InitialLaunchVelocity += direction * Mathf.Sqrt(G * parent.body.mass / distance);
					}
					break;
				}
				case InitialVelocityType.SetSpeed:
				{
					Vector2 difference = _parents[0].body.position - body.position;
					Vector2 gravityDirection = difference.normalized;
					Vector2 direction = new Vector2(gravityDirection.y, -gravityDirection.x);
					InitialLaunchVelocity = direction * _initVelocity.x;
					break;
				}
				case InitialVelocityType.SetVelocity:
				{
					InitialLaunchVelocity = _initVelocity;
					break;
				}
			}

			body.velocity += InitialLaunchVelocity;
		}

		_initialLaunchHandled = true;
	}

	private void FixedUpdate()
	{
		foreach (var parent in _parents)
		{
			ApplyGravityFrom(parent);
		}
	}

	private void ApplyGravityFrom(OrbitalBody parent)
	{
		Vector2 difference = parent.body.position - body.position;
		float distance = difference.magnitude;

		float forceMagnitude = G * parent.body.mass * body.mass / Mathf.Pow(distance, 2);

		Vector2 forceDirection = difference.normalized;
		Vector2 forceVector = forceDirection * forceMagnitude;
		body.AddForce(forceVector);
	}
}
