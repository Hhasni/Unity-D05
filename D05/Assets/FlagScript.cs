using UnityEngine;
using System.Collections;

public class FlagScript : MonoBehaviour {

	public int hole;
	public bool Finish;

	// Use this for initialization
	void Start () {
		Finish = false;
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Ball") {
			if (hole == col.gameObject.GetComponent<ShootScript> ().holeNbr) {
				Finish = true;
				if (hole == 2)
					col.gameObject.GetComponent<ShootScript> ().GameOver = true;
				col.gameObject.GetComponent<ShootScript> ().isIN = true;
				Destroy(gameObject);
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
