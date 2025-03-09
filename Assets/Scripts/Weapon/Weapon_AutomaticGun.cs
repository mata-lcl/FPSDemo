using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using static Weapon_AutomaticGun;

/*内部类：音效*/
[System.Serializable]
public class Soundclips
{
    public AudioClip shootsound;   //开火音效
    public AudioClip silencershootsound;   //消音器开火
    public AudioClip reloadsoundAmmotLeft;   //换弹
    public AudioClip reloadSoundOutOfAmmo;   //换弹拉栓
    public AudioClip AimSound;   //瞄准

}
public class Weapon_AutomaticGun : Weapon
{
    public PlayerController playerController;
    private Camera mainCamera;
    public Camera GunCamera;
    [Header("武器部件位置")]
    [Tooltip("射击位置")] public Transform ShootPoint;   //射线打出的位置
    [Tooltip("特效位置")] public Transform BulletShootPoint;    //子弹打出的位置
    [Tooltip("弹壳位置")] public Transform CasingBulletSpawnPoint;  //弹壳抛出位置

    [Header("子弹预制体和特效")]
    public Transform BulletPrefab;  //子弹
    public Transform CasingPrefab;  //子弹抛壳

    [Header("枪械属性")]
    [Tooltip("武器射程")] public float range;  //武器射程
    [Tooltip("武器射速")] public float firerate;
    [Tooltip("最小伤害")] public float mindamage;
    [Tooltip("最大伤害")] public float maxdamage;
    private float originRate;   //原始射速
    private float SpreadFactor;   //射击偏移
    private float fireTimer;   //计时器，控制武器射速
    private float BulletForce;   //子弹发射力
    public bool IsSilencer;     //是否装备消音器
    public bool Is_AUTORIFLE;   //是否是全自动武器
    public bool Is_SEMIGUN;     //是否色色半自动武器
    [Tooltip("弹夹子弹")] public int BulletMag;
    [Tooltip("弹夹剩余子弹")] public int CurrentBullet;
    [Tooltip("备弹")] public int BulletLeft;

    [Header("特效")]
    [Tooltip("开火灯光")] public Light MuzzleFlashLight;
    [Tooltip("火光持续时间")] private float LightDuration;
    [Tooltip("开火特效0")] public ParticleSystem MuzzlePartic;
    [Tooltip("开火特效1")] public ParticleSystem SparkPartic;
    private int MinSparkEmission = 1;
    private int MaxSparkEmission = 7;

    [Header("音效")]
    private AudioSource mainAudioSource;
    public Soundclips soundclip;

    [Header("UI")]
    public UnityEngine.UI.Image[] crossQuarterImgs;    //准星
    public float CurrentExpanedDegree;  //当前准星开合度
    private float crossExpanedDegree;   //每帧准星开合度
    private float MaxCrossExpaned;  //最大开合度
    public Text AmmoTextUI;
    public Text ShootModeTextUI;
    [Header("狙击镜瞄准")]
    public Material ScopeRenderMaterial;
    public Color fadecolor;
    public Color defaultcolor;

    public PlayerController.MovementState state;
    private Animator animator;

    //使用枚举区分自动半自动
    public ShootModeB ShootMODE;
    private bool GunShootInput;
    private int modeNum;    //模式切换参数（1.全自动 0.半自动）
    private string shootmodename;

    public bool IsReloading;
    public bool IsAiming;  //是否瞄准
    private Vector3 OriginPosition; //瞄准初始位置
    public Vector3 AimingPosition; //瞄准位置

