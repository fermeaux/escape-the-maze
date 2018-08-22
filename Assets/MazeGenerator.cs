using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
	EMPTY = 0,
	STARTER = 1,
	EXIT = 2,
	OTHERS = 3
}

public enum WallDirection {
	NORTH = 0,
	NORTH_EST = 1,
	SOUTH_EST = 2,
	SOUTH = 3,
	SOUTH_WEST = 4,
	NORTH_WEST = 5,
	NONE = 6
}

public class Cell {
	public Vector3 pos;
	public bool visited;
	public List<WallDirection> walls;
	public RoomType type;

	public Cell(int x, int y, int z) {
		pos = new Vector3 (x, y, z);
		visited = false;
		walls = new List<WallDirection> ();
		for (int i = 0; i < 6; i++) {
			walls.Add((WallDirection)i);
		}
		type = RoomType.EMPTY;
	}
}

public class MazeGenerator : MonoBehaviour {

	public float cellScale = 1f;
	public int mazeSize = 10;
	public GameObject wallPrefab;
	public GameObject startPrefab;
	public GameObject endPrefab;
	public GameObject enemyPrefab;
	public List<Cell> maze;
	public Cell enemyStartCell;

	public void Start() {
		Random.InitState ((int)System.DateTime.Now.Ticks);
		Generate ();
	}

	public List<Cell> Generate() {
		InitMaze ();
		LaunchPerfectMaze ();
		MakeMazeHarder ();
		for (int i = 0; i < maze.Count; i++) {
			for (int j = 0; j < maze[i].walls.Count; j++) {
				Vector2 pos = GetWallPositionInGame(maze[i], maze[i].walls[j]);
				float rot = GetWallRotationInGame(maze[i].walls[j]);
				Instantiate(wallPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.Euler(0, rot, 0), transform);
			}
		}
		CreateEnemy ();
		return maze;
	}

	private void MakeMazeHarder() {
		for (int i = 0; i < maze.Count; i++) {
			if (maze[i].walls.Count == 5) {
				List<WallDirection> walls = new List<WallDirection> ();
				maze [i].walls.ForEach ((WallDirection obj) => {
					walls.Add(obj);
				});
				while (walls.Count != 0) {
					int rand = Random.Range (0, walls.Count);
					if (GetCellByDirection (maze [i], walls [rand]) != null) {
						RemoveWallByDirection (maze [i], walls [rand]);
						break;
					} else {
						walls.RemoveAt (rand);
					}
				}
			}
		}
		Cell startCell = GetCellByPosition (Vector3.zero);
		while (startCell.walls.Count > 0) {
			RemoveWallByDirection(startCell, startCell.walls[0]);
		}
	}

	private void InitMaze() {
		maze = new List<Cell> ();
		for (int x = -mazeSize; x <= mazeSize; x++) {
			for (int y = -mazeSize; y <= mazeSize; y++) {
				for (int z = -mazeSize; z <= mazeSize; z++) {
					if (x == -y - z) {
						maze.Add(new Cell(x, y, z));
					}
				}
			}
		}
		Cell startCell = GetCellByPosition (Vector3.zero);
		startCell.type = RoomType.STARTER;
		GameObject player = Instantiate (startPrefab, Vector3.zero, Quaternion.identity);
		Camera.main.GetComponent<FollowCamera> ().target = player;
		SetRandomExitCell ();
	}

	private void SetRandomExitCell() {
		int rand = Random.Range (0, 3);
		int xPos = 0;
		int yPos = 0;
		int zPos = 0;
		if (rand == 0) {
			xPos = Random.Range (-mazeSize / 2, mazeSize / 2 + 1);
			xPos += (xPos < 0 ? -mazeSize / 2 : mazeSize / 2);
			rand = Random.Range (0, 2);
			if (rand == 0) {
				yPos = (xPos < 0 ? Random.Range (-mazeSize - xPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - xPos));
				zPos = -xPos - yPos;
			} else if (rand == 1) {
				zPos = (xPos < 0 ? Random.Range (-mazeSize - xPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - xPos));
				yPos = -xPos - zPos;
			}
		} else if (rand == 1) {
			yPos = Random.Range (-mazeSize / 2, mazeSize / 2 + 1);
			yPos += (yPos < 0 ? -mazeSize / 2 : mazeSize / 2);
			rand = Random.Range (0, 2);
			if (rand == 0) {
				xPos = (yPos < 0 ? Random.Range (-mazeSize - yPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - yPos));
				zPos = -xPos - yPos;
			} else if (rand == 1) {
				zPos = (yPos < 0 ? Random.Range (-mazeSize - yPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - yPos));
				xPos = -yPos - zPos;
			}
		} else if (rand == 2) {
			zPos = Random.Range (-mazeSize / 2, mazeSize / 2 + 1);
			zPos += (zPos < 0 ? -mazeSize / 2 : mazeSize / 2);
			rand = Random.Range (0, 2);
			if (rand == 0) {
				xPos = (zPos < 0 ? Random.Range (-mazeSize - zPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - zPos));
				yPos = -xPos - zPos;
			} else if (rand == 1) {
				yPos = (zPos < 0 ? Random.Range (-mazeSize - zPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - zPos));
				xPos = -yPos - zPos;
			}
		}
		Cell exitCell = GetCellByPosition (new Vector3(xPos, yPos, zPos));
		exitCell.type = RoomType.EXIT;
		Vector2 endPos = GetCellPositionInGame (exitCell);
		Instantiate (endPrefab, new Vector3(endPos.x, 0, endPos.y), Quaternion.identity);
	}

