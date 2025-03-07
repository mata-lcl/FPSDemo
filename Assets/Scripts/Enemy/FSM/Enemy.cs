using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator animator;

    public GameObject[] wayPointObj;    //存放敌人巡逻路线
    public List<Vector3> wayPoints = new List<Vector3>();   //存放巡逻点

    public int animState;   //动画状态表示。0idel。1run。2attack
    public int index;  //下标值


    public EnemyBaseStats currentStats;     //敌人当前状态
    Vector3 targetpostion;
    private PatrolState patrolState;
    private AttackState attackState;

    private void Awake()
    {
        // 动态添加组件
        patrolState = gameObject.AddComponent<PatrolState>();
        attackState = gameObject.AddComponent<AttackState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        index = 0;
        //敌人一开始进入巡逻状态
        TransitionToState(patrolState);
    }

    // Update is called once per frame
    void Update()
    {
        currentStats.OnUpdate(this);
        animator.SetInteger("State", animState);
    }

    public void MoveToTaget()
    {
        targetpostion = Vector3.MoveTowards(transform.position, wayPoints[index], agent.speed * Time.deltaTime);
        agent.destination = targetpostion;
    }

    public void LoadPath(GameObject go)
    {
        //加载路线之前清空LIST
        wayPoints.Clear();
        //遍历路线预制体中所有导航点的位置信息，并添加到list中
        foreach (Transform t in go.transform)
        {
            wayPoints.Add(t.position);
        }
    }

    public void TransitionToState(EnemyBaseStats stats)
    {
        currentStats = stats;
        currentStats.EnemyState(this);
    }
}
