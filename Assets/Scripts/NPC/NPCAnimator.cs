using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimator : MonoBehaviour
{
    Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("no animator found");
        }
    }
    
    public void NPC_WalkAnimation()
    {
        ResetAnimation();
        _animator.SetBool("walk", true);
    }
    
    public void NPC_RunAnimation()
    {
        ResetAnimation();
        _animator.SetBool("run", true);
    }
    
    public void NPC_GrabAnimation() //拿取商品動畫
    {
        ResetAnimation();
        _animator.SetTrigger("grab");
    }
    
    public void NPC_PutdownNormalAnimation() //放下商品動畫-正常
    {
        ResetAnimation();
        _animator.SetTrigger("putdownNormal");
    }
    
    public void NPC_PutdownSlothAnimation() //放下商品動畫-懶人
    {
        ResetAnimation();
        _animator.SetBool("putdownSloth", true);
    }
    public void NPC_ThrowAnimation() //放下商品動畫-爆怒
    {
        ResetAnimation();
        _animator.SetTrigger("throw");
    }
    
    public void ResetAnimation() //放下NPC狀態機
    {
        _animator.SetBool("idle", true);
        _animator.SetBool("walk", false);
        _animator.SetBool("run", false);
        _animator.SetBool("putdownSloth", false);
    }
}
