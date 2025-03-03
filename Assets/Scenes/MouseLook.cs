using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Tooltip("视野灵敏度")] public float mouseSensitivity = 400f;
    private Transform playerBody; //玩家位置
    private float yRotation = 0f; //摄像机上下旋转的数值
    private CharacterController characterController;
    public float cameraheight = 1.8f;
    private float interpolationSpeed = 12f; //高度变化平滑值



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //锁定鼠标并隐藏
        playerBody = transform.GetComponentInParent<PlayerController>().transform;
        characterController = GetComponentInParent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        float mouse_x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouse_y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        yRotation -= mouse_y;   //将上下旋转轴值进行累计
        yRotation = Mathf.Clamp(yRotation, -60f, 90f);
        transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);  //摄像机上下旋转
        playerBody.Rotate(Vector3.up * mouse_x);    //玩家左右移动

        //下蹲时摄像机高度发生变化
        float heightTarget = characterController.height * 0.9f;
        cameraheight = Mathf.Lerp(cameraheight, heightTarget, interpolationSpeed * Time.deltaTime);
        transform.localPosition = Vector3.up * cameraheight;
    }
}
