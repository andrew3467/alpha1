using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {
    const float WALL_SIZE_X = 8f; 
    const float WALL_SIZE_Y = 8f; 
    
    [SerializeField] Transform parent;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject floorPrefab;
    [SerializeField] Material groundMat;
    
    [Space(20)]
    [SerializeField] Vector2Int size = Vector2Int.zero;
    [SerializeField] int depth = 4;

    List<GameObject> walls;
    GameObject ground;

    void Start() {
        walls = new List<GameObject>();
        GenerateCells();
        GenerateMaze(depth);
    }

    void GenerateCells() {
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                Vector2Int cellPosition = new Vector2Int(x, y);
                MazeCell cell = new MazeCell(cellPosition, floorPrefab, parent);
                cell.AddWall(Facing.North, wallPrefab, parent);
                cell.AddWall(Facing.South, wallPrefab, parent);
                cell.AddWall(Facing.East, wallPrefab, parent);
                cell.AddWall(Facing.West, wallPrefab, parent);
            }
        }
    }

    void GenerateMaze(int depth) {
        // Maze generation logic here
    }

    enum Facing {
        North = 0,
        South = 1,
        East = 2,
        West = 3
    }

    struct MazeCell {
        GameObject[] Walls;
        GameObject Floor;
        Vector2Int Position;

        public MazeCell(Vector2Int pos, GameObject floorPrefab, Transform parent) {
            Position = pos;
            Floor = GameObject.Instantiate(floorPrefab);
            Floor.transform.position = new Vector3(Position.x * WALL_SIZE_X, 0, Position.y * WALL_SIZE_Y);
            Floor.transform.SetParent(parent);
            Walls = new GameObject[4];
        }

        public void AddWall(Facing facing, GameObject wallPrefab, Transform parent) {
            Vector3 positionOffset = Vector3.zero;
            Vector3 rotation = Vector3.zero;

            switch (facing) {
                case Facing.North:
                    positionOffset = new Vector3(0, 3, WALL_SIZE_Y / 2);
                    rotation = new Vector3(0, 0, 0);
                    break;

                case Facing.South:
                    positionOffset = new Vector3(0, 3, -WALL_SIZE_Y / 2);
                    rotation = new Vector3(0, 180, 0);
                    break;

                case Facing.East:
                    positionOffset = new Vector3(WALL_SIZE_X / 2, 3, 0);
                    rotation = new Vector3(0, 90, 0);
                    break;

                case Facing.West:
                    positionOffset = new Vector3(-WALL_SIZE_X / 2, 3, 0);
                    rotation = new Vector3(0, -90, 0);
                    break;
            }

            GameObject wall = GameObject.Instantiate(wallPrefab);
            wall.transform.position = new Vector3(Position.x * WALL_SIZE_X, 0, Position.y * WALL_SIZE_Y) + positionOffset;
            wall.transform.rotation = Quaternion.Euler(rotation);
            wall.transform.SetParent(parent);

            Walls[(int)facing] = wall;
        }
    }
}
