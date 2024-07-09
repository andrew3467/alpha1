using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {
    Vector2Int[] Neighbors = new [] {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
    };
    
    const float WALL_SIZE_X = 8f;
    const float WALL_SIZE_Y = 8f;

    [SerializeField] Transform parent;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject floorPrefab;

    [Space(20)]
    [SerializeField] int maxCellsCount = 512;
    [SerializeField] int delay = 1;

    Dictionary<Vector2Int, MazeCell> mazeCells;
    public List<Vector2Int> frontierCells;

    MazeCell prevCell;

    System.Random prng;
    bool firstRun = true;

    void Start() {
        mazeCells = new Dictionary<Vector2Int, MazeCell>();
        frontierCells = new List<Vector2Int>();

        prng = new System.Random();
        
    }
    
    Vector2Int GetRandomCell() {
        return frontierCells[prng.Next(0, frontierCells.Count)];
    }

    public void StartPrims() {
        print('1');
        StartCoroutine(RunPrims());
        
    }


    IEnumerator RunPrims() {
        bool running = true;
        while (running) {
            StepThroughPrims();
            print(mazeCells.Count);
            yield return new WaitForSeconds(delay);

            running = mazeCells.Count < maxCellsCount;
        }
    }

    public void StepThroughPrims() {
        Vector2Int selection;
        bool firstRun = false;

        if (frontierCells.Count == 0) {
            selection = Vector2Int.zero;
        } else {
            selection = GetRandomCell();

            while (mazeCells.ContainsKey(selection)) {
                selection = GetRandomCell();
            }
            
            frontierCells.Remove(selection);
        }

            MazeCell cell = new MazeCell(selection, floorPrefab, parent);
        cell.AddWall(Facing.North, wallPrefab);
        cell.AddWall(Facing.South, wallPrefab);
        cell.AddWall(Facing.East, wallPrefab);
        cell.AddWall(Facing.West, wallPrefab);
        
        
        
        mazeCells.Add(selection, cell);

        //None is not on edge
        for (int i = 0; i < Neighbors.Length; i++) {
            Facing neighborFacing = (Facing)i;
            var neighborPos = selection + Neighbors[i];

            if (!mazeCells.ContainsKey(neighborPos)) {
                frontierCells.Add(neighborPos);
            }
        }

        if (prevCell != null) {
            RemoveWalls(cell, mazeCells[RandomNeighbor(cell)]);
        }

        prevCell = cell;

    }

    Vector2Int RandomNeighbor(MazeCell cell) {
        var neighbor = cell.Position + Neighbors[prng.Next(0, Neighbors.Length)];

        while (!mazeCells.ContainsKey(neighbor)) {
            neighbor = cell.Position + Neighbors[prng.Next(0, Neighbors.Length)];
        }

        return neighbor;
    }
    
    void RemoveWalls(MazeCell cell, MazeCell other) {
        Facing direction = FacingFromOffset(cell.Position - other.Position);

        switch (direction) {
            case Facing.North:
                other.RemoveWall(Facing.North);
                cell.RemoveWall(Facing.South);
                break;
            case Facing.South:
                other.RemoveWall(Facing.South);
                cell.RemoveWall(Facing.North);
                break;
            case Facing.East:
                other.RemoveWall(Facing.East);
                cell.RemoveWall(Facing.West);
                break;
            case Facing.West:
                other.RemoveWall(Facing.West);
                cell.RemoveWall(Facing.East);
                break;
        }
    }
    
    Facing FacingFromOffset(Vector2Int prevCellPosition) {
        for (int i = 0; i < Neighbors.Length; i++) {
            if (Neighbors[i] == prevCellPosition) return (Facing)i;
        }
        return Facing.None;
    }

    public enum Facing
    {
        None = -1,
        North = 0,
        South = 1,
        East = 2,
        West = 3
    }
    
    public class MazeCell
    {
        public GameObject[] Walls;
        public GameObject Floor;
        public Vector2Int Position;

        public MazeCell(Vector2Int pos, GameObject floorPrefab, Transform parent)
        {
            Position = pos;
            Floor = GameObject.Instantiate(floorPrefab);
            Floor.transform.position = new Vector3(Position.x * WALL_SIZE_X, 0, Position.y * WALL_SIZE_Y);
            Floor.transform.SetParent(parent);
            Walls = new GameObject[4];
        }

        public void RemoveWall(Facing facing) {
            if(Walls[(int)facing] != null)
                Destroy(Walls[(int)facing]);
        }

        public void AddWall(Facing facing, GameObject wallPrefab)
        {
            Vector3 positionOffset = Vector3.zero;
            Vector3 rotation = Vector3.zero;

            switch (facing)
            {
                case Facing.East:
                    positionOffset = new Vector3(0, 3, WALL_SIZE_Y / 2);
                    rotation = new Vector3(0, 0, 0);
                    break;
                case Facing.West:
                    positionOffset = new Vector3(0, 3, -WALL_SIZE_Y / 2);
                    rotation = new Vector3(0, 180, 0);
                    break;
                case Facing.North:
                    positionOffset = new Vector3(WALL_SIZE_X / 2, 3, 0);
                    rotation = new Vector3(0, 90, 0);
                    break;
                case Facing.South:
                    positionOffset = new Vector3(-WALL_SIZE_X / 2, 3, 0);
                    rotation = new Vector3(0, -90, 0);
                    break;
            }

            GameObject wall = GameObject.Instantiate(wallPrefab);
            wall.transform.position = new Vector3(Position.x * WALL_SIZE_X, 0, Position.y * WALL_SIZE_Y) + positionOffset;
            wall.transform.rotation = Quaternion.Euler(rotation);
            wall.transform.SetParent(Floor.transform);
            Walls[(int)facing] = wall;
        }
    }
}
