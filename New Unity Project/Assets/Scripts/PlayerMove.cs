using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMove : NetworkBehaviour {

	float speed = 10f;

	void Start()
	{
		if (isLocalPlayer)
			gameObject.GetComponent<Renderer> ().material.color = Color.blue;
	}

	void Update()
	{
		if (!isLocalPlayer)
			return;
		
		transform.position += new Vector3 (Input.GetAxis ("Horizontal"), 0f, Input.GetAxis ("Vertical")) * speed * Time.deltaTime;
	}

}
