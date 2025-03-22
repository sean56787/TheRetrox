using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterQueueManager : MonoBehaviour
{
    public static CounterQueueManager instance;
    public Transform[] customerQueuePositions;
    public Queue<NPCController> _customerQueue = new();

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    public void JoinQueue(NPCController customerController)
    {
        if (_customerQueue.Count < customerQueuePositions.Length)
        {
            customerController.currentQueueIndex = _customerQueue.Count;
            _customerQueue.Enqueue(customerController);
        }
    }

    public void RemoveFirst()
    {
        if (_customerQueue.Count > 0)
        {
            _customerQueue.Dequeue();
        }
    }

    public void UpdateQueue()
    {
        int index = 0;
        foreach (var customerController in _customerQueue)
        {
            if(customerController == null) continue;
            customerController.currentQueueIndex = index;
            customerController.SetQueuePosition(customerQueuePositions[index]);
            index++;
        }
    }

    public void ClearQueue()
    {
        _customerQueue.Clear();
    }
}
