using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController;
    public SpriteRenderer spriteRenderer;
    public NavMeshAgent agent;


    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = playerController.playerSpeed;
        // --- 2D NAVMESH AYARLARI ---
        agent.updateRotation = false; // Sprite dönmesin (Biz flip yapacağız)
        agent.updateUpAxis = false;  
    }
    public bool MovePlayer(Vector3 targetPosition)
    {

        agent.speed = playerController.playerSpeed;
        agent.SetDestination(targetPosition);

        HandleSpriteFlip();

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true; // Evet, vardık!
                }
            }
        }

        return false; // Henüz varmadık, yürümeye devam
    }

    
    private void HandleSpriteFlip()
    {
        // Eğer hareket halindeysek
        if (agent.velocity.magnitude > 0.1f)
        {
            // Sağa gidiyorsa (Velocity X pozitif) -> Flip yapma
            // Sola gidiyorsa (Velocity X negatif) -> Flip yap
            if (agent.velocity.x > 0)
            {
                spriteRenderer.flipX = false; 
            }
            else if (agent.velocity.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

}
