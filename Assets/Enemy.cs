using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float speed = 1;

	private MazeGenerator maze;
	private Cell lastCell;
	private Cell targetCell;
	private Vector3 targetPos;
	private Transform player;

	void Start() {
		maze = GameObject.FindObjectOfType<MazeGenerator> ();
		targetCell = maze.enemyStartCell;
		lastCell = targetCell;
		Vector2 pos = maze.GetCellPositionInGame (targetCell);
		targetPos = new Vector3 (pos.x, 0, pos.y);
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		Debug.Log ("oui");
	}

	void Update() {
		RaycastHit hit;
		if (Physics.Raycast (transform.position, player.position - transform.position, out hit) && hit.transform.tag == "Player") {
			Debug.Log ("Player find");
			Vector3 dir = player.position - transform.position;
			transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
		} else if (Vector3.Distance (targetPos, transform.position) <= 0.01f) {
			Debug.Log ("Distance is low");
			FindNewTarget ();
		} else {
			Vector3 dir = targetPos - transform.position;
			transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
		}
	}

	private void FindNewTarget() {
		List<WallDirection> dirPossibilities = new List<WallDirection> ();
		for (int i = 0; i < 6; i++) {
			dirPossibilities.Add ((WallDirection)i);
		}
		Debug.Log (targetCell == null);
		for (int j = 0; j < targetCell.walls.Count; j++) {
			Debug.Log ("current cell wall : " + targetCell.walls[j]);
			dirPossibilities.Remove (targetCell.walls[j]);
		}
		for (int k = 0; k < dirPossibilities.Count; k++) {
			Debug.Log ("possibilities dir : " + dirPossibilities[k]);
		}
		WallDirection selectedDirection = dirPossibilities[Random.Range (0, dirPossibilities.Count)];
		Cell selectedCell = maze.GetCellByDirection (targetCell, selectedDirection);
		if (selectedCell.pos == lastCell.pos) {
			Debug.Log ("change");
			dirPossibilities.Remove (selectedDirection);
			selectedDirection = dirPossibilities[Random.Range (0, dirPossibilities.Count)];
			selectedCell = maze.GetCellByDirection (targetCell, selectedDirection);
		}
		Debug.Log ("target dir : " + selectedDirection);
		lastCell = targetCell;
		targetCell = selectedCell;
		Vector2 pos = maze.GetCellPositionInGame (targetCell);
		targetPos = new Vector3 (pos.x, 0, pos.y);
	}
}