	private void CreateEnemy() {
		int rand = Random.Range (0, 3);
		int xPos = 0;
		int yPos = 0;
		int zPos = 0;
		if (rand == 0) {
			xPos = Random.Range (-mazeSize / 2, mazeSize / 2 + 1);
			xPos += (xPos < 0 ? -mazeSize / 2 : mazeSize / 2);
			rand = Random.Range (0, 2);
			if (rand == 0) {
				yPos = (xPos < 0 ? Random.Range (-mazeSize - xPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - xPos));
				zPos = -xPos - yPos;
			} else if (rand == 1) {
				zPos = (xPos < 0 ? Random.Range (-mazeSize - xPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - xPos));
				yPos = -xPos - zPos;
			}
		} else if (rand == 1) {
			yPos = Random.Range (-mazeSize / 2, mazeSize / 2 + 1);
			yPos += (yPos < 0 ? -mazeSize / 2 : mazeSize / 2);
			rand = Random.Range (0, 2);
			if (rand == 0) {
				xPos = (yPos < 0 ? Random.Range (-mazeSize - yPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - yPos));
				zPos = -xPos - yPos;
			} else if (rand == 1) {
				zPos = (yPos < 0 ? Random.Range (-mazeSize - yPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - yPos));
				xPos = -yPos - zPos;
			}
		} else if (rand == 2) {
			zPos = Random.Range (-mazeSize / 2, mazeSize / 2 + 1);
			zPos += (zPos < 0 ? -mazeSize / 2 : mazeSize / 2);
			rand = Random.Range (0, 2);
			if (rand == 0) {
				xPos = (zPos < 0 ? Random.Range (-mazeSize - zPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - zPos));
				yPos = -xPos - zPos;
			} else if (rand == 1) {
				yPos = (zPos < 0 ? Random.Range (-mazeSize - zPos, mazeSize + 1) : Random.Range(-mazeSize, mazeSize + 1 - zPos));
				xPos = -yPos - zPos;
			}
		}
		enemyStartCell = GetCellByPosition (new Vector3(xPos, yPos, zPos));
		Debug.Log ("enemy cell : " + enemyStartCell.pos);
		Vector2 endPos = GetCellPositionInGame (enemyStartCell);
		Instantiate (enemyPrefab, new Vector3 (endPos.x, 0, endPos.y), Quaternion.identity);
	}

	private void LaunchPerfectMaze() {
		Cell initialCell = GetCellByPosition (Vector3.zero);
		initialCell.visited = true;
		RecursiveBacktracer (initialCell);
	}

	private void RecursiveBacktracer(Cell currentCell) {
		WallDirection nextDir;
		while ((nextDir = GetRandomUnvisitedCellDirectionAround (currentCell)) != WallDirection.NONE) {
			RemoveWallByDirection (currentCell, nextDir);
			Cell nextCell = GetCellByDirection (currentCell, nextDir);
			nextCell.visited = true;
			RecursiveBacktracer (nextCell);
		}
	}

	private void RemoveWallByDirection(Cell currentCell, WallDirection dir) {
		currentCell.walls.Remove (dir);
		Cell nextCell = GetCellByDirection (currentCell, dir);
		nextCell.walls.Remove (GetReverseDirection (dir));
	}

	private WallDirection GetRandomUnvisitedCellDirectionAround(Cell currentCell) {
		List<WallDirection> cells = GetUnvisitedCellsDirectionAround (currentCell);
		if (cells.Count == 0) {
			return WallDirection.NONE;
		} else {
			int rand = Random.Range (0, cells.Count);
			return cells[rand];
		}
	}

