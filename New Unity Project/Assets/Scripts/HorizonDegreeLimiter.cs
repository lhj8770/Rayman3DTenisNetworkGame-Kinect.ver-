using UnityEngine;
using System.Collections;

public class HorizonDegreeLimiter : MonoBehaviour {

	public static bool isInDegree = false;
	private GameObject now=null;

	void OnTriggerStay(Collider Other){
		if (Other.gameObject.name.Contains ("SwingLimit")) {
			now = Other.gameObject;
			isInDegree = true;
			//Debug.Log ("stay");
		}
	}

	void OnTriggerExit(Collider Other){
		if (Other.gameObject.name==now.name) {
			isInDegree = false;
			//Debug.Log ("end");
		}
	}
	
}
