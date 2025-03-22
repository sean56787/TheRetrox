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

    // public void NPC_IdleAnimation()
    // {
    //     Debug.Log("idle animation");
    //     ResetAnimation();
    //     _animator.SetBool("idle", true);
    // }
    public void NPC_WalkAnimation()
    {
        ResetAnimation();
        _animator.SetBool("walk", true);
    }
    
    public void NPC_GrabAnimation()
    {
        ResetAnimation();
        _animator.SetTrigger("grab");
    }
    
    public void NPC_PutdownAnimation()
    {
        ResetAnimation();
        _animator.SetBool("putdown", true);
    }
    
    public void NPC_ThrowAnimation()
    {
        ResetAnimation();
        _animator.SetTrigger("throw");
    }
    
    public void NPC_PayAnimation()
    {
        ResetAnimation();
        _animator.SetTrigger("pay");
    }
    
    public void ResetAnimation()
    {
        _animator.SetBool("idle", true);
        _animator.SetBool("walk", false);
        _animator.SetBool("putdown", false);
    }

    // public float GetAnimationLength(string animationTag)
    // {
    //     AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
    //     if (stateInfo.IsTag(animationTag))
    //     {
    //         return stateInfo.length;
    //     }
    //     Debug.LogError("no animation clip found: "+ animationTag);
    //     return 0f;
    // }
}
