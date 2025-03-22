using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public float smoothTime = 5f;
    public Transform playerDirection;
    float xRotation;
    float yRotation;
    float targetXRotation;
    float targetYRotation;

    private void Start()
    {
        Camera pCamera = GetComponent<Camera>();
        pCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));//忽略玩家collider -> 解決太靠近物體時 物體被切
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * sensY;
        targetYRotation += mouseX;
        targetXRotation -= mouseY;
        targetXRotation = Mathf.Clamp(targetXRotation, -89f, 89f);

        xRotation = Mathf.Lerp(xRotation, targetXRotation, Time.deltaTime * smoothTime);
        yRotation = Mathf.Lerp(yRotation, targetYRotation, Time.deltaTime * smoothTime);
        
        transform.rotation = Quaternion.Euler(xRotation,yRotation,0);
        // orientation.rotation = Quaternion.Euler(0,yRotation,0);
        playerDirection.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
