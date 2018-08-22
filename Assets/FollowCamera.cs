using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {

	public GameObject target;

	
	// Update is called once per frame
	void Update () {
		Camera.main.transform.position = target.transform.position + Vector3.up * 4;
	}
}
