using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Header("ground check")]
    public float groundDrag;
    public float playerHeight;
    public LayerMask groundLayer;
    public bool isGrounded;
    
    [Header("movement")]
    private float _moveSpeed;
    public float walkSpeed = 7;
    public float sprintSpeed = 10;
    public Transform playerDirection;
    private float _horizontalInput;
    private float _verticalInput;
    Vector3 _moveDirection;
    private Rigidbody _rb;
    public KeyCode sprintKey = KeyCode.LeftShift;
    
    [Header("jump")]
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    public bool readyToJump;

    [Header("how to step on stair")]
    public GameObject stepRaycastHigh;
    public GameObject stepRaycastLow;
    public float stepHeight = 0.3f;
    public float stepSmooth = 0.1f;
    public float maxSlopeAngle = 60;
    RaycastHit _slopeHit;
    [Header("friction")]
    public Collider playerCollider;
    private void Awake()
    {
        stepRaycastHigh.transform.position = new Vector3(stepRaycastHigh.transform.position.x, 
            stepRaycastHigh.transform.position.y + stepHeight,
            stepRaycastHigh.transform.position.z);
    }
    
    private void Start()
    {
        Transform playerBody = transform.Find("body");
        if (playerBody != null) // 使用PhysicMaterial 減少玩家與物體摩擦
        {
            playerCollider = playerBody.GetComponent<Collider>();
            if (playerCollider)
            {
                PhysicMaterial noFriction = new PhysicMaterial();
                noFriction.dynamicFriction = 0f;
                noFriction.staticFriction = 0f;
                noFriction.frictionCombine = PhysicMaterialCombine.Minimum;
                playerCollider.material = noFriction;
            }
        }
        
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        Invoke(nameof(ResetJump), jumpCoolDown);
    }

    private void Update()
    {
        // drag 內部減速 不影響PhysicMat
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f,groundLayer); //雷射檢查是否觸地
        KeyInput(); // WASD:移動 SPACE:跳
        SpeedControl(); // 限制速度
        CheckMovementState(); // 檢查走路或跑步
        if (isGrounded)
            _rb.drag = groundDrag;
        else
            _rb.drag = 0; // 在空中摩擦:0
        _rb.useGravity = !IsOnSlope(); // 上坡先關掉重力
    }

    private void FixedUpdate() // 物理移動應放在FixedUpdate
    {
        MovePlayer();
        //ClimbStair();
    }

    private void KeyInput() // WASD:移動 SPACE:跳
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown); // 跳躍冷卻
        }
    }

    private bool IsOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return slopeAngle < maxSlopeAngle && slopeAngle != 0;
        }
        return false;
    }
    
    private Vector3 VectorCrossGetSlopeDir()
    {
        Vector3 groundNormal = _slopeHit.normal; // 取得坡面法線
        Vector3 rightVector = Vector3.Cross(Vector3.up, groundNormal).normalized; // 計算坡面「橫向」方向
        // 右手法則
        Vector3 slopeForward = Vector3.Cross(groundNormal, rightVector).normalized; // 計算坡度「向上移動」方向

        return ((_horizontalInput * rightVector) + (_verticalInput * slopeForward)).normalized;
    }

    private Vector3 ProjectOnPlaneGetSlopeDir()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized; // 獲得上坡方向
    }
    
    private void MovePlayer()
    {
        _moveDirection = playerDirection.forward * _verticalInput + playerDirection.right * _horizontalInput; // 原始方向
        if (IsOnSlope()) // 在走上坡
        {
            _rb.AddForce(ProjectOnPlaneGetSlopeDir() * _moveSpeed, ForceMode.Force);
            // _rb.AddForce(VectorCrossGetSlopeDir() * _moveSpeed, ForceMode.Force);
        }
        
        if(isGrounded) // 在地面
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed, ForceMode.Force); //  ForceMode.Force持續推力
            if (SoundManager.instance != null && 
                SoundManager.instance.canPlayPlayerFootstep && 
                _moveDirection != Vector3.zero)
            {
                StartCoroutine(SoundManager.instance.PlayClip_PlayerWalk());
            }
        }
        else
        {
            _rb.AddForce(_moveDirection.normalized * (_moveSpeed * airMultiplier), ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if (flatVel.magnitude > _moveSpeed) // 限制目前速度 在 _moveSpeed
        {
            Vector3 limitedVel = flatVel.normalized * _moveSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }

    void CheckMovementState()
    {
        if (isGrounded && Input.GetKey(sprintKey))
        {
            _moveSpeed = sprintSpeed;
            SoundManager.instance.playerFootstepDelay = 0.35f;
        }
        else if (isGrounded)
        {
            _moveSpeed = walkSpeed;
            SoundManager.instance.playerFootstepDelay = 0.5f;
        }
    }
    private void Jump() // 跳
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z); // 重置Y
        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    void ClimbStair()
    {
        Debug.DrawRay(stepRaycastLow.transform.position, transform.TransformDirection(Vector3.forward) * 0.3f, Color.red); 
        Debug.DrawRay(stepRaycastHigh.transform.position, transform.TransformDirection(Vector3.forward) * 0.4f, Color.green); 
        
        RaycastHit hitLower;
        if(Physics.Raycast(stepRaycastLow.transform.position, transform.TransformDirection(Vector3.forward),out hitLower, 0.3f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRaycastHigh.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.4f))
            {
                _rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        
        Debug.DrawRay(stepRaycastLow.transform.position, transform.TransformDirection(1.5f,0,1) * 0.3f, Color.red); 
        Debug.DrawRay(stepRaycastHigh.transform.position, transform.TransformDirection(1.5f,0,1) * 0.4f, Color.green); 
        RaycastHit hitLower45;
        if(Physics.Raycast(stepRaycastLow.transform.position, transform.TransformDirection(1.5f,0,1),out hitLower45, 0.3f))
        {
            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRaycastHigh.transform.position, transform.TransformDirection(1.5f,0,1), out hitUpper45, 0.4f))
            {
                _rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        
        Debug.DrawRay(stepRaycastLow.transform.position, transform.TransformDirection(-1.5f,0,1) * 0.3f, Color.red); 
        Debug.DrawRay(stepRaycastHigh.transform.position, transform.TransformDirection(-1.5f,0,1) * 0.4f, Color.green); 
        RaycastHit hitLowerMinus45;
        if(Physics.Raycast(stepRaycastLow.transform.position, transform.TransformDirection(-1.5f,0,1),out hitLowerMinus45, 0.3f))
        {
            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRaycastHigh.transform.position, transform.TransformDirection(-1.5f,0,1), out hitUpperMinus45, 0.4f))
            {
                _rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }
}
