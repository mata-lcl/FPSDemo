using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseStats
{
    public override void EnemyState(Enemy enemy)
    {
        enemy.animState = 0;
        enemy.LoadPath(enemy.wayPointObj[0]);
    }

    public override void OnUpdate(Enemy enemy)
    {
        if (!enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            enemy.animState = 1;
            enemy.MoveToTaget();
        }

        //计算敌人和导航点的距离
        float Distance = Vector3.Distance(enemy.transform.position, enemy.wayPoints[enemy.index]);
        if (Distance <= 0.5f)
        {
            enemy.animState = 0;
            enemy.animator.Play("Idle");

            enemy.index++;
            enemy.index = Mathf.Clamp(enemy.index, 0, enemy.wayPoints.Count - 1);
            if ((Vector3.Distance(enemy.transform.position, enemy.wayPoints[enemy.wayPoints.Count - 1]) < 0.5f))
            {
                enemy.index = 0;
            }
        }
        //Debug.Log(Distance);
    }
}
