using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*抽象状态，用于扩展实现敌人的一些基础状态*/
public abstract class EnemyBaseStats : MonoBehaviour
{
    public abstract void EnemyState(Enemy enemy);

    public abstract void OnUpdate(Enemy enemy);

}
