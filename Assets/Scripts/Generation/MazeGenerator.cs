using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    const float WALL_SIZE_X = 8f;
    const float WALL_SIZE_Y = 8f;

    [SerializeField] Transform parent;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject floorPrefab;
    [SerializeField] Material groundMat;

    [Space(20)]
    [SerializeField] Vector2Int size = new Vector2Int(60, 60);  // Set the maze size to 60x60
    [SerializeField] int depth = 4;

    List<GameObject> walls;
    GameObject ground;
    MazeCell[,] cells;

    void Start()
    {
        walls = new List<GameObject>();
        GenerateCells();
        GenerateMaze();
    }

    void GenerateCells()
    {
        cells = new MazeCell[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cellPosition = new Vector2Int(x, y);
                cells[x, y] = new MazeCell(cellPosition, floorPrefab, parent);
                cells[x, y].AddWall(Facing.North, wallPrefab, parent);
                cells[x, y].AddWall(Facing.South, wallPrefab, parent);
                cells[x, y].AddWall(Facing.East, wallPrefab, parent);
                cells[x, y].AddWall(Facing.West, wallPrefab, parent);
            }
        }
    }

    void GenerateMaze()
    {
        List<Edge> edges = new List<Edge>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (x < size.x - 1)
                {
                    edges.Add(new Edge(new Vector2Int(x, y), new Vector2Int(x + 1, y)));
                }
                if (y < size.y - 1)
                {
                    edges.Add(new Edge(new Vector2Int(x, y), new Vector2Int(x, y + 1)));
                }
            }
        }

        edges = Shuffle(edges);

        DisjointSet ds = new DisjointSet(size.x * size.y);

        foreach (var edge in edges)
        {
            int cellIndex1 = edge.Cell1.x + edge.Cell1.y * size.x;
            int cellIndex2 = edge.Cell2.x + edge.Cell2.y * size.x;

            if (ds.Find(cellIndex1) != ds.Find(cellIndex2))
            {
                ds.Union(cellIndex1, cellIndex2);
                RemoveWall(cells[edge.Cell1.x, edge.Cell1.y], cells[edge.Cell2.x, edge.Cell2.y]);
            }
        }
    }

    void RemoveWall(MazeCell cell1, MazeCell cell2)
    {
        Vector2Int direction = cell2.Position - cell1.Position;
        if (direction == Vector2Int.up)
        {
            Destroy(cell1.Walls[(int)Facing.North]);
            Destroy(cell2.Walls[(int)Facing.South]);
        }
        else if (direction == Vector2Int.down)
        {
            Destroy(cell1.Walls[(int)Facing.South]);
            Destroy(cell2.Walls[(int)Facing.North]);
        }
        else if (direction == Vector2Int.right)
        {
            Destroy(cell1.Walls[(int)Facing.East]);
            Destroy(cell2.Walls[(int)Facing.West]);
        }
        else if (direction == Vector2Int.left)
        {
            Destroy(cell1.Walls[(int)Facing.West]);
            Destroy(cell2.Walls[(int)Facing.East]);
        }
    }

    List<Edge> Shuffle(List<Edge> edges)
    {
        System.Random rng = new System.Random();
        int n = edges.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Edge value = edges[k];
            edges[k] = edges[n];
            edges[n] = value;
        }
        return edges;
    }

    public enum Facing
    {
        North = 0,
        South = 1,
        East = 2,
        West = 3
    }

    struct Edge
    {
        public Vector2Int Cell1;
        public Vector2Int Cell2;

        public Edge(Vector2Int cell1, Vector2Int cell2)
        {
            Cell1 = cell1;
            Cell2 = cell2;
        }
    }

    public class DisjointSet
    {
        int[] parent, rank;

        public DisjointSet(int n)
        {
            parent = new int[n];
            rank = new int[n];
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
                rank[i] = 0;
            }
        }

        public int Find(int u)
        {
            if (u != parent[u])
            {
                parent[u] = Find(parent[u]);
            }
            return parent[u];
        }

        public void Union(int u, int v)
        {
            int rootU = Find(u);
            int rootV = Find(v);

            if (rootU != rootV)
            {
                if (rank[rootU] > rank[rootV])
                {
                    parent[rootV] = rootU;
                }
                else if (rank[rootU] < rank[rootV])
                {
                    parent[rootU] = rootV;
                }
                else
                {
                    parent[rootV] = rootU;
                    rank[rootU]++;
                }
            }
        }
    }

    public class MazeCell
    {
        public GameObject[] Walls;
        public GameObject Floor;
        public Vector2Int Position { get; private set; }

        public MazeCell(Vector2Int pos, GameObject floorPrefab, Transform parent)
        {
            Position = pos;
            Floor = GameObject.Instantiate(floorPrefab);
            Floor.transform.position = new Vector3(Position.x * WALL_SIZE_X, 0, Position.y * WALL_SIZE_Y);
            Floor.transform.SetParent(parent);
            Walls = new GameObject[4];
        }

        public void AddWall(Facing facing, GameObject wallPrefab, Transform parent)
        {
            Vector3 positionOffset = Vector3.zero;
            Vector3 rotation = Vector3.zero;

            switch (facing)
            {
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
