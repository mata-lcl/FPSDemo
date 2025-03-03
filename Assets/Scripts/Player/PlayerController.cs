using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    public Vector3 MovingDirction;
    private AudioSource audioSource;

    [Header("玩家属性")]
    public float Speed;
    [Tooltip("行走速度")] public float WalkSpeed = 2.0f;
    [Tooltip("奔跑速度")] public float RunSpeed = 4.0f;
    [Tooltip("蹲走速度")] public float CrouchSpeed = 1.0f;
    [Tooltip("跳跃力")] public float JumpForce = 0f;
    [Tooltip("重力")] public float FallForce = 9.8f;
    [Tooltip("蹲下高度")] public float CrouchedHeight;
    [Tooltip("站起高度")] public float StandHeight = 1.8f;




    [Header("键位设置")]
    [Tooltip("奔跑")] public KeyCode RunInputName = KeyCode.LeftShift;
    [Tooltip("跳跃")] public KeyCode JumpInputName = KeyCode.Space;
    [Tooltip("下蹲")] public KeyCode crouchInputName = KeyCode.LeftControl;

    [Header("属性判断")]
    public MovementState State;
    private CollisionFlags collisionFlags;
    public bool IsWalk;
    public bool IsRun;
    public bool IsJump;
    public bool IsCrouch;
    public bool CanStandUp;
    public bool IsGround;
    public LayerMask crouchLayerMask;

    [Header("音效")]
    [Tooltip("行走音效")] public AudioClip WalkSound;
    [Tooltip("奔跑音效")] public AudioClip RunSound;

    private Inventory inventory;
    private Weapon_AutomaticGun weaponGun;

    // private GameObject nowweapon;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        StandHeight = characterController.height;
        CrouchedHeight = 1f;
        audioSource = GetComponent<AudioSource>();
        inventory = GetComponentInChildren<Inventory>();

    }


    // Update is called once per frame
    void Update()
    {

        CheckCanStandUp();
        if (Input.GetKey(crouchInputName))
        {
            Crouch(true);
        }
        else
        {
            Crouch(false);
        }
        Palyersound();
        Jump();
        Movement();
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     inventory.ThrowWeapon(0);
        // }

    }


    //人物移动
    public void Movement()
    {
        float H = Input.GetAxis("Horizontal");
        float V = Input.GetAxis("Vertical");

        // 计算方向
        MovingDirction = (transform.right * H + transform.forward * V).normalized;

        // 仅当有输入时才更新状态
        if (MovingDirction.sqrMagnitude > 0)
        {
            if (Input.GetKey(RunInputName) && IsGround && !IsCrouch) // 只有地面状态才能跑步
            {
                State = MovementState.Running;
                Speed = RunSpeed;
            }
            else if (IsCrouch)
            {
                State = MovementState.Crouching;
                Speed = CrouchSpeed;
            }
            else
            {
                State = MovementState.Walking;
                Speed = WalkSpeed;
            }
        }
        else
        {
            State = MovementState.Idle;
            Speed = 0f; // 站立时不移动
        }
        // 应用移动
        characterController.Move(MovingDirction * Speed * Time.deltaTime);
    }


    public void Jump()
    {
        if (!CanStandUp) return;
        if (Input.GetKeyDown(JumpInputName) && IsGround)
        {
            JumpForce = 5.0f;
            IsGround = false;
        }

        if (!IsGround)
        {
            JumpForce -= FallForce * Time.deltaTime;
            Vector3 jump = new Vector3(0, JumpForce * Time.deltaTime, 0);
            collisionFlags = characterController.Move(jump);

            if ((collisionFlags & CollisionFlags.Below) != 0)
            {
                IsGround = true;
                JumpForce = 0f;
            }
        }
    }

    public void CheckCanStandUp()
    {
        // 使用站立时的高度进行检测
        float checkHeight = StandHeight;
        Vector3 sphereLocation = transform.position + Vector3.up * (checkHeight / 2 + 0.1f);
        float detectionRadius = characterController.radius * 1.5f;

        Collider[] colis = Physics.OverlapSphere(sphereLocation, detectionRadius, crouchLayerMask);
        CanStandUp = true;

        foreach (var col in colis)
        {
            if (col.gameObject != gameObject) // 过滤自身
            {
                CanStandUp = false;
            }
        }
    }



    public void Crouch(bool newcrouch)
    {
        if (!newcrouch)
        {
            CheckCanStandUp();
            if (!CanStandUp)
            {
                return; // 不能站立时不允许恢复高度
            }
        }
        IsCrouch = newcrouch;

        // 设定高度
        characterController.height = IsCrouch ? CrouchedHeight : StandHeight;

        // 修正角色中心点，确保不会漂浮或卡地面
        characterController.center = new Vector3(0, characterController.height / 2, 0);
    }


    public void Palyersound()
    {
        if (IsGround && MovingDirction.sqrMagnitude > 0)
        {
            audioSource.clip = IsRun ? RunSound : WalkSound;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
        if (IsCrouch)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }

    }

    public void PickUpWeapon(int itemID, GameObject weapon)
    {
        if (inventory.weapons.Contains(weapon))
        {
            weaponGun = GetComponentInChildren<Weapon_AutomaticGun>();
            if (weaponGun != null)
            {
                weaponGun.BulletLeft = weaponGun.BulletMag * 5;
                weaponGun.UpdateAmmoUI();
                Debug.Log("已存在枪械，补充后备子弹");
            }
            else
            {
                Debug.LogError("Weapon_AutomaticGun component is null.");
            }
        }
        else
        {
            inventory.AddWeapon(itemID, weapon);
        }
    }

    public enum MovementState
    {
        Walking,
        Running,
        Crouching,
        Idle
    }
}

