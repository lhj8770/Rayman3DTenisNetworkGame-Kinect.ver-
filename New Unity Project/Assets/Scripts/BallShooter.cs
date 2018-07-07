
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BallShooter : NetworkBehaviour {

	float timer;
	int waitingTime;
	public GameObject projectile;
	public Transform pos;
//	public float shotForce = 50f;
//	public float moveSpeed = 10f;

	void Start(){
		timer = 0.0f;
		waitingTime = 3;

	}
		
	void Update() {
//		timer += Time.deltaTime;
//
//		if (timer > waitingTime) {
//			Rigidbody shot = Instantiate (projectile, shotPos.position, shotPos.rotation) as Rigidbody;
//
//
//			timer = 0.0f;
//		}
//		bool a = GameObject.Find("Player 1").transform.Find("Body").Find("Stoma").Find("Main Camera").GetComponent<GestureListener>().IsStartPosition();
//		Debug.Log (GameObject.Find("Player 1").transform.Find("Body").Find("Stoma").Find("Main Camera").GetComponent<GestureListener>().IsStartPosition());
		if (isLocalPlayer) {
			if (GameObject.Find ("Player 1").transform.Find ("Body").Find ("Stoma").Find ("Main Camera").GetComponent<GestureListener> ().IsStartPosition ()) {
				CmdRequestSpawn ();
			}
		}

	}

	[Command]
	void CmdRequestSpawn(){
		var ball = (GameObject)Instantiate (projectile, pos.position, pos.rotation);

		NetworkServer.Spawn (ball);
	}
}
