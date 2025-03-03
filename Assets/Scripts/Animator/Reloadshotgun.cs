using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reloadshotgun : StateMachineBehaviour
{
    public float reloadTime = 0.8f;
    private bool hasreload;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasreload = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (hasreload) return;

        if (stateInfo.normalizedTime >= reloadTime)
        {
            if (animator.GetComponent<Weapon_AutomaticGun>() != null)
            {
                animator.GetComponent<Weapon_AutomaticGun>().ShotGunReload();
            }
            hasreload = true;
        }

    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasreload = false;
    }

}
