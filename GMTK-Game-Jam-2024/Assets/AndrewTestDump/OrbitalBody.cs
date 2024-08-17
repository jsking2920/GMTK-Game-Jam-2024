using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum InitialVelocityType
{
	AutoCircular,
	SetSpeed, //uses just x comp of initVelocity, moves perpendicular to gravity
	SetVelocity
}

[RequireComponent(typeof(Rigidbody))]
public class OrbitalBody : MonoBehaviour
{
	private static float G = 0.0001f; //make this handled in another script

	[SerializeField] private Rigidbody _parent;

	[SerializeField] private InitialVelocityType _initVelocityType = InitialVelocityType.AutoCircular;
	[SerializeField] private Vector3 _initVelocity;

	private Rigidbody _body;


	// Start is called before the first frame update
	void Start()
	{
		_body = GetComponent<Rigidbody>();
		Vector3 difference = _parent.position - _body.position;
		Vector3 gravityDirection = difference.normalized;
		float distance = difference.magnitude;

		Vector3 direction = Vector3.Cross(_parent.transform.up, gravityDirection);
		switch (_initVelocityType)
		{
			case InitialVelocityType.AutoCircular:
				_body.AddForce(direction * Mathf.Sqrt(G * _parent.mass / distance), ForceMode.VelocityChange);
				break;
			case InitialVelocityType.SetSpeed:
				_body.AddForce(direction * _initVelocity.x, ForceMode.VelocityChange);
				break;
			case InitialVelocityType.SetVelocity:
				_body.AddForce(_initVelocity, ForceMode.VelocityChange);
				break;
		}
	}

	void FixedUpdate()
	{
		Vector3 difference = _parent.position - _body.position;
		float distance = difference.magnitude;

		float forceMagnitude = G * _parent.mass * _body.mass / Mathf.Pow(distance, 2);
		
		Vector3 forceDirection = difference.normalized;
		Vector3 forceVector = forceDirection * forceMagnitude;
		_body.AddForce(forceVector);
	}
}
