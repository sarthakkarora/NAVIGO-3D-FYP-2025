using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/** Utils to estimate remaining path to selected POI **/
public class PathEstimationUtils : MonoBehaviour
{
    public static PathEstimationUtils instance;

    public NavMeshAgent agent;

    // distance
    float remainingDistance = 0;

    // time estimation
    float estimatedArrivalDuration = 0;
    float defaultVelocity = 0.45f; // we substract, since you will be slower
    // default 1.34f; // in m/s source: https://www.healthline.com/health/exercise-fitness/average-walking-speed
    Camera ARCamera;

    // collider radius of the ARCamera used to detect arrival, radius is substracted from overal path length
    float arCameraColliderRadius;

    POI destination;

    // collider radius of the current destination, radius is substracted from overal path length
    float destinationColliderRadius;

    public Slider progressSlider;

    float startingDistance = 0; // distance when starting navigation
    bool estimationStarted = false;

    void Awake()
    {
        instance = this;
        ARCamera = Camera.main;
    }

    void Start()
    {
        arCameraColliderRadius = ARCamera.gameObject.GetComponent<SphereCollider>().radius;
    }

    void FixedUpdate()
    {
        HandleProgress();
    }

    public void HandleProgress()
    {
        if (estimationStarted)
        {
            float currentDistance = startingDistance - remainingDistance;
            progressSlider.value = currentDistance / startingDistance + 0.03f;
        }
    }

    /**
     * Estimates time from first to last position of given Vector3 path, e.g. from NavMeshAgent.
     */
    public void UpdateEstimation(Vector3[] path)
    {
        if (destination == null) {
            destination = NavController.instance.currentDestination;
            destinationColliderRadius = destination.poiCollider.gameObject.GetComponent<SphereCollider>().radius;
        }

        if (path.Length > 1)
        {
            float remainingPathTotal = 0;
            // loop through path and add up distance between Vectors
            for (int i = 0; i < path.Length; i++)
            {
                if (i < path.Length - 1) // we always comparing with next one
                {
                    remainingPathTotal += Vector3.Distance(path[i], path[i + 1]);
                }
            }
            remainingDistance = remainingPathTotal - arCameraColliderRadius - destinationColliderRadius;

            // hotfix because average velocity is 0 when user is standing still
            estimatedArrivalDuration = remainingDistance / defaultVelocity;

            if (!estimationStarted || remainingDistance > startingDistance)
            {
                startingDistance = remainingDistance;
                estimationStarted = true;
            }
        }
    }

    public void ResetEstimation()
    {
        destination = null;
        estimationStarted = false;
        remainingDistance = 0;
        estimatedArrivalDuration = 0;
    }

    public int getRemainingDistanceMeters()
    {
        return (int)remainingDistance;
    }

    public int getRemainingDurationSeconds()
    {
        return (int)estimatedArrivalDuration;
    }

    /**
    *   Returns distance from agent to given destination
    */
    public float EstimateDistanceToPosition(POI destination)
    {
        agent.isStopped = true;
        NavMeshPath path = new NavMeshPath();

        if (!agent.isOnNavMesh)
        {
            return -1;
        }

        NavMesh.CalculatePath(agent.gameObject.transform.position, destination.poiCollider.transform.position, NavMesh.AllAreas, path);

        if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
        {
            // handle unreachable route
            return -2;
        }

        if (path.corners.Length > 1)
        {
            float distance = 0;
            for (int i = 0; i < path.corners.Length; i++)
            {
                if (i < path.corners.Length - 1) // we always comparing with next one
                {
                    distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                }
            }
            return distance;
        }

        return -1;
    }

}