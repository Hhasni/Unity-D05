using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIBarScript : MonoBehaviour {
	
	public Image		my_bar;
	public Vector3		v;
	public int 			moveSpeed;
	public float 		power;
	// Use this for initialization
	void Start () {
		v = Vector3.left;
		power = 0;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (my_bar.rectTransform.localPosition.x < -100)
			v = Vector3.right;
		else if (my_bar.rectTransform.localPosition.x > 0)
			v = Vector3.left;
		my_bar.rectTransform.Translate (v * moveSpeed * Time.deltaTime);
		power = 100 + my_bar.rectTransform.localPosition.x;
		if (power <= 0)
			power = 1;
		if (power >= 100)
			power = 100;
	}
}
