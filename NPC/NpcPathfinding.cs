using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NpcPathfinding : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float verticalOffset = 1f;
    [SerializeField] private float nextWaypointDistance = 0.1f;
    [SerializeField] private float pathEndDistanceThreshold = 1f;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Seeker seeker;
 
    public Path path;
    private int currentWaypoint;

    private NpcAI npcAI;

    public bool pathReached { get; private set; }

    private Vector2 targetPosition;
    public bool hasPath { get; private set; }
    public bool isPathfindingComplete { get; private set; } // New flag

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>(); 
        npcAI = GetComponent<NpcAI>();
    }

    private void FixedUpdate()
    {
        if (path != null)
        {
            FollowPath();
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        moveDir = (targetPosition - rb.position).normalized;
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
    }

    public void StopMoving()
    {
        moveDir = Vector2.zero;
        path = null;
        hasPath = false;
        
        // Debug.Log("StopMoving");
        pathReached = true;

        isPathfindingComplete = false; // Reset flag
    }

   public void StartPath(Vector2 targetPosition)
{
    // if (    hasPath) return; // Prevent starting a new pathfinding process if already complete and has a valid path
    pathReached = false;

        if (npcAI.state != NpcAI.State.Following) { 
             targetPosition.y += verticalOffset;
        }
   

    this.targetPosition = targetPosition;
    // Debug.Log("Starting pathfinding to: " + targetPosition);

    isPathfindingComplete = false; // Ensure this is reset before starting a new path
    seeker.StartPath(rb.position, targetPosition, OnPathComplete);
}


   private void OnPathComplete(Path p)
{
    isPathfindingComplete = true; // Set flag to true
    if (!p.error && p.vectorPath.Count > 0) 
    {
        float distanceToTarget = Vector2.Distance(p.vectorPath[p.vectorPath.Count - 1], targetPosition);
        if (distanceToTarget < pathEndDistanceThreshold)
        {
            path = p;
            currentWaypoint = 0;
            hasPath = true;
            // Debug.Log("Path is valid and set. hasPath: " + hasPath);
        }
        else
        {
            path = null;
            hasPath = false;
            isPathfindingComplete = false; // Reset flag for new pathfinding
            // Debug.Log("Path is invalid; target is too far from the end of the path. hasPath: " + hasPath);
        }
    }
    else
    {
        path = null;
        hasPath = false;
        isPathfindingComplete = false; // Reset flag for new pathfinding
        Debug.Log("Error in pathfinding. hasPath: " + hasPath);
    }
}


    public void FollowPath()
    {
        if (path == null) return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            StopMoving();
            return;
        }

        Vector3 offsetWaypoint = path.vectorPath[currentWaypoint];

        if (npcAI.state != NpcAI.State.Following) { 
            offsetWaypoint.y += verticalOffset;
        }

       

        Vector2 moveDir = ((Vector2)offsetWaypoint - rb.position).normalized;
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        float distance = Vector2.Distance(transform.position, offsetWaypoint);
        if (distance < nextWaypointDistance)
        {
            IncrementWaypoint();
        }
    }

   public Vector3 GetCurrentWaypoint()
{
    if (path == null)
    {
        Debug.LogWarning("Path is null. Cannot retrieve waypoint.");
        return transform.position;
    }

    if (currentWaypoint < 0)
    {
        Debug.LogWarning("CurrentWaypoint index is negative. CurrentWaypoint: " + currentWaypoint);
        return transform.position;
    }

    if (currentWaypoint >= path.vectorPath.Count)
    {
        Debug.LogWarning("CurrentWaypoint index is out of range. CurrentWaypoint: " + currentWaypoint + ", Path vectorPath count: " + path.vectorPath.Count);
        return transform.position;
    }

    Vector3 waypoint = path.vectorPath[currentWaypoint];
    waypoint.y += verticalOffset; // Apply the vertical offset

    return waypoint;
}

    public void IncrementWaypoint()
    {
        if (path != null && currentWaypoint < path.vectorPath.Count - 1)
        {
            currentWaypoint++;
        }
        else
        {
            StopMoving();
            Debug.Log("here StopMoving");
        }
    }
}
