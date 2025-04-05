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
        pCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player")); //忽略玩家collider -> 解決物體被玩家身體檔到
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX; // 獲得數標x
        float mouseY = Input.GetAxis("Mouse Y") * sensY; // 獲得數標y
        targetYRotation += mouseX; // Y-Rotation: 左右旋轉 所以拿x
        targetXRotation -= mouseY;
        targetXRotation = Mathf.Clamp(targetXRotation, -89f, 89f); // 限制向上、下看最大角度 在90/-90度時會過頭，故所以使用89/-89

        xRotation = Mathf.Lerp(xRotation, targetXRotation, Time.deltaTime * smoothTime); // 依照偵數平滑過渡
        yRotation = Mathf.Lerp(yRotation, targetYRotation, Time.deltaTime * smoothTime);
        
        transform.rotation = Quaternion.Euler(xRotation,yRotation,0); // transform.rotation 只能透過歐拉角設置
        // orientation.rotation = Quaternion.Euler(0,yRotation,0);
        playerDirection.rotation = Quaternion.Euler(0, yRotation, 0); // 玩家身體旋轉
    }
}
