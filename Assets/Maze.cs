using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
	EMPTY = 0,
	STARTER = 1,
	EXIT = 2,
	OTHERS = 3
}

public enum Direction {
	NORTH = 0,
	NORTH_EST = 1,
	SOUTH_EST = 2,
	SOUTH = 3,
	SOUTH_WEST = 4,
	NORTH_WEST = 5,
	NONE = 6
}

public class Cell {
	private Vector3 position;
	private List<Direction> walls;
	private List<Direction> paths;
	private RoomType type;
	private int distance;
	private bool visited;
	private int groupNumber;

	public Cell(Vector3 position) {
		this.position = position;
		walls = new List<Direction> ();
		paths = new List<Direction> ();
		for (int i = 0; i < 6; i++) {
			walls.Add((Direction)i);
		}
		type = RoomType.EMPTY;
		ResetValues ();
	}

	public Vector3 GetPosition() {
		return position;
	}

	public Vector3 GetRealPosition() {
		return new Vector3 (pos.x * 3 / 2, 0, (pos.y - pos.z) * Mathf.Sqrt(3) / 2);
	}

	public Vector3 GetWallPosition(Direction direction) {
		Vector3 cellPosition = GetRealPosition ();
		if (direction == Direction.NORTH) {
			return new Vector3(cellPosition.x, 0, cellPosition.z + Mathf.Sqrt (3) / 2);
		} else if (direction == Direction.NORTH_EST) {
			return new Vector3(cellPosition.x + 6 / 8, 0, cellPosition.z + Mathf.Sqrt (3) / 4);
		} else if (direction == Direction.SOUTH_EST) {
			return new Vector3(cellPosition.x + 6 / 8, 0, cellPosition.z - Mathf.Sqrt (3) / 4);
		} else if (direction == Direction.SOUTH) {
			return new Vector3(cellPosition.x, 0,cellPosition.z - Mathf.Sqrt (3) / 2);
		} else if (direction == Direction.SOUTH_WEST) {
			return new Vector3(cellPosition.x - 6 / 8, 0, cellPosition.z - Mathf.Sqrt (3) / 4);
		} else if (direction == Direction.NORTH_WEST) {
			return new Vector3(cellPosition.x - 6 / 8, 0, cellPosition.z + Mathf.Sqrt (3) / 4);
		}
		return Vector3.zero;
	}

	public Vector3 GetWallRotation(Direction direction) {
		if (direction == Direction.NORTH) {
			return new Vector3(0, 180, 0);
		} else if (direction == Direction.NORTH_EST) {
			return new Vector3(0, -120, 0);
		} else if (direction == Direction.SOUTH_EST) {
			return new Vector3(0, -60, 0);
		} else if (direction == Direction.SOUTH) {
			return Vector3.zero;
		} else if (direction == Direction.SOUTH_WEST) {
			return new Vector3(0, 60, 0);
		} else if (direction == Direction.NORTH_WEST) {
			return new Vector3(0, 120, 0);
		}
		return Vector3.zero;
	}

	public void RemoveWall(Direction direction) {
		walls.Remove (direction);
		paths.Add (direction);
	}

	public List<Direction> GetWalls() {
		return walls;
	}

	public Direction GetRandomWall() {
		return walls[Random.Range(0, walls.Count)];
	}

	public List<Direction> GetPaths() {
		return paths;
	}

	public List<Direction> GetRandomPath() {
		return paths[Random.Range(0, paths.Count)];
	}

	public RoomType GetType() {
		return type;
	}

	public void SetType(RoomType _type) {
		type = _type;
	}

	public void ResetValues () {
		distance = -1;
		visited = false;
	}

	public void SetDistance(int _distance) {
		distance = _distance;
	}

	public int GetDistance() {
		return distance;
	}

	public void Visit() {
		visited = true;
	}

	public bool IsVisited() {
		return visited;
	}

	public void SetGroupNumber(int _groupNumber) {
		this.groupNumber = _groupNumber;
	}

	public int GetGroupNumber() {
		return groupNumber;
	}
}

public class Maze {

	protected List<Cell> maze;
	protected int size;

	public Maze(int size) {
		this.size = size;
		InitMaze ();
	}

	public Cell GetCellByPosition(Vector3 position) {
		return maze.Find ((Cell obj) => obj.GetPosition() == position);
	}

	public Cell GetCellByRealPosition(Vector3 position) {
//		float q = 2 * position.x / 3;
//		float r = -1 * position.x / 3 + Mathf.Sqrt (3) * position.z / 3;
//		return ;
	}

