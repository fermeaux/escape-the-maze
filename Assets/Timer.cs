using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public float time = 0;
	private Text text;

	void Start() {
		text = GetComponent<Text> ();
	}

	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		text.text = ((int)time).ToString ();
	}
}
