using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

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
        MazeCell cell = new MazeCell(new Vector2Int(0,1), floorPrefab);
        cell.AddWall(Facing.North, wallPrefab);
        cell.AddWall(Facing.South, wallPrefab);
        cell.AddWall(Facing.East, wallPrefab);
        cell.AddWall(Facing.West, wallPrefab);
    }

    void GenerateMaze(int depth) {
        
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

        public MazeCell(Vector2Int pos, GameObject floorPrefab) {
            Floor = Instantiate(floorPrefab);
            Walls = new GameObject[4];
            Position = pos;
        }

        public void AddWall(Facing facing, GameObject wallPrefab) {
            Vector3 rotation = new Vector3(
                0,
                90.0f * (int)facing,
                0
            );

            GameObject wall = GameObject.Instantiate(wallPrefab);
            wall.transform.rotation = Quaternion.Euler(rotation);

            switch (facing) {
                case Facing.North:
                    wall.transform.position = new Vector3(
                            0,
                            3,
                            Position.y + 4.0f
                    );
                    break;
                
                case Facing.South:
                    wall.transform.position = new Vector3(
                        0,
                        3,
                        -(Position.y + 4.0f)
                    );
                    break;
                
                case Facing.East:
                    wall.transform.position = new Vector3(
                        Position.x + 4.0f,
                        3,
                        0
                    );
                    break;
                
                case Facing.West:
                    wall.transform.position = new Vector3(
                        -(Position.x + 4.0f),
                        3,
                        0
                    );
                    break;
            }
        }
    }
}
