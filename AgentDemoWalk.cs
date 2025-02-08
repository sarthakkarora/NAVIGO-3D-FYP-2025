using UnityEngine;
using UnityEngine.AI;

public class AgentDemoWalk : MonoBehaviour
{
    public static AgentDemoWalk instance;
    public NavMeshAgent agent;
    Vector3 agentStartPosition;
    public GameObject destinationA;
    public GameObject destinationB;
    public GameObject augmentationParent;
    string currentDestination = "";
    public bool isLocalized = false;
    bool isWalking = false;

    void Awake()
    {
        instance = this;
        agentStartPosition = agent.gameObject.transform.position;
    }

    // handle start of demo
    void Update()
    {
        if (isLocalized)
        {
            SetRenderVisibility(true);
            agent.enabled = true;
            if (agent.isOnNavMesh && !isWalking)
            {
                StartDemo();
            }
        }
        else
        {
            SetRenderVisibility(false);
            agent.enabled = false;
            isWalking = false;
            agent.gameObject.transform.position = agentStartPosition;
            currentDestination = "";
        }

        if (isWalking)
        {
            HandleDemo();
        }

    }

    // loops through renderers and enables disables them
    void SetRenderVisibility(bool visible)
    {
        for (var i = 0; i < augmentationParent.transform.childCount; i++)
        {
            SetEnabledOnChildComponents(augmentationParent.transform, visible);
        }
    }

    // enable disable renderer, canvas or colliders on all child components
    void SetEnabledOnChildComponents(Transform augmentationTransform, bool value)
    {
        var rendererComponent = augmentationTransform.GetComponent<Renderer>();
        if (rendererComponent != null)
            rendererComponent.enabled = value;
        var canvasComponent = augmentationTransform.GetComponent<Canvas>();
        if (canvasComponent != null)
            canvasComponent.enabled = value;
        var colliderComponent = augmentationTransform.GetComponent<Collider>();
        if (colliderComponent != null)
            colliderComponent.enabled = value;

        for (var i = 0; i < augmentationTransform.childCount; i++)
            SetEnabledOnChildComponents(augmentationTransform.GetChild(i), value);
    }


    void StartDemo()
    {
        isWalking = true;
        agent.isStopped = false;
    }

    // handle agent continuously walking from A to B and backwards
    void HandleDemo()
    {
        if (currentDestination == "")
        {
            currentDestination = destinationA.name;
            agent.destination = destinationA.transform.position;
        }

        // Check if we've reached the destination
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    if (currentDestination == destinationA.name)
                    {
                        // agent was at A go to B
                        currentDestination = destinationB.name;
                        agent.destination = destinationB.transform.position;
                    }
                    else
                    {
                        // otherwise go to A
                        currentDestination = destinationA.name;
                        agent.destination = destinationA.transform.position;
                    }
                }
            }
        }

    }

    // set localization state from sdk
    public void SetLocalizedState(bool localized)
    {
        isLocalized = localized;
    }

    public bool IsWalking() {
        return isWalking;
    }

}
