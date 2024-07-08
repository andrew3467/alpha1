using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    const float WALL_SIZE_X = 8f; // Width of each wall segment
    const float WALL_SIZE_Y = 8f; // Height of each wall segment

    [SerializeField] Transform parent; // The great parent of all maze objects im bettin you cant relate
    [SerializeField] GameObject wallPrefab; // The one and only wall in this maze
    [SerializeField] GameObject floorPrefab; // The ground beneath your feet (kinda)
    [SerializeField] Material groundMat; // Who needs ground when you can have walls? (hint you need both)

    [Space(20)]
    [SerializeField] Vector2Int size = new Vector2Int(60, 60); // The vastness of our maze (width x height)
    [SerializeField] int depth = 4; // How deep into the maze do i dare to go? exactly how deep i went inside your father
    [SerializeField] int numberOfMazes = 10; // A horde of mazes is upon us! like the swarm of dildos im eating HEHEHEHHAHAHAHHAHA kill me andrew please put me down

    List<GameObject> walls; // The collective of all walls ever built
    GameObject ground; // The mystical, elusive ground object
    MazeCell[,] cells; // Behold the matrix of maze cells!

    void Start()
    {
        walls = new List<GameObject>(); // Initializing the wall street of mazes
        numberOfMazes = Mathf.Clamp(numberOfMazes, 1, 10); // Throttling the maze generator to a reasonable number...i say reasonable but who am i kidding make it 4000 for all i care

        // Loop to unleash multiple mazes upon the world because i love mazes so much omg kill me
        for (int i = 0; i < numberOfMazes; i++)
        {
            Vector2Int offset = new Vector2Int(i * size.x, 2); // Giving each maze its personal space
            GenerateCells(offset); // Creating the very essence of maze-ness just like how much of a maze your whole existence is
            GenerateMaze(offset); // Constructing the labyrinthian pathways like im in a dungeons and dragons sessions fucking end me
        }
    }

    // Function to spawn maze cells with a touch of wizardry
    void GenerateCells(Vector2Int offset)
    {
        cells = new MazeCell[size.x, size.y]; // Preparing the canvas for our maze masterpiece

        // Looping through each cell to establish its presence in the maze continuum
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cellPosition = new Vector2Int(x, y) + offset; // Calculating the precise location of this maze cell
                cells[x, y] = new MazeCell(cellPosition, floorPrefab, parent); // Bringing this cell to life with a floor and parent
                cells[x, y].AddWall(Facing.North, wallPrefab, parent); // Defining its northern boundary
                cells[x, y].AddWall(Facing.South, wallPrefab, parent); // Marking the southern limit
                cells[x, y].AddWall(Facing.East, wallPrefab, parent); // Establishing the eastern frontier
                cells[x, y].AddWall(Facing.West, wallPrefab, parent); // Securing the western flank
            }
        }
    }

    // Function to weave the intricate web of maze paths and walls
    void GenerateMaze(Vector2Int offset)
    {
        List<Edge> edges = new List<Edge>(); // Container for potential maze connections

        // Creating connections between neighboring maze cells
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (x < size.x - 1)
                {
                    edges.Add(new Edge(new Vector2Int(x, y) + offset, new Vector2Int(x + 1, y) + offset)); // Bridging eastward like donald j trump
                }
                if (y < size.y - 1)
                {
                    edges.Add(new Edge(new Vector2Int(x, y) + offset, new Vector2Int(x, y + 1) + offset)); // Connecting southward like the mexicans
                }
            }
        }

        edges = Shuffle(edges); // Shuffling the deck of potential maze connections like my fathers anus

        DisjointSet ds = new DisjointSet(size.x * size.y); // A set to manage connected maze components

        foreach (var edge in edges)
        {
            int cellIndex1 = (edge.Cell1.x - offset.x) + (edge.Cell1.y - offset.y) * size.x; // Calculating maze cell indices
            int cellIndex2 = (edge.Cell2.x - offset.x) + (edge.Cell2.y - offset.y) * size.x;

            if (ds.Find(cellIndex1) != ds.Find(cellIndex2)) // If the cells are not yet connected like your parents marriage
            {
                ds.Union(cellIndex1, cellIndex2); // Connecting them in the grand maze scheme
                RemoveWall(cells[edge.Cell1.x - offset.x, edge.Cell1.y - offset.y], cells[edge.Cell2.x - offset.x, edge.Cell2.y - offset.y]); // Demolishing the wall between them like how i demolished your parentss marriage
            }
        }
    }

    // Function to obliterate walls between connected maze cells
    void RemoveWall(MazeCell cell1, MazeCell cell2)
    {
        Vector2Int direction = cell2.Position - cell1.Position; // Determining the direction between two maze cells

        // Choosing the right wall to destroy based on the direction between cells
        if (direction == Vector2Int.up)
        {
            Destroy(cell1.Walls[(int)Facing.North]); // Erasing the northern boundary of cell1
            Destroy(cell2.Walls[(int)Facing.South]); // Eradicating the southern boundary of cell2
        }
        else if (direction == Vector2Int.down)
        {
            Destroy(cell1.Walls[(int)Facing.South]); // Annihilating the southern boundary of cell1
            Destroy(cell2.Walls[(int)Facing.North]); // Eliminating the northern boundary of cell2
        }
        else if (direction == Vector2Int.right)
        {
            Destroy(cell1.Walls[(int)Facing.East]); // Wiping out the eastern boundary of cell1
            Destroy(cell2.Walls[(int)Facing.West]); // Smashing the western barrier of cell2
        }
        else if (direction == Vector2Int.left)
        {
            Destroy(cell1.Walls[(int)Facing.West]); // Decimating the western boundary of cell1
            Destroy(cell2.Walls[(int)Facing.East]); // Breaking down the eastern barrier of cell2
        }
    }

    // Function to shuffle the sequence of maze connections
    List<Edge> Shuffle(List<Edge> edges)
    {
        System.Random rng = new System.Random();
        int n = edges.Count; 

     
        while (n > 1)
        {
            n--; 
            int k = rng.Next(n + 1); 
            Edge value = edges[k]; 
            edges[k] = edges[n]; // Swapping edge at index k with the last unshuffled edge
            edges[n] = value; 
        }

        return edges; // Returning the shuffled list of maze connections
    }

    // Enum for defining directions in the maze (cardinal points)
    public enum Facing
    {
        North = 0, // Direction pointing upwards
        South = 1, // Direction pointing downwards
        East = 2, // Direction pointing to the right
        West = 3 // Direction pointing to the left
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
            if (u != parent[u]) // If the cell is not its own parent
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
        public GameObject[] Walls; // Array to store walls around the maze cell
        public GameObject Floor; // The ground tile of the maze cell
        public Vector2Int Position { get; private set; } // Position of the maze cell in the grid

        public MazeCell(Vector2Int pos, GameObject floorPrefab, Transform parent)
        {
            Position = pos; // Setting the position of the maze cell
            Floor = GameObject.Instantiate(floorPrefab); // Creating the floor of the maze cell
            Floor.transform.position = new Vector3(Position.x * WALL_SIZE_X, 0, Position.y * WALL_SIZE_Y);
            Floor.transform.SetParent(parent); 

            Walls = new GameObject[4];
        }

        public void AddWall(Facing facing, GameObject wallPrefab, Transform parent)
        {
            Vector3 positionOffset = Vector3.zero; // Offset position of the wall
            Vector3 rotation = Vector3.zero; // Rotation of the wall

    
            switch (facing)
            {
                case Facing.North:
                    positionOffset = new Vector3(0, 3, WALL_SIZE_Y / 2); // Calculating north wall position
                    rotation = new Vector3(0, 0, 0); // Setting rotation to face north
                    break;
                case Facing.South:
                    positionOffset = new Vector3(0, 3, -WALL_SIZE_Y / 2); // Calculating south wall position
                    rotation = new Vector3(0, 180, 0); // Setting rotation to face south
                    break;
                case Facing.East:
                    positionOffset = new Vector3(WALL_SIZE_X / 2, 3, 0); // Calculating east wall position
                    rotation = new Vector3(0, 90, 0); // Setting rotation to face east
                    break;
                case Facing.West:
                    positionOffset = new Vector3(-WALL_SIZE_X / 2, 3, 0); // Calculating west wall position
                    rotation = new Vector3(0, -90, 0); // Setting rotation to face west
                    break;
            }

            // Creating and positioning the wall object
            GameObject wall = GameObject.Instantiate(wallPrefab); // Creating the wall prefab
            wall.transform.position = new Vector3(Position.x * WALL_SIZE_X, 0, Position.y * WALL_SIZE_Y) + positionOffset; // Positioning the wall
            wall.transform.rotation = Quaternion.Euler(rotation); 
            wall.transform.SetParent(parent); 

            Walls[(int)facing] = wall;
        }
    }
}