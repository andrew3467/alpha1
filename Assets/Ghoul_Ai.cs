using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Ghoul_Ai : MonoBehaviour
{
    [SerializeField] Transform target;
    
    private NavMeshAgent agent;
    private float roamtimer;
    private GameObject[] roamNodes;

    public float roamDelay = 3f;      // Delay between roaming
    public float roamRadius = 50f;    // Radius to create nodes
    public int numberOfNodes = 100;   // Number of nodes to create
    public GameObject nodePrefab;     // Prefab for nodes
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        roamtimer = roamDelay;

        // Create roaming nodes
        CreateRoamNodes();

        // Set initial destination
        SetRandomDestination();
    }

    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            roamtimer += Time.deltaTime;
            if (roamtimer >= roamDelay)
            {
                SetRandomDestination();
                roamtimer = 0f;
            }
        }
    }

    void CreateRoamNodes()
    {
        roamNodes = new GameObject[numberOfNodes];
        for (int i = 0; i < numberOfNodes; i++)
        {
            Vector3 randomPosition = GetValidNavMeshPosition(transform.position, roamRadius);
            GameObject node = Instantiate(nodePrefab, randomPosition, Quaternion.identity);
            roamNodes[i] = node;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (roamNodes != null)
        {
            foreach (var node in roamNodes)
            {
                Gizmos.DrawCube(node.transform.position, Vector3.one);
            }
        }
    }

    void SetRandomDestination()
    {
        if (roamNodes.Length == 0)
        {
            Debug.LogWarning("No Roam Nodes Available.");
            return;
        }

        int randomIndex = Random.Range(0, roamNodes.Length);
        Vector3 newDestination = roamNodes[randomIndex].transform.position;
        agent.SetDestination(newDestination);
    }

    Vector3 GetValidNavMeshPosition(Vector3 center, float radius)
    {
        Vector3 randomDirection;
        NavMeshHit hit;
        int attempts = 0;
        const int maxAttempts = 30;

        do
        {
            randomDirection = Random.insideUnitSphere * radius;
            randomDirection += center;
            attempts++;
        }
        while (!NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Failed to find valid NavMesh position after multiple attempts.");
            return center; // Fallback to the center position if no valid position is found
        }

        return hit.position;
    }
}
