using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float speed = 50;

	private Rigidbody rb;

	public void Start() {
		rb = GetComponent<Rigidbody> ();
	}

	void Update() {
		float verticalMove = Input.GetAxis ("Vertical") * Time.deltaTime * speed;
		float horizontalMove = Input.GetAxis ("Horizontal") * Time.deltaTime * speed;
		rb.velocity = new Vector3 (horizontalMove, 0, verticalMove);
	}
}
