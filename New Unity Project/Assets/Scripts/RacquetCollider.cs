using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class RacquetCollider : NetworkBehaviour {

	public float accel;
	public GameObject rotateOb;
	public Transform shotPos;
	public float shotForce = 0.0f;
	// forward 스윙과 back 스윙이 겹치는 것을 방지
	public static bool isCollision = false;


	//When ball touched Racquet 
	void OnTriggerEnter(Collider ball){
		
		if (ball.gameObject.name.Contains("tennisball")&& RacquetCollider.isCollision == false) {
			GameObject shot = ball.gameObject;
			//우선 속도를 0으로
			shot.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			//Debug.Log ("Entered");
			//강도 조절(BSensorController 스크립트 참조) -> 네트워크 구성 때 rotate object 구별 필요
			rotateOb = GameObject.Find("rotateObject1");
			//accel = GameObect.Find("rotateObject2");
			accel=rotateOb.GetComponent<BSensorController>().getAcceleration();
			if (accel > 100f) {
				shotForce = 58f;
			} else if (accel > 70f) {
				shotForce = 50f;
			} else if (accel > 50f){
				shotForce = 40f;
			}else{
				shotForce = accel;
			}			
			Debug.Log (accel);
			//각도 조절(HorizonDegreeLimiter 스크립트 참조)
			if (HorizonDegreeLimiter.isInDegree == true) {
				shot.GetComponent<Rigidbody> ().AddForce (shotPos.up * shotForce);
			} else {
				var heading = GameObject.Find ("OverDegreeObject").transform.position - shotPos.position;
				var distance = heading.magnitude;
				var direction = heading / distance;
				shot.GetComponent<Rigidbody> ().AddForce (direction * shotForce);
			}
			RacquetCollider.isCollision = true;
		}
	}
}