	public Cell GetCellByDirection(Cell cell, Direction direction) {
		if (direction == Direction.NORTH) {
			return GetCellByPosition (cell.GetPosition() + Vector3.back + Vector3.up);
		} else if (direction == Direction.NORTH_EST) {
			return GetCellByPosition (cell.GetPosition() + Vector3.right + Vector3.back);
		} else if (direction == Direction.SOUTH_EST) {
			return GetCellByPosition (cell.GetPosition() + Vector3.right + Vector3.down);
		} else if (direction == Direction.SOUTH) {
			return GetCellByPosition (cell.GetPosition() + Vector3.forward + Vector3.down);
		} else if (direction == Direction.SOUTH_WEST) {
			return GetCellByPosition (cell.GetPosition() + Vector3.left + Vector3.forward);
		} else if (direction == Direction.NORTH_WEST) {
			return GetCellByPosition (cell.GetPosition() + Vector3.left + Vector3.up);
		} else {
			return null;
		}
	}

	public List<Cell> GetCellsByUpperDistance(Cell cell, int minDistance) {
		Vector3 cellPosition = cell.GetPosition();
		return maze.FindAll ((Cell obj) => {
			Vector3 objPosition = obj.GetPosition();
			return (Mathf.Abs(cellPosition.x - objPosition.x) +
				Mathf.Abs(cellPosition.y - objPosition.y) +
				Mathf.Abs(cellPosition.z - objPosition.z)) / 2 >= minDistance;
		});
	}

	public List<Cell> GetCellsByBelowDistance(Cell cell, int maxDistance) {
		Vector3 cellPosition = cell.GetPosition();
		return maze.FindAll (delegate(Cell obj) {
			Vector3 objPosition = obj.GetPosition();
			return (Mathf.Abs(cellPosition.x - objPosition.x) +
				Mathf.Abs(cellPosition.y - objPosition.y) +
				Mathf.Abs(cellPosition.z - objPosition.z)) / 2 <= maxDistance;
		});
	}

	public List<Cell> GetNeighbourCells(Cell cell) {
		List<Cell> cells = new List<Cell>();
		List<Direction> directions = cell.GetPaths ();
		directions.ForEach (delegate(Direction obj) {
			Cell tmpCell = GetCellByDirection(cell, obj);
			if (tmpCell != null) {
				cells.Add(tmpCell);
			}
		});
		return cells;
	}

	public int GetDistanceBetweenTwoCells(Cell cell1, Cell cell2) {
		ResetMazeValues ();
		List<Cell> currentCells = new List<Cell> ();
		currentCells.Add (cell1);
		int currentDistance = 0;
		cell1.Visit ();
		cell1.SetDistance (currentDistance);
		while (!currentCells.Contains(cell2)) {
			List<Cell> tmpCells = new List<Cell>();
			currentCells.ForEach(delegate(Cell cell) {
				List<Cell> neighbours = GetNeighbourCells(cell);
				List<Cell> pathsNotVisited = neighbours.FindAll(delegate(Cell neighbour) {
					return !neighbour.IsVisited();
				});
				tmpCells.AddRange(pathsNotVisited);
			});
			currentDistance++;
			currentCells = tmpCells;
			currentCells.ForEach (delegate(Cell obj) {
				obj.Visit();
				obj.SetDistance(currentDistance);
			});
		}
		return currentDistance;
	}

	public Direction GetReverseDirection(Direction direction) {
		if (direction == Direction.NONE) {
			return direction;
		} else if ((int)direction < 3) {
			return direction + 3;
		} else {
			return direction - 3;
		}
	}

	public void InitMaze() {
		maze = new List<Cell> ();
		for (int x = -size; x <= size; x++) {
			for (int y = -size; y <= size; y++) {
				for (int z = -size; z <= size; z++) {
					if (x + y + z == 0) {
						maze.Add (new Cell(new Vector3(x, y, z)));
					}
				}
			}
		}
	}

	public void CreateMaze() {
		List<int> groupIndexes;
		for (int i = 0; i < maze.Count; i++) {
			maze [i].SetGroupNumber (i);
			groupIndexes.Add (i);
		}
		while (groupIndexes.Count > 1) {
			List<Cell> currentGroup = GetCellsByGroupNumber (groupIndexes[Random.Range (0, groupIndexes.Count)]);
			// todo continue
		}
	}

	private void ResetMazeValues() {
		maze.ForEach (delegate(Cell obj) {
			obj.ResetValues();
		});
	}

	private List<Cell> GetCellsByGroupNumber(int groupNumber) {
		return maze.FindAll (delegate(Cell obj) {
			return obj.GetGroupNumber() == groupNumber;
		});
	}
}
