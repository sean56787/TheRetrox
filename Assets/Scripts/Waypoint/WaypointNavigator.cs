using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WaypointNavigator : MonoBehaviour
{
    public NPCAnimator npcAnimator;
    public NavMeshAgent npcNavAgent;
    public NPCController npcController;
    public Waypoint currentWaypoint;
    public Waypoint previousWaypoint;
    public Transform entryPoint;
    public Transform exitPoint;
    public NPCSpawner spawnerFrom;
    public int direction;
    public bool isWalking;
    public bool isInStore;
    public bool isAlreadyBought;
    public bool isAgentStopped;

    private void Start()
    {
        npcNavAgent = GetComponent<NavMeshAgent>();
        npcAnimator = GetComponent<NPCAnimator>();
        npcController = GetComponent<NPCController>();
        
        direction = Mathf.RoundToInt(Random.Range(0f, 1f)); // 0 or 1
        if(!isAgentStopped) npcNavAgent.SetDestination(currentWaypoint.GetPosition());
        previousWaypoint = currentWaypoint;
    }

    private void Update()
    {
        if(gameObject == null) return;
        if (!isInStore) // 如果在街上(不在店裡)
        {
            CheckIsWalking();
            if (isWalking)
            {
                if (npcController.npcCustomer.npcPersonality.personality == NPCPersonality.Personality.InHurry) npcAnimator.NPC_RunAnimation();
                else npcAnimator.NPC_WalkAnimation();
            }
            else if (!isWalking)
            {
                if(currentWaypoint.isWaypointEnd)
                {
                    StopNavAgent();
                    spawnerFrom.RemoveNPCFromInGameNPCList(this.gameObject);
                    Destroy(gameObject, 0.1f);
                }
                previousWaypoint = currentWaypoint;
                switch (direction)
                {
                    case 0:
                        WalkForward();
                        break;
                    case 1:
                        WalkBackward();
                        break;
                }
                if(!isAgentStopped) npcNavAgent.SetDestination(currentWaypoint.GetPosition());
                if (previousWaypoint.isBranch && !isAlreadyBought)
                {
                    GetComponent<WaypointDecide>().BranchDecision();
                }
            }
        }
    }
    
    private void CheckIsWalking()
    {
        if(isAgentStopped) return;
        if (npcNavAgent.pathPending) // Path 計算中
        {
            isWalking = true;
            return;
        }

        if (npcNavAgent.remainingDistance > npcNavAgent.stoppingDistance)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }
    
    void WalkForward()
    {
        if (currentWaypoint.nextWayPoint is not null && !isInStore)
        {
            currentWaypoint = currentWaypoint.nextWayPoint;
        }
        else
        {
            direction = 1 - direction;
        }
    }

    void WalkBackward()
    {
        if (currentWaypoint.previousWayPoint is not null && !isInStore)
        {
            currentWaypoint = currentWaypoint.previousWayPoint;
        }
        else
        {
            direction = 1 - direction;
        }
    }

    public void SetAgentInStoreDestination(Transform destination)
    {
        if(!isAgentStopped) npcNavAgent.SetDestination(destination.position);
    }

    public void StopNavAgent() //NPC到達終點 清除agent
    {
        if(isAgentStopped) return;
        if (npcNavAgent != null)
        {
            npcNavAgent.isStopped = true; // 停止尋路
            npcNavAgent.ResetPath(); //清除目標
            npcNavAgent.enabled = false;
        }
        CancelInvoke();
        StopAllCoroutines();
        isAgentStopped = true;
    }
}
