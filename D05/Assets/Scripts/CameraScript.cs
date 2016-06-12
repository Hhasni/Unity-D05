using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public CharacterController cc;
	public bool 		freecam;
	public int 			MoveSpeed;
	public int 			RotSpeed;

	public float 		rotationY;
	public float 		rotationX;
	public float 		sensitivityX = 15F;
	public float 		sensitivityY = 15F;
	public float 		minimumY = -60F;
	public float 		maximumY = 60F;
	public float 		MaxHeight;
	
	public GameObject	gGhost;
	public GameObject	gFlag;
	public GameObject	gArrow;
	private ShootScript	my_ShootScript;
	private GameObject 	gBall;
	private Vector3 	offset;
	private Vector3 	lastPos;

	void Start () {
		gBall = GameObject.FindGameObjectWithTag ("Ball");
		my_ShootScript = gBall.GetComponent<ShootScript> ();
		gFlag = my_ShootScript.gHole [0];
		freecam = false;
		MoveSpeed = 3;
		RotSpeed = MoveSpeed + 10;
		cc = GetComponent<CharacterController>();
		Cursor.visible = false;
		offset = transform.position - gBall.transform.position;
		gGhost.transform.LookAt (gBall.transform);
		transform.position = gGhost.transform.position;
		transform.rotation = gGhost.transform.rotation;
	}
	
	void moveVerticaly(){
		if (transform.position.y < MaxHeight) {
			if (Input.GetKey (KeyCode.E))
				cc.Move (transform.TransformDirection (Vector3.up) * MoveSpeed);
		}
		if (Input.GetKey (KeyCode.Q))
			cc.Move (transform.TransformDirection(Vector3.down) * MoveSpeed);
	}

	void moveLeftRight(){
		if (Input.GetKey (KeyCode.W))
			cc.Move (transform.TransformDirection(Vector3.forward) * MoveSpeed);
		if (Input.GetKey (KeyCode.S))
			cc.Move (transform.TransformDirection(Vector3.back) * MoveSpeed);
	}

	void moveForwardBackward(){
		if (Input.GetKey (KeyCode.A))
			cc.Move (transform.TransformDirection(Vector3.left) * MoveSpeed);
		if (Input.GetKey (KeyCode.D))
			cc.Move (transform.TransformDirection(Vector3.right) * MoveSpeed);
	}

	void RotateAround(){
		if (Input.GetKey (KeyCode.A)) {
			transform.LookAt (gBall.transform);
			gArrow.SetActive (true);
			StopAllCoroutines();
			cc.transform.Translate (Vector3.right * RotSpeed * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.D)) {
			transform.LookAt (gBall.transform);
			cc.transform.Translate (Vector3.left * RotSpeed * Time.deltaTime);
			gArrow.SetActive (true);
			StopAllCoroutines();
		}
		else
		 StartCoroutine(FadeOutArow ());
	}

	void MouseDirection(){
		float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
		transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
	}

	IEnumerator FadeOutArow(){
		yield return new WaitForSeconds (1.5f);
		gArrow.SetActive(false);
	}

	public void resetCam(){
		freecam = false;
		if (gFlag)
			gBall.transform.LookAt (gFlag.transform);
		transform.position = gGhost.transform.position;
		transform.rotation = gGhost.transform.rotation;
		//Vector3 targetDirection = gFlag.transform.position - transform.position;
		//Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 100f, .0f);
		
		//transform.LookAt(transform.position + newDirection);
		//transform.rotation = Quaternion.FromToRotation(Vector3.right, (gBall.transform.position - gFlag.transform.position));
		//transform.LookAt (gFlag.transform);
		lastPos = transform.position;
	}

	void NotFreeCam(){
		freecam = false;
	}

	void Update () {
		if (freecam) {
			if (transform.position.y > MaxHeight)
				transform.position = new Vector3 (transform.position.x, (MaxHeight - 0.5f), transform.position.z);
			moveVerticaly ();
			moveLeftRight ();
			moveForwardBackward ();
			MouseDirection ();
		} else if (!my_ShootScript.isShooting) {
		//	transform.position = gBall.transform.position + offset;
			//transform.LookAt (gBall.transform);
			RotateAround();
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			lastPos = transform.position;
			if (!freecam)
				freecam = true;
		}
		if (Input.GetKeyDown (KeyCode.Space) && freecam) {
			transform.position = lastPos;
			Invoke ("NotFreeCam", 0.2f);
		}
		if (transform.position.y <= -20)
			transform.position = lastPos;
	}
}