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

[RequireComponent(typeof(Rigidbody))]
public class OrbitalBody : MonoBehaviour
{
	private static float G = 0.0001f; //make this handled in another script perhaps

	[SerializeField] private List<OrbitalBody> _parents;

	[SerializeField] private InitialVelocityType _initVelocityType = InitialVelocityType.AutoCircular;
	[SerializeField] private Vector3 _initVelocity;

	private Rigidbody _body;
	public Rigidbody body
	{
		get
		{
			if (_body) return _body;
			return _body = GetComponent<Rigidbody>();
		}
	}

	private bool _initialLaunchHandled = false;
	[DoNotSerialize] public Vector3 InitialLaunchVelocity;

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
			body.AddForce(InitialLaunchVelocity, ForceMode.VelocityChange);
		}
		else
		{
			switch (_initVelocityType)
			{
				case InitialVelocityType.AutoCircular:
				{
					foreach (var parent in _parents)
					{
						Vector3 difference = parent.body.position - body.position;
						Vector3 gravityDirection = difference.normalized;
						float distance = difference.magnitude;

						Vector3 direction = Vector3.Cross(parent.transform.up, gravityDirection);
						InitialLaunchVelocity += direction * Mathf.Sqrt(G * parent.body.mass / distance);
					}
					break;
				}
				case InitialVelocityType.SetSpeed:
				{
					Vector3 difference = _parents[0].body.position - body.position;
					Vector3 gravityDirection = difference.normalized;
					Vector3 direction = Vector3.Cross(_parents[0].transform.up, gravityDirection);
					InitialLaunchVelocity = direction * _initVelocity.x;
					break;
				}
				case InitialVelocityType.SetVelocity:
				{
					InitialLaunchVelocity = _initVelocity;
					break;
				}
			}

			body.AddForce(InitialLaunchVelocity, ForceMode.VelocityChange);
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
		Vector3 difference = parent.body.position - body.position;
		float distance = difference.magnitude;

		float forceMagnitude = G * parent.body.mass * body.mass / Mathf.Pow(distance, 2);

		Vector3 forceDirection = difference.normalized;
		Vector3 forceVector = forceDirection * forceMagnitude;
		body.AddForce(forceVector);
	}
}
