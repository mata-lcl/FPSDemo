using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyBaseStats
{
    public override void EnemyState(Enemy enemy)
    {
        enemy.animState = 2;
        enemy.targetPoint = enemy.attacklist[0];
    }

    public override void OnUpdate(Enemy enemy)
    {
        //当前敌人没有目标，切换回巡逻状态
        if (enemy.attacklist.Count == 0)
        {
            enemy.TransitionToState(enemy.patrolState);
        }

        //当前敌人有目标，可能存在多个目标，寻找距离最近的攻击目标
        // if (enemy.attacklist.Count > 1)
        // {
        //     for (int i = 0; i < enemy.attacklist.Count; i++)
        //     {
        //         Mathf.Abs(enemy.transform.position.x - enemy.attacklist[i].transform.position.x);
        //     }
        // }

        //当前敌人只有一个攻击目标，就只找List中的第一个
        if (enemy.attacklist.Count == 1)
        {
            enemy.targetPoint = enemy.attacklist[0];
        }

        if (enemy.targetPoint.CompareTag("Player"))
        {
            //敌人对玩家进行攻击
            enemy.AttackAction();
        }

        enemy.MoveToTaget();
    }
}