	private List<WallDirection> GetUnvisitedCellsDirectionAround(Cell currentCell) {
		List<WallDirection> unvisitedCellsDirectionAround = new List<WallDirection> ();

		Cell northCell = GetCellByDirection(currentCell, WallDirection.NORTH);
		Cell northEstCell = GetCellByDirection(currentCell, WallDirection.NORTH_EST);
		Cell southEstCell = GetCellByDirection(currentCell, WallDirection.SOUTH_EST);
		Cell southCell = GetCellByDirection(currentCell, WallDirection.SOUTH);
		Cell southWestCell = GetCellByDirection(currentCell, WallDirection.SOUTH_WEST);
		Cell northWestCell = GetCellByDirection(currentCell, WallDirection.NORTH_WEST);

		if (northCell != null && !northCell.visited) {
			unvisitedCellsDirectionAround.Add (WallDirection.NORTH);
		} if (northEstCell != null && !northEstCell.visited) {
			unvisitedCellsDirectionAround.Add (WallDirection.NORTH_EST);
		} if (southEstCell != null && !southEstCell.visited) {
			unvisitedCellsDirectionAround.Add (WallDirection.SOUTH_EST);
		} if (southCell != null && !southCell.visited) {
			unvisitedCellsDirectionAround.Add (WallDirection.SOUTH);
		} if (southWestCell != null && !southWestCell.visited) {
			unvisitedCellsDirectionAround.Add (WallDirection.SOUTH_WEST);
		} if (northWestCell != null && !northWestCell.visited) {
			unvisitedCellsDirectionAround.Add (WallDirection.NORTH_WEST);
		}

		return unvisitedCellsDirectionAround;
	}

	private Cell GetCellByPosition(Vector3 pos) {
		return maze.Find (cell => cell.pos == pos);
	}

	public Cell GetCellByDirection(Cell currentCell, WallDirection dir) {
		if (dir == WallDirection.NORTH) {
			return GetCellByPosition (currentCell.pos + Vector3.back + Vector3.up);
		} else if (dir == WallDirection.NORTH_EST) {
			return GetCellByPosition (currentCell.pos + Vector3.right + Vector3.back);
		} else if (dir == WallDirection.SOUTH_EST) {
			return GetCellByPosition (currentCell.pos + Vector3.right + Vector3.down);
		} else if (dir == WallDirection.SOUTH) {
			return GetCellByPosition (currentCell.pos + Vector3.forward + Vector3.down);
		} else if (dir == WallDirection.SOUTH_WEST) {
			return GetCellByPosition (currentCell.pos + Vector3.left + Vector3.forward);
		} else if (dir == WallDirection.NORTH_WEST) {
			return GetCellByPosition (currentCell.pos + Vector3.left + Vector3.up);
		} else {
			return null;
		}
	}

	private WallDirection GetReverseDirection(WallDirection dir) {
		if (dir == WallDirection.NONE) {
			return dir;
		} else if ((int)dir < 3) {
			return dir + 3;
		} else {
			return dir - 3;
		}
	}

	private Vector2 GetWallPositionInGame(Cell cell, WallDirection dir) {
		Vector2 cellPosition = GetCellPositionInGame (cell);
		if (dir == WallDirection.NORTH) {
			return new Vector2 (cellPosition.x, cellPosition.y + Mathf.Sqrt (3) * cellScale / 2);
		} else if (dir == WallDirection.NORTH_EST) {
			return new Vector2 (cellPosition.x + 6 * cellScale / 8, cellPosition.y + Mathf.Sqrt (3) * cellScale / 4);
		} else if (dir == WallDirection.SOUTH_EST) {
			return new Vector2 (cellPosition.x + 6 * cellScale / 8, cellPosition.y - Mathf.Sqrt (3) * cellScale / 4);
		} else if (dir == WallDirection.SOUTH) {
			return new Vector2 (cellPosition.x, cellPosition.y - Mathf.Sqrt (3) * cellScale / 2);
		} else if (dir == WallDirection.SOUTH_WEST) {
			return new Vector2 (cellPosition.x - 6 * cellScale / 8, cellPosition.y - Mathf.Sqrt (3) * cellScale / 4);
		} else if (dir == WallDirection.NORTH_WEST) {
			return new Vector2 (cellPosition.x - 6 * cellScale / 8, cellPosition.y + Mathf.Sqrt (3) * cellScale / 4);
		} else {
			return cellPosition;
		}
	}

	private float GetWallRotationInGame(WallDirection dir) {
		if (dir == WallDirection.NORTH) {
			return 180;
		} else if (dir == WallDirection.NORTH_EST) {
			return -120;
		} else if (dir == WallDirection.SOUTH_EST) {
			return -60;
		} else if (dir == WallDirection.SOUTH) {
			return 0;
		} else if (dir == WallDirection.SOUTH_WEST) {
			return 60;
		} else if (dir == WallDirection.NORTH_WEST) {
			return 120;
		}
		return 0;
	}

	public Vector2 GetCellPositionInGame(Cell cell) {
		return new Vector2 ((cell.pos.x * 3 / 2) * cellScale, ((cell.pos.y - cell.pos.z) * Mathf.Sqrt(3) / 2) * cellScale);
	}
}
