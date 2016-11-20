using UnityEngine;
using System.Collections;

public class HoopScript : MonoBehaviour {

	public GameObject plane;
	public bool isCollected;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (isCollected) {
			transform.localScale += new Vector3(-0.5F, 0, -0.5F);
		
			CanvasGroup cg = GetComponents<CanvasGroup>()[0];
			cg.alpha -= 0.02f;

			if (cg.alpha <= 0.0) {
				Destroy(this.gameObject);
			}
		}

		if (Vector3.Distance( plane.transform.position, transform.position) > 10 ) {
			transform.LookAt(plane.transform);
		}



		transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

	}

	void OnTriggerEnter(Collider coll) {
		isCollected = true;

	}
}
