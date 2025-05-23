using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
    
    public List<Transform> worldSpawnPoint;
    public List<GameObject> originNPCList;
    public List<GameObject> inGameNPCList;
    public int maxSpawnCount = 1;
    public int currentSpawnCount = 0;
    public float minSpawnTime = 15f;
    public float maxSpawnTime = 30f;
    public bool isInSpawnDelay = false;
    private void Start()
    {
        foreach (var origin in originNPCList)
        {
            origin.SetActive(false);
        }
    }

    private void Update()
    {
        if(!isInSpawnDelay) StartCoroutine(SpawnOverTime());
    }

    private IEnumerator SpawnOverTime()
    {
        if (currentSpawnCount < maxSpawnCount)
        {
            isInSpawnDelay = true;
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
            SpawnNPC();
            isInSpawnDelay = false;
        }
    }
    
    private void SpawnNPC()
    {
        if(!DayNightManager.instance.isDaytime) return;
        Transform spawnPoint = worldSpawnPoint[Random.Range(0, worldSpawnPoint.Count)];
        Vector3 randomPosition = spawnPoint.position + Random.insideUnitSphere * 1f;          // 取以點為半徑1的隨機位置
        randomPosition.y = spawnPoint.position.y;    // 強制高度

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomPosition, out navHit, 2f,
                NavMesh.AllAreas))          //球形半徑x找一個可以走的位置
        {
            GameObject randomNPC = originNPCList[Random.Range(0, originNPCList.Count)];
            GameObject newNPC = Instantiate(randomNPC, navHit.position, Quaternion.identity); //Quaternion.identity 保持原來方向
            newNPC.GetComponent<WaypointNavigator>().spawnerFrom = this;
            newNPC.transform.name = $"npc_{currentSpawnCount}";
            inGameNPCList.Add(newNPC);
            newNPC.SetActive(true);
        }
        currentSpawnCount++;
    }

    public void RemoveNPCFromInGameNPCList(GameObject npcToDelete) // 從NPC清單刪除某NPC
    {
        inGameNPCList.Remove(npcToDelete);
    }

    public void ResetAllNPCFromThisSpawner()                      // 刪除NPC
    {
        foreach (var npcToDelete in inGameNPCList.ToList())
        {
            npcToDelete.GetComponent<NPCController>().DestroyCheckedProducts(); // delete product(if exist)
            npcToDelete.GetComponent<WaypointNavigator>().StopNavAgent();       // stop agent
            RemoveNPCFromInGameNPCList(npcToDelete);
            Destroy(npcToDelete);
        }
        currentSpawnCount = 0;
        CounterQueueManager.instance.ClearQueue();
    }
}
