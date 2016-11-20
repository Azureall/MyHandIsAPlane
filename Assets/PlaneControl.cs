using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;


public class PlaneControl : MonoBehaviour {
	public GameObject planebody, planewing, planetail, planerudder;
	public GameObject myo = null;
	public bool fullcontrol = true;
	float speed = 80.0f;
	public float halfControlRate = 2.0f;
	public float rollrate = 2.0f;
	public float pitchrate = 2.0f;
	public float brakerate = 5.0f;
	public float minSpeed = 80.0f;
	public float maxSpeed= 300.0f;
	public bool collided = false;
	float pitch, roll, prevpitch, prevroll;
	public GameObject camera, crash;
	public bool debugMode = false; //Allows keyboard controls

	private Quaternion _antiYaw = Quaternion.identity;

	// A reference angle representing how the armband is rotated about the wearer's arm, i.e. roll.
	// Set by making the fingers spread pose or pressing "r".
	private float _referenceRoll = 0.0f;

	// Use this for initialization
	void Start () {
	}

	void Awake(){
		Vector3 referenceZeroRoll = computeZeroRollVector (myo.transform.forward);
		_referenceRoll = rollFromZero (referenceZeroRoll, myo.transform.forward, myo.transform.up);
	}
	// Update is called once per frame
	void Update () {
		if (collided) {
			speed = 0;
		} else if (!collided) {
			
			bool updateReference = false;
			ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();
			Vector3 camChaseSpot = transform.position -
			                      transform.forward * 10.0f +
			                      Vector3.up * 5.0f;
			float chaseBias = 0.9f;
			camera.transform.position = chaseBias * camera.transform.position +
			(1.0f - chaseBias) * camChaseSpot;
			camera.transform.LookAt (transform.position + transform.forward * 20.0f);

			speed -= transform.forward.y;
			if (thalmicMyo.pose == Pose.Fist) {
				speed -= brakerate;
			} else if (thalmicMyo.pose == Pose.WaveIn) {
				UnityEngine.VR.InputTracking.Recenter ();
			} else if (thalmicMyo.pose == Pose.WaveOut) {
				updateReference = true;
			}

			if (Input.GetKeyDown("r")) {
				UnityEngine.VR.InputTracking.Recenter ();
			}

			if (speed < minSpeed) {
				speed = minSpeed;
			} else if (speed > maxSpeed) {
				speed = maxSpeed;
			}

			float controlEffect = speed / 120.0f;

			if (transform.position.y < Terrain.activeTerrain.SampleHeight (transform.position)) {
				speed = 0.0f;
				transform.position =
				new Vector3 (transform.position.x,
					Terrain.activeTerrain.SampleHeight (transform.position),
					transform.position.z);
			}

			if (updateReference) {
				Vector3 referenceZeroRoll = computeZeroRollVector (myo.transform.forward);
				_referenceRoll = rollFromZero (referenceZeroRoll, myo.transform.forward, myo.transform.up);
			}

			Vector3 zeroRoll = computeZeroRollVector (myo.transform.forward);
			float roll = (rollFromZero (zeroRoll, myo.transform.forward, myo.transform.up));

			float relativeRoll = normalizeAngle (roll - _referenceRoll);
			
			//y is pitch, z is roll

			//roll = myo.transform.forward.x;
			pitch = -myo.transform.forward.y;

			transform.position += transform.forward * Time.deltaTime * speed;
			//transform.Rotate ((pitch - prevpitch/2) * pitchrate, 0.0f, (roll - prevroll/2) * rollrate);
			if (fullcontrol) {
				transform.Rotate ((pitch - prevpitch / 2) * pitchrate, 0.0f, relativeRoll / 25);
			} else {
				transform.Rotate ((pitch - prevpitch / 2) * pitchrate, 0.0f, 0.0f);
				transform.eulerAngles = new Vector3 (
					transform.eulerAngles.x,
					transform.eulerAngles.y,
					relativeRoll * halfControlRate
				);

			}

			if (debugMode) {
				transform.Rotate(controlEffect*Input.GetAxis("Vertical"),0.0f,
					-controlEffect*Input.GetAxis("Horizontal"));
			}

			prevroll = roll;
			prevpitch = pitch;
		}
	}

	float rollFromZero (Vector3 zeroRoll, Vector3 forward, Vector3 up)
	{
		// The cosine of the angle between the up vector and the zero roll vector. Since both are
		// orthogonal to the forward vector, this tells us how far the Myo has been turned around the
		// forward axis relative to the zero roll vector, but we need to determine separately whether the
		// Myo has been rolled clockwise or counterclockwise.
		float cosine = Vector3.Dot (up, zeroRoll);

		// To determine the sign of the roll, we take the cross product of the up vector and the zero
		// roll vector. This cross product will either be the same or opposite direction as the forward
		// vector depending on whether up is clockwise or counter-clockwise from zero roll.
		// Thus the sign of the dot product of forward and it yields the sign of our roll value.
		Vector3 cp = Vector3.Cross (up, zeroRoll);
		float directionCosine = Vector3.Dot (forward, cp);
		float sign = directionCosine < 0.0f ? 1.0f : -1.0f;

		// Return the angle of roll (in degrees) from the cosine and the sign.
		return sign * Mathf.Rad2Deg * Mathf.Acos (cosine);
	}

	// Compute a vector that points perpendicular to the forward direction,
	// minimizing angular distance from world up (positive Y axis).
	// This represents the direction of no rotation about its forward axis.
	Vector3 computeZeroRollVector (Vector3 forward)
	{
		Vector3 antigravity = Vector3.up;
		Vector3 m = Vector3.Cross (myo.transform.forward, antigravity);
		Vector3 roll = Vector3.Cross (m, myo.transform.forward);

		return roll.normalized;
	}

	float normalizeAngle (float angle)
	{
		if (angle > 180.0f) {
			return angle - 360.0f;
		}
		if (angle < -180.0f) {
			return angle + 360.0f;
		}
		return angle;
	}

	void OnCollisionEnter(Collision col){
		Destroy (planebody); 
		Destroy (planerudder); 
		Destroy (planetail);
		Destroy (planewing);
		collided = true;
		crash.transform.position = transform.position;
		crash.transform.rotation = Quaternion.Euler (camera.transform.rotation.x, 0.0f, camera.transform.rotation.y);
	}
}