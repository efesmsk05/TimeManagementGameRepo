using UnityEngine;
using UnityEngine.AI;

public class CustomerMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        agent.updateRotation = false;
        agent.updateUpAxis = false; 
    }

    public bool MoveTo(Vector3 targetPos, float speed)
    {
        // GÜVENLİK 1: Agent veya GameObject kapalıysa işlem yapma
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh) return false;

        agent.speed = speed;
        agent.SetDestination(targetPos);

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false; 
    }

    public void StopMoving()
    {
        // GÜVENLİK 2: Agent NavMesh üzerinde mi?
        if (agent.isOnNavMesh && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        
        agent.avoidancePriority = 50; 
    }

    public void StartMoving()
    {
        if (agent.isActiveAndEnabled)
        {
            if (!agent.isOnNavMesh)
            {
                NavMeshHit hit;
                // En yakın NavMesh noktasına 1 birim çapında bak
                if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position); // Zorla NavMesh'e oturt
                }
            }

            // Artık güvenle hareket ettirebiliriz
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.avoidancePriority = 0; 
            }
        }
    }
}