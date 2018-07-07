using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class NetColliderEvent :MonoBehaviour{	
	public int Bounding;

	void OnTriggerEnter(Collider Ball){
		if (Ball.gameObject.name.Contains ("tennisball")) {
			//
			Debug.Log("dd");
			Destroy(Ball.gameObject);
		}
	}
}
