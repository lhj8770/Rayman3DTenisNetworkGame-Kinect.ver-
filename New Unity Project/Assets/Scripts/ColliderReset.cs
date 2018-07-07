using UnityEngine;
using System.Collections;

public class ColliderReset : MonoBehaviour {

	void OnTriggerEnter(Collider ball){

		if (ball.gameObject.name.Contains("tennisball")) {
			
			RacquetCollider.isCollision = false;
		}
	}
}
