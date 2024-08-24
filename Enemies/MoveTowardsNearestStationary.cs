using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsNearestStationary : MonoBehaviour
{
    public GameObject[] stationaryObjects; // Array of all stationary objects
    public float moveSpeed = 5f;

    private Transform target; // The nearest stationary object to move towards

    void Start()
    {
        FindNearestStationaryObject();
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    void FindNearestStationaryObject()
    {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestObject = null;

        foreach (GameObject obj in stationaryObjects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestObject = obj;
            }
        }

        if (nearestObject != null)
        {
            target = nearestObject.transform;
        }
    }
}
