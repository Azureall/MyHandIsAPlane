using UnityEngine;
using System.Collections;

public class PlaneControl : MonoBehaviour {
	float speed = 120.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 camChaseSpot = transform.position -
								transform.forward * 10.0f +
				Vector3.up * 5.0f;
		float chaseBias = 0.9f;
		Camera.main.transform.position = chaseBias * Camera.main.transform.position +
			(1.0f-chaseBias)*camChaseSpot;
		Camera.main.transform.LookAt(transform.position + transform.forward * 20.0f);

		speed -= transform.forward.y;
		if(speed < 50.0f) {
			speed = 50.0f;
		}
		float controlEffect = speed/120.0f;

		if(transform.position.y < Terrain.activeTerrain.SampleHeight(transform.position)) {
			speed = 0.0f;
			transform.position =
				new Vector3(transform.position.x,
				Terrain.activeTerrain.SampleHeight(transform.position),
				            transform.position.z);
		}

		transform.position += transform.forward * Time.deltaTime * speed;
		transform.Rotate(controlEffect*Input.GetAxis("Vertical"),0.0f,
		                 -controlEffect*Input.GetAxis("Horizontal"));
	}
}
