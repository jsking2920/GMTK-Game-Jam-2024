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
	private static float minGravDist = 1f;

	public bool IsAttractor;
	public bool IsAttractee;

	[SerializeField] private InitialVelocityType _initVelocityType = InitialVelocityType.SetVelocity;
	[SerializeField] private Vector2 _initVelocity;
	[SerializeField] private List<OrbitalBody> _initialOrbits;

	//Optional param for wormholes and any orbital body that needs to ignore gravity for some objects temporarily
	[SerializeField] private SmartGravityField _smartGravityField;
	[SerializeField] private bool changeDragAfterCollision = false;

	[Header("Neutron Star Only Fields")]
	[SerializeField] private bool _isNeutronStar = false;
	[SerializeField] private float _neutronLockDistance = 0.01F;
	public bool IsLocked = false;
	[SerializeField] private float _unlockDelay = 0.3F;

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

		if (_isNeutronStar)
		{
			if (!otherOrbitalBody.IsLocked && Vector3.Distance(other.gameObject.transform.position, transform.position) < _neutronLockDistance)
			{
				other.gameObject.transform.position = transform.position;
				otherOrbitalBody.IsLocked = true;
				other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			}

			if (Vector3.Distance(other.gameObject.transform.position, transform.position) >= _neutronLockDistance)
			{
				StartCoroutine(UnlockPlanet(otherOrbitalBody));
			}
		}

		if (otherOrbitalBody != null && otherOrbitalBody.IsAttractee && !IgnoredBodies.Contains(otherOrbitalBody) && !otherOrbitalBody.IsLocked)
		{
			ApplyGravityTo(otherOrbitalBody);
		}
	}

	IEnumerator UnlockPlanet(OrbitalBody other)
	{
		yield return new WaitForSeconds(_unlockDelay);
		other.IsLocked = false;
	}

	//this is for wormholes, when object TPs we temporarily ignore gravity from destination wormhole
	private void OnCollisionEnter2D(Collision2D other)
	{
		// dumb fix so orbiting bodies gain drag when knocked out. otherwise they will fly forever which we don't want
		if (changeDragAfterCollision) body.drag = 0.4f;
	}

	public bool IsBodyIgnored(OrbitalBody other)
	{
		return _smartGravityField != null && _smartGravityField.IgnoredBodies.Contains(other);
	}

	public void StartIgnoringBody(OrbitalBody other)
	{
		if (_smartGravityField != null)
		{
			_smartGravityField.IgnoredBodies.Add(other);
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
