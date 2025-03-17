using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Tooltip("敌人血量")] public float Health;
    [Tooltip("敌人伤害")] public float Damage;
    [Tooltip("敌人血条")] public Slider slider;
    [Tooltip("伤害UI")] public Text damageText;
    [Tooltip("死亡特效")] public GameObject deathEff;
    private NavMeshAgent agent;
    public Animator animator;
    private AudioSource audioSource;

    public GameObject[] wayPointObj;    //存放敌人巡逻路线
    public List<Vector3> wayPoints = new List<Vector3>();   //存放巡逻点

    public Transform targetPoint;

    public int animState;   //动画状态表示。0idel。1run。2attack
    public int index;  //下标值
    [Tooltip("分配的路线")] public int nameIndex;



    public EnemyBaseStats currentStats;     //敌人当前状态
    Vector3 targetpostion;
    public PatrolState patrolState;
    public AttackState attackState;

    private bool IsDeah;

    public List<Transform> attacklist = new List<Transform>();
    [Tooltip("攻击频率")] public float attackRate;
    private float nextattack;
    [Tooltip("攻击范围")] public float attackRange;

    public GameObject attackParticle01;
    public Transform attackParticle01Postion;
    public AudioClip attackSound;

    private void Awake()
    {
        // 动态添加组件
        patrolState = gameObject.AddComponent<PatrolState>();
        attackState = gameObject.AddComponent<AttackState>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        //敌人一开始进入巡逻状态
        TransitionToState(patrolState);
        IsDeah = false;
        slider.minValue = 0;
        slider.maxValue = Health;
        slider.value = Health;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDeah) return;
        currentStats.OnUpdate(this);
        animator.SetInteger("State", animState);
    }

    public void MoveToTaget()
    {
        if (attacklist.Count == 0)
        {
            targetpostion = Vector3.MoveTowards(transform.position, wayPoints[index], agent.speed * Time.deltaTime);
        }
        else
        {
            targetpostion = Vector3.MoveTowards(transform.position, attacklist[0].transform.position, agent.speed * Time.deltaTime);
        }

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

    public void Healthchange(float damage)
    {
        if (IsDeah) return;
        damageText.text = Mathf.Round(damage).ToString();
        Health -= damage;
        slider.value = Health;
        if (slider.value <= 0)
        {
            IsDeah = true;
            animator.SetTrigger("Dying");
        }
    }

    public void AttackAction()
    {
        if (Vector3.Distance(transform.position, targetPoint.position) < attackRange)
        {
            if (Time.time > nextattack)
            {
                //触发攻击
                animator.SetTrigger("Attack");
                nextattack = Time.deltaTime + attackRate;
            }
        }
    }

    public void PlayMutantAttackEFF()
    {
        if (gameObject.name == "Mutant")
        {
            GameObject gameObject = Instantiate(attackParticle01, attackParticle01Postion.position, attackParticle01Postion.rotation);
            PlayAttackSound();
            Destroy(attackParticle01, 3f);
        }
    }

    public void PlayAttackSound()
    {
        audioSource.clip = attackSound;
        audioSource.Play();
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!attacklist.Contains(collider.transform) && !IsDeah && !collider.transform.CompareTag("Bullet"))
        {
            attacklist.Add(collider.transform);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        attacklist.Remove(collider.transform);
    }
}
