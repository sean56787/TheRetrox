using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaypointDecide : MonoBehaviour
{
    public int branchRate = 50;

    private void Update()       //商店營業時有5成機率進入
    {
        if (GameStateManager.instance.isOperating)
        {
            branchRate = 50;
        }
        else
        {
            branchRate = 0;
        }
    }

    public void BranchDecision()
    {
        int prob = Random.Range(0, 101);
        if (prob <= branchRate) // 進商店
        {
            GetComponent<NPCController>().currentNpcState = NPCController.NPCState.EnterStore;
        }
    }
}