    private int ShootGunBullet = 8;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
        mainAudioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
    }
    void Start()
    {
        OriginPosition = transform.localPosition;
        IsReloading = false;
        MuzzleFlashLight.enabled = false;
        BulletForce = 100f;
        crossExpanedDegree = 50f;
        MaxCrossExpaned = 300f;
        LightDuration = 0.02f;
        originRate = 0.15f;
        range = 300f;
        BulletLeft = BulletMag * 5;
        CurrentBullet = BulletMag;
        UpdateAmmoUI();

        if (Is_AUTORIFLE)
        {
            modeNum = 1;
            shootmodename = "全自动";
            ShootMODE = ShootModeB.AutoRifle;
            UpdateAmmoUI();
        }
        if (Is_SEMIGUN)
        {
            modeNum = 0;
            shootmodename = "半自动";
            ShootMODE = ShootModeB.SemiGun;
            UpdateAmmoUI();
        }
    }

    void Update()
    {
        if (Is_AUTORIFLE)
        {
            if (Input.GetKeyDown(KeyCode.B) && modeNum != 1)
            {
                modeNum = 1;
                shootmodename = "全自动";
                ShootMODE = ShootModeB.AutoRifle;
                UpdateAmmoUI();
            }
            else if (Input.GetKeyDown(KeyCode.B) && modeNum != 0)
            {
                modeNum = 0;
                shootmodename = "半自动";
                ShootMODE = ShootModeB.SemiGun;
                UpdateAmmoUI();
            }
            switch (ShootMODE)
            {
                case ShootModeB.AutoRifle:
                    GunShootInput = Input.GetMouseButton(0);
                    firerate = originRate;
                    break;
                case ShootModeB.SemiGun:
                    GunShootInput = Input.GetMouseButtonDown(0);
                    firerate = 0.3f;
                    break;
            }
        }
        else
        {
            GunShootInput = Input.GetMouseButtonDown(0);
        }

        state = playerController.State;
        if (state == PlayerController.MovementState.Walking && Vector3.SqrMagnitude(playerController.MovingDirction) > 0)
        {
            ExpandingCrossUpdate(crossExpanedDegree);
        }
        else if (state == PlayerController.MovementState.Running)
        {
            ExpandingCrossUpdate(crossExpanedDegree * 2);
        }
        else
        {
            ExpandingCrossUpdate(0);
        }

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if
        (info.IsName("reload_ammo_left") ||
         info.IsName("reload_out_of_ammo") ||
         info.IsName("reload_open") ||
         info.IsName("reload_close") ||
         info.IsName("reload_insert0") ||
         info.IsName("reload_insert1") ||
         info.IsName("reload_insert2") ||
         info.IsName("reload_insert3") ||
         info.IsName("reload_insert4"))

        {
            IsReloading = true;
        }
        else
        {
            IsReloading = false;
        }

        if (
         (info.IsName("reload_insert0") ||
         info.IsName("reload_insert1") ||
         info.IsName("reload_insert2") ||
         info.IsName("reload_insert3") ||
         info.IsName("reload_insert4")) &&
         CurrentBullet == BulletMag)
        {
            animator.Play("reload_close");
            IsReloading = false;
        }

        SpreadFactor = (IsAiming) ? 0.01f : 0.1f;   //不同状态下的射击误差

        if (Input.GetMouseButtonDown(1) && !IsReloading && !playerController.IsRun)
        {
            IsAiming = !IsAiming;
            animator.SetBool("Aim", IsAiming);
            if (IsAiming)
            {
                transform.localPosition = AimingPosition;
            }
            else
            {
                transform.localPosition = OriginPosition;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && CurrentBullet < BulletMag && !IsReloading)
        {
            DoReloadAnimation();
            return;
        }

        if (GunShootInput && CurrentBullet > 0)
        {
            if (Is_SEMIGUN && gameObject.name == "3")
            {
                ShootGunBullet = 8;
            }
            else
            {
                ShootGunBullet = 1;
            }
            GunFire();
        }

        animator.SetBool("Run", playerController.IsRun);
        animator.SetBool("Walk", playerController.IsWalk);


        //计时器
        if (fireTimer < firerate)
        {
            fireTimer += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            animator.SetTrigger("Inspect");
        }

    }
    public override void AimIn()
    {
        float currentVelocity = 0f;
        for (int i = 0; i < crossQuarterImgs.Length; i++)
        {
            crossQuarterImgs[i].gameObject.SetActive(false);    //隐藏
        }

        if (Is_SEMIGUN && (gameObject.name == "4"))
        {
            ScopeRenderMaterial.color = defaultcolor;
            GunCamera.fieldOfView = 15;
        }
        mainCamera.fieldOfView = Mathf.SmoothDamp(30, 45, ref currentVelocity, 0.3f);         //smoothdamp平滑
        mainAudioSource.clip = soundclip.AimSound;
        mainAudioSource.Play();

    }

    public override void AimOut()
    {
        float currentVelocity = 0f;
        for (int i = 0; i < crossQuarterImgs.Length; i++)
        {
            crossQuarterImgs[i].gameObject.SetActive(true);
        }
        mainCamera.fieldOfView = Mathf.SmoothDamp(45, 30, ref currentVelocity, 0.3f);         //smoothdamp平滑

        if (Is_SEMIGUN && (gameObject.name == "4"))
        {
            ScopeRenderMaterial.color = fadecolor;
            GunCamera.fieldOfView = 45;
        }

        mainAudioSource.clip = soundclip.AimSound;
        mainAudioSource.Play();

    }

    public override void DoReloadAnimation()
    {
        if (!(Is_SEMIGUN && (gameObject.name == "3" || gameObject.name == "4")))
        {
            if (CurrentBullet > 0 && BulletLeft > 0)
            {
                AimOut();
                animator.Play("reload_ammo_left", 0, 0);
                Reload();
                mainAudioSource.clip = soundclip.reloadsoundAmmotLeft;
                mainAudioSource.Play();
            }
            if (CurrentBullet == 0 && BulletLeft > 0)
            {
                AimOut();
                animator.CrossFadeInFixedTime("reload_out_of_ammo", 0.1f, 0);
                Reload();
                mainAudioSource.clip = soundclip.reloadSoundOutOfAmmo;
                mainAudioSource.Play();
            }
        }
        else
        {
            if (CurrentBullet == BulletMag) return;
            animator.SetTrigger("ShotGun_Reload");
        }

    }

    public void ShotGunReload()
    {
        if (CurrentBullet < BulletMag && BulletLeft > 0)
        {
            CurrentBullet++;
            BulletLeft--;
            UpdateAmmoUI();
        }
        else
        {
            animator.Play("reload_close");
        }
        if (BulletLeft <= 0) return;
    }

    public override void ExpandingCrossUpdate(float expanDegree)
    {
        if (CurrentExpanedDegree < expanDegree - 5)
        {
            ExpanedCross(150 * Time.deltaTime);
        }
        else if (CurrentExpanedDegree > expanDegree + 5)
        {
            ExpanedCross(-300 * Time.deltaTime);
        }
    }


    public override void GunFire()
    {
        if (fireTimer < firerate || CurrentBullet < 0 || animator.GetCurrentAnimatorStateInfo(0).IsName("take_out") || IsReloading) return;

        StartCoroutine(MuzzleFlashlight()); //调用携程开火灯光
        MuzzlePartic.Emit(1);  //发射1个粒子
        SparkPartic.Emit(Random.Range(MinSparkEmission, MaxSparkEmission));
        StartCoroutine(Shoot_Cross());

        if (mainAudioSource == null)
        {
            Debug.LogError("mainAudioSource 未初始化");
            return;
        }

        if (!IsAiming)
        {
            animator.CrossFadeInFixedTime("fire", 0.1f);//开火动画
        }
        else
        {
            animator.CrossFadeInFixedTime("aim_fire", 0, 0);
        }

        for (int i = 0; i < ShootGunBullet; i++)
        {
            RaycastHit hit;
            Vector3 ShootDirection = ShootPoint.forward;
            ShootDirection = ShootDirection + ShootPoint.TransformDirection(new Vector3(Random.Range(-SpreadFactor, SpreadFactor), Random.Range(-SpreadFactor, SpreadFactor)));
            if (Physics.Raycast(ShootPoint.position, ShootDirection, out hit))
            {
                Transform bullet;
                if (Is_AUTORIFLE || (Is_SEMIGUN && gameObject.name == "2"))
                {
                    bullet = Instantiate(BulletPrefab, BulletShootPoint.position, BulletShootPoint.rotation);
                }
                else
                {
                    bullet = Instantiate(BulletPrefab, hit
                    .point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    //霰弹枪特殊处理，将子弹位置设定到hit.point击中的位置
                }

                bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward + ShootDirection) * BulletForce;
                //击中敌人时的判断
                if (hit.transform.gameObject.transform.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<Enemy>().Healthchange(Random.Range(mindamage, maxdamage));
                }
                Debug.Log(hit.transform.gameObject.name + "被击中");
            }
        }

        Instantiate(CasingPrefab, CasingBulletSpawnPoint.position, CasingBulletSpawnPoint.rotation);
        mainAudioSource.clip = IsSilencer ? soundclip.silencershootsound : soundclip.shootsound;
        mainAudioSource.Play();
        CurrentBullet--;
        UpdateAmmoUI();
        fireTimer = 0;


    }

    public IEnumerator MuzzleFlashlight()
    {
        MuzzleFlashLight.enabled = true;
        yield return new WaitForSeconds(LightDuration);
        MuzzleFlashLight.enabled = false;
    }

    public override void Reload()
    {
        if (BulletLeft < 0) return;
        // if (animator.GetCurrentAnimatorStateInfo(0).IsName("reload_ammo_left") || animator.GetCurrentAnimatorStateInfo(0).IsName("reload_out_of_ammo")) return;


        //计算需要填充的子弹 每个弹夹子弹数-弹夹剩余的子弹数
        int bulletToLoad = BulletMag - CurrentBullet;
        //计算备弹扣除的子弹数
        int BulletToReduce = BulletLeft >= bulletToLoad ? bulletToLoad : BulletLeft;
        BulletLeft -= BulletToReduce;
        CurrentBullet += BulletToReduce;
        UpdateAmmoUI();
    }

    public void ExpanedCross(float add)
    {
        crossQuarterImgs[0].transform.localPosition += new Vector3(-add, 0, 0);//左准星
        crossQuarterImgs[1].transform.localPosition += new Vector3(add, 0, 0);//右准星
        crossQuarterImgs[2].transform.localPosition += new Vector3(0, add, 0);//上准星
        crossQuarterImgs[3].transform.localPosition += new Vector3(0, -add, 0);//左准星

        CurrentExpanedDegree += add;
        CurrentExpanedDegree = Mathf.Clamp(CurrentExpanedDegree, 0, MaxCrossExpaned);//限制开合度大小
    }

    public IEnumerator Shoot_Cross()
    {
        yield return null;
        for (int i = 0; i < 10; i++)
        {
            ExpanedCross(Time.deltaTime * 500);
        }
    }

    public void UpdateAmmoUI()
    {
        AmmoTextUI.text = CurrentBullet + "/" + BulletLeft;
        ShootModeTextUI.text = shootmodename;
    }

    public enum ShootModeB
    {
        AutoRifle,
        SemiGun
    };
}
