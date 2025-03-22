using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class NPCCustomer : MonoBehaviour
{
    private NavMeshAgent _agent;
    [FormerlySerializedAs("_npcPersonality")] public NPCPersonality npcPersonality;
    private NPCShoppingList _npcShoppingList;
    public Transform throwPos;
    public float throwStrength = 5f;
    public GameObject moneyPrefab;
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        npcPersonality = GetComponent<NPCPersonality>();
        _npcShoppingList = GetComponent<NPCShoppingList>();
        if (_npcShoppingList == null || npcPersonality == null)
        {
            throw new Exception("missing NPCPersonality or NPCShoppingList");
        }
        SetNPCSpeed();
    }
    private void Update()
    {
        SetNPCSpeed();
    }
    private void SetNPCSpeed()
    {
        switch (npcPersonality.personality)
        {
            case NPCPersonality.Personality.Normal:
            case NPCPersonality.Personality.Drinker:
            case NPCPersonality.Personality.Fruiter:
            case NPCPersonality.Personality.Snacker:
            case NPCPersonality.Personality.Shopaholic:
            case NPCPersonality.Personality.Thrifty:
                _agent.speed = 2f;
                break;
            case NPCPersonality.Personality.InHurry: 
                _agent.speed = 3.5f;
                break;
            case NPCPersonality.Personality.Sloth:
                _agent.speed = 1.5f;
                break;
        }
    }

    
}
