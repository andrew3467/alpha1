using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
struct WeightedPrefab {
    public GameObject GO;
    public int Weight;
}

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
    [SerializeField] WeightedPrefab[] wallPrefabs;
    [SerializeField] WeightedPrefab[] floorPrefabs;
    [SerializeField] WeightedPrefab[] ceilingPrefabs;
    [SerializeField] GameObject playerPrefab;

    [Space(20)]
    [SerializeField] int maxCellsCount = 512;
    [SerializeField] int delay = 1;

    Dictionary<Vector2Int, MazeCell> mazeCells;
    public List<Vector2Int> frontierCells;

    MazeCell prevCell;

    System.Random prng;
    bool firstRun = true;
    bool mazeGenerating = false;

    int floorWeight;
    int wallWeight;
    int ceilingWeight;

    void Start() {
        mazeCells = new Dictionary<Vector2Int, MazeCell>();
        frontierCells = new List<Vector2Int>();

        prng = new System.Random();

        CalculateWeights();
        
        
        
        StartPrims();
    }
    void CalculateWeights() {
        for (int i = 0; i < floorPrefabs.Length; i++) {
            floorWeight += floorPrefabs[i].Weight;
        }
    }

    Vector2Int GetRandomCell() {
        return frontierCells[prng.Next(0, frontierCells.Count)];
    }

    public void StartPrims() {
        StartCoroutine(RunPrims());
        SpawnPlayer();
    }
    
    void SpawnPlayer() {
        GameObject playerGO = Instantiate(playerPrefab, new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
    }


    IEnumerator RunPrims() {
        bool running = true;
        mazeGenerating = true;
        while (running) {
            StepThroughPrims();
            yield return new WaitForSeconds(delay);

            running = mazeCells.Count < maxCellsCount;
        }

        mazeGenerating = false;
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


        int randomFloor = prng.Next(0, floorWeight);
        GameObject floorGO = floorPrefabs[0].GO;

        foreach (var go in floorPrefabs) {
            if (randomFloor < go.Weight) {
                floorGO = go.GO;
            }
        }
        
        MazeCell cell = new MazeCell(selection, floorGO, ceilingPrefabs[0].GO, parent);

        var wallPrefab = wallPrefabs[prng.Next(0, wallPrefabs.Length - 1)].GO;
        cell.AddWall(Facing.North, wallPrefab);
        cell.AddWall(Facing.South, wallPrefab);
        cell.AddWall(Facing.East, wallPrefab);
        cell.AddWall(Facing.West, wallPrefab);

        if(mazeCells.Count > 0) {
            var neighbors = GetNeighbors(cell);
            for (int i = 0; i < 4; i++) {
                print(neighbors.Count);
                var neighbor = neighbors[i];
                if (neighbor == null) continue;

                Facing dir = (Facing)i;
                cell.RemoveWall(dir);
            }
        }
        
        
        mazeCells.Add(selection, cell);

        //None is not on edge
        for (int i = 0; i < Neighbors.Length; i++) {
            var neighborPos = selection + Neighbors[i];

            if (!mazeCells.ContainsKey(neighborPos)) {
                frontierCells.Add(neighborPos);
            }
        }

        if (prevCell != null) {
            RemoveWalls(cell, RandomNeighbor(cell));
        }

        prevCell = cell;

    }

    List<MazeCell> GetNeighbors(MazeCell cell) {
        List<MazeCell> neighbors = new List<MazeCell>();
        
        for (int i = 0; i < 4; i++) {
            var pos = cell.Position + Neighbors[i];
            neighbors.Add(mazeCells.ContainsKey(pos) ? mazeCells[pos] : null);
        }

        return neighbors;
    }

    MazeCell RandomNeighbor(MazeCell cell) {
        var neighbors = GetNeighbors(cell);
        var neighbor = neighbors[prng.Next(0, neighbors.Count - 1)];

        int limit = 16;
        while (neighbor == null && limit > 0) {
            neighbor = neighbors[prng.Next(0, 4)];
            limit--;
        }

        return neighbor;
    }
    
    void RemoveWalls(MazeCell cell, MazeCell other) {
        if(other == null || cell == null) return;
        
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
        public GameObject Ceiling;
        public Vector2Int Position;

        public MazeCell(Vector2Int pos, GameObject floorPrefab, GameObject ceilingPrefab, Transform parent)
        {
            Position = pos;
            Floor = GameObject.Instantiate(floorPrefab);
            Floor.transform.position = new Vector3(Position.x * WALL_SIZE_X, 0, Position.y * WALL_SIZE_Y);
            Floor.transform.SetParent(parent);
            
            Ceiling = GameObject.Instantiate(ceilingPrefab);
            Ceiling.transform.position = new Vector3(Position.x * WALL_SIZE_X, 8, Position.y * WALL_SIZE_Y);
            Ceiling.transform.SetParent(parent);
            
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
