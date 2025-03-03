using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMSClearSingnal : StateMachineBehaviour
{
    public string[] ClearEnter;
    public string[] ClearExit;
    public AudioClip soundclip;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 获取 AudioSource 组件
        AudioSource audioSource = animator.gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource 组件未找到");
            return;
        }

        // 检查 soundclip 是否已设置
        if (soundclip == null)
        {
            Debug.LogError("soundclip 未设置");
            return;
        }

        // 设置音频剪辑并播放
        audioSource.clip = soundclip;
        audioSource.Play();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // for (int i = 0; i < ClearExit.Length; i++)
        // {
        //     animator.ResetTrigger(ClearExit[i]);
        // }
        foreach (string signal in ClearExit)
        {
            animator.ResetTrigger(signal);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
