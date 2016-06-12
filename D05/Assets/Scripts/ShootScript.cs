using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShootScript : MonoBehaviour {
	
	public bool				GameOver;
	public bool				isShooting;
	public bool  			isCharging;
	public bool				isIN;
	public bool				isGreen;
	public bool				isOut;
	public bool  			isReady;
	public bool  			isScoreLoaded;
	public GameObject 		gOut;
	public GameObject 		gPanel;
	public GameObject 		gArrow;
	public GameObject[]		gFlag;
	public GameObject[]		gHole;
	public enum Type {Wood, Iron, Wedge, Putter};
	public Type 			type;
	public int 				holeNbr;
	public int 				TotalScore;
	public GameObject 		gPlayer;
	public GameObject 		gCurrentFlag;
	private Rigidbody 		rbBall;
	private int 			nbShoot;
	private int 			nbType;
	public float 			velLoss;
	public float 			DragInc;
	public float 			timer;
	public float 			Dragtimer;
	public CameraScript 	PlayerCameraS;
	public float			Power;
	public FlagScript 		sFlag;
	public Vector3[]		myPos;
	public Vector3			lastPos;
	public Text 			tHole;
	public Text 			tShoot;
	public Text 			tType;
	public GameObject		gPanelScore;
	public GameObject		gTabScore;

	// Use this for initialization
	void Start () {
		nbType = 0;
		type = Type.Wood;
		nbShoot = 0;
		holeNbr = 0;
		isReady = true;
		velLoss = 0.1f;
		rbBall = GetComponent<Rigidbody> ();
		gPlayer = GameObject.FindGameObjectWithTag("Player");
		PlayerCameraS = gPlayer.GetComponent<CameraScript>();
		gCurrentFlag = gFlag [holeNbr];
		sFlag = gCurrentFlag.GetComponentInChildren<FlagScript> ();
		lastPos = myPos [0];
	}

	void OnCollisionEnter(Collision col){
		if (isShooting) {
			if (Dragtimer > 1f) {
				Dragtimer = 0;
				if (type == Type.Wedge)
					rbBall.drag += 3.9f;
				rbBall.drag += 0.1f;
			}
		}
	}

	void ft_PreparForNextShoot(){
		rbBall.mass = 1.5f;
		rbBall.velocity = Vector3.zero;
		isShooting = false;
		rbBall.isKinematic = true;
		rbBall.drag = 0.1f;
		isReady = true;
		transform.rotation = Quaternion.identity;
		PlayerCameraS.resetCam();
		lastPos = transform.position;
	}

	void Fadeout(){
		gOut.SetActive (false);
		transform.position = lastPos;
		isOut = false;
	}

	void ft_penality(){
		gOut.SetActive(true);
		isOut = true;
		Invoke("Fadeout", 4);
	}

	void ft_setGreen(){
		isGreen = true;
		nbType = 2;
		type = Type.Putter;
		tType.text = "Putter";
	}

	void OnTriggerEnter(Collider col){
		if (rbBall.drag <= 0.9f && type != Type.Putter) {
			Debug.Log ("Triggger");
			rbBall.drag += 0.9f;
		}
		if (col.gameObject.tag == "aqua") {
			ft_penality();
		}
		if (col.gameObject.tag == "Green")
			ft_setGreen ();
	}

	void OnTriggerExit(Collider col){
		if (col.gameObject.tag == "Green") {
			isGreen = false;
		}
	}
	
	void OnCollisionStay(Collision col){
		if (timer >= 5 &&
		    rbBall.velocity.x >= -0.3f && rbBall.velocity.x <= 0.3f 
		    && rbBall.velocity.y >= -0.3f && rbBall.velocity.y <= 0.3f
		    && rbBall.velocity.z >= -0.3f && rbBall.velocity.z <= 0.3f){
			if (!isOut)
				ft_PreparForNextShoot();
		}
		if (type != Type.Putter)
			rbBall.drag += 0.001f;
		if (timer > 15)
			rbBall.drag += 0.05f;
		timer += Time.deltaTime;
	}

	void ft_loadNextFlag (){
		isGreen = false;
		gCurrentFlag = gFlag [holeNbr];
		sFlag = gCurrentFlag.GetComponent<FlagScript> ();
		transform.position = myPos [holeNbr];
		PlayerCameraS.gFlag = gHole [holeNbr];
		ft_PreparForNextShoot();
		isIN = false;
		isScoreLoaded = false;
		nbShoot = 0;
		nbType = 0;
		type = Type.Wood;
		tShoot.text = "";
		tType.text = "Wood";
		tHole.text = "Trou " + (holeNbr + 1).ToString ();
		gPanelScore.SetActive (false);
		transform.rotation = Quaternion.identity;
		lastPos = transform.position;
	}

	void ft_ScoreLoad(){
		if (!isScoreLoaded) {
			gPanelScore.SetActive (true);
			isScoreLoaded = true;
			Text[] tmp = gPanelScore.GetComponentsInChildren<Text>();
			gTabScore.SetActive (true);
			Text[] tScore = gTabScore.GetComponentsInChildren<Text>();
			int scoretmp = -3 + nbShoot;
			TotalScore += scoretmp;
			tmp[0].text = scoretmp.ToString();
			Debug.Log (holeNbr);
			tScore[holeNbr].text = scoretmp.ToString();
			holeNbr+= 1;
			if (GameOver){
				tScore[holeNbr].text = TotalScore.ToString();
				gPanelScore.SetActive(false);
				return ;
			}
			gTabScore.SetActive(false);
			tmp[1].text = "";
			if (nbShoot == 1)
				tmp[1].text = "Ace";
			else {
				switch (scoretmp){
					case -3:
						tmp[1].text = "Albatross";
						break;
					case -2:
						tmp[1].text = "Eagle";
						break;
					case -1:
						tmp[1].text = "Birdie";
						break;
					case 0:
						tmp[1].text = "Par";
						break;
					case 1:
						tmp[1].text = "Bogey";
						break;
					case 2:
						tmp[1].text = "Double Bogey";
						break;
					case 3:
						tmp[1].text = "Triple Bogey";
						break;
				}
			}
		}
	}

	void 	setType(){
		if (nbType == 0) {
			type = Type.Wood;
			tType.text = "Wood";
		}
		if (nbType == 1) {
			type = Type.Iron;
			tType.text = "Iron";
		}
		if (nbType == 2) {
			type = Type.Wedge;
			tType.text = "Wedge";
		}
	//	if (nbType == 3) {
	//		type = Type.Putter;
	//		tType.text = "Putter";
	///	}
	}

	void 	ft_choose_type(){
		if (!isGreen) {
			if (Input.GetKeyDown (KeyCode.Equals)) {
				nbType += 1;
				if (nbType > 2)
					nbType = 0;
				setType ();
			}
			if (Input.GetKeyDown (KeyCode.Minus)) {
				nbType -= 1;
				if (nbType < 0)
					nbType = 3;
				setType ();
			}
		}
	}

	void ft_wedge_mod(){
		rbBall.mass = 10;
		rbBall.drag = 0;
	}

	// Update is called once per frame
	void Update () {
		if (transform.position.y < -2)
			ft_penality ();
		if (!isCharging && isReady)
			ft_choose_type ();
		if (Input.GetKeyDown (KeyCode.Tab)) {
			gTabScore.SetActive(!gTabScore.activeSelf);
		}
		if (Input.GetKeyDown (KeyCode.Space) && !PlayerCameraS.freecam) {
			if (!isCharging && isReady){
				gPanel.SetActive(true);
				isCharging = true;
			}
			else if (isCharging && isReady){
				Power = gPanel.GetComponentInChildren<GUIBarScript>().power;
				Debug.Log("POWER = " + Power);
				gPanel.SetActive(false);
				nbShoot += 1;
				tShoot.text = "Shoot " + nbShoot.ToString();
				isShooting = true;
				isReady = false;
				isCharging = false;
				timer = 0;
				rbBall.isKinematic = false;
				if (type == Type.Wood){
					Vector3 tmp = new Vector3 (transform.position.x, transform.position.y, transform.position.z) - gPlayer.transform.position;
					rbBall.AddForce(new Vector3(tmp.x, tmp.y + (Power/5), tmp.z) * (Power/ 32), ForceMode.Impulse);
				}
				else if (type == Type.Iron){
					Vector3 tmp = new Vector3 (transform.position.x, transform.position.y, transform.position.z) - gPlayer.transform.position;
					rbBall.AddForce(new Vector3(tmp.x, tmp.y + (Power/ 10) , tmp.z ) * (Power/ 25)  , ForceMode.Impulse);
				}
				else if (type == Type.Wedge){
					Invoke ("ft_wedge_mod", 0.2f);
					rbBall.mass = 10;
					rbBall.drag = 0;
					Vector3 tmp = new Vector3 (transform.position.x, transform.position.y, transform.position.z) - gPlayer.transform.position;
					tmp = tmp * (Power /20);
					rbBall.AddForce(new Vector3(tmp.x , tmp.y + (Power*4),tmp.z), ForceMode.Impulse);
				}
				else if (type == Type.Putter){
					rbBall.mass = 1.5f;
					rbBall.drag = 0.1f;
					Vector3 tmp = new Vector3 (transform.position.x, transform.position.y, transform.position.z) - gPlayer.transform.position;
					rbBall.AddForce(new Vector3(tmp.x * (Power/40), tmp.y, tmp.z * (Power/ 40)), ForceMode.Impulse);
				}
			}
		}
		if (isIN){
			ft_ScoreLoad();
			if (!GameOver && Input.GetKeyDown(KeyCode.Return))
				ft_loadNextFlag ();
		}
		Dragtimer += Time.deltaTime;
	}
}
