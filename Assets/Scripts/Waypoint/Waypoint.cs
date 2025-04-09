using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Waypoint : MonoBehaviour //NPC路標
{
    public Waypoint previousWayPoint;
    public Waypoint nextWayPoint;
    public bool isBranch;
    public float wayPointWidth = 2f;
    public bool isWaypointEnd;
    public Vector3 GetPosition()      //取的路標上某一點
    {
        Vector3 minBound = transform.position + transform.right * wayPointWidth / 2f;
        Vector3 maxBound = transform.position - transform.right * wayPointWidth / 2f;
        Vector3 randomPos = Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
        return randomPos;
    }
}
