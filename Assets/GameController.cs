using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public bool tutorialActive = false;
	public int worldX = 2000;
	public int worldZ = 2000;

	public Transform hoop;

	// Use this for initialization
	void Start () {
		spawnHoop();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void spawnHoop() {
		float x, z;
		x = (Random.value * worldX);
		z = (Random.value * worldX);

		//Generate y coordinate to make sure it stays above the terrain.
		Vector3 pos = transform.position;
		pos.x = x;
		pos.z = z;
		pos.y = Terrain.activeTerrain.SampleHeight(transform.position);

		pos.y += Random.value * 175 + 25;

		Instantiate(hoop, pos, Quaternion.identity);

	}
}
