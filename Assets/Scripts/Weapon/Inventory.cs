using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //武器库
    public List<GameObject> weapons;
    public int currentWeaponID;

    private Weapon_AutomaticGun aimout;

    public Weapon_AutomaticGun Is_Aiming;


    // Start is called before the first frame update
    void Start()
    {
        weapons = new List<GameObject>();
        currentWeaponID = -1;
        //transform.Find("Weapon_AutomaticGun").gameObject.SetActive(true);
        //aimout = GetComponentInChildren<Weapon_AutomaticGun>();
    }

    // Update is called once per frame
    void Update()
    {
        ChargeCurrentWeaponID();
    }

    public void ChargeCurrentWeaponID()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        //-0.1向下 0不动 0.1向上
        if (scroll < 0)
        {
            ChargeWeapon(currentWeaponID + 1);
        }
        else if (scroll > 0)
        {
            ChargeWeapon(currentWeaponID - 1);
        }

        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                int num = 0;
                if (i == 10)
                {
                    num = 10;
                }
                else
                {
                    num = i - 1;
                }
                if (num < weapons.Count)
                {
                    ChargeWeapon(num);
                }
            }
        }
    }

    public void ChargeWeapon(int weaponID)
    {
        if (weapons.Count == 0)
        {
            Debug.Log("列表为空");
            return;
        }

        // 在切换前重置所有处于激活状态的枪械的瞄准状态
        for (int i = 0; i < weapons.Count; i++)
        {
            if (!weapons[i].activeInHierarchy)  // 仅对已激活的武器调用 AimOut()
                continue;

            Weapon_AutomaticGun gun = weapons[i].GetComponent<Weapon_AutomaticGun>();
            if (gun != null)
            {
                gun.AimOut(); // 强制退出瞄准状态，重置FOV等
            }
        }

        // 修复边界条件：仅在 weaponID < 0 或 weaponID >= weapons.Count 时调整
        if (weaponID < 0)
        {
            weaponID = weapons.Count - 1;
        }
        else if (weaponID >= weapons.Count)
        {
            weaponID = 0;
        }

        currentWeaponID = weaponID;

        // 激活目标武器，同时其他武器保持禁用
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(i == weaponID);
        }

        Debug.Log($"当前武器ID: {currentWeaponID}");
    }

    public void AddWeapon(int itemID, GameObject weapon)
    {
        if (weapons.Contains(weapon))
        {
            print("已存在此枪械");
            return;
        }
        else
        {
            weapons.Add(weapon);
            ChargeWeapon(itemID);  //添加武器
        }
    }

    public void ThrowWeapon(GameObject weapon)
    {
        if (!weapons.Contains(weapon) || weapons.Count == 0)
        {
            print("当前无武器，无法丢弃");
        }
        else
        {
            weapons.Remove(weapon);
            ChargeWeapon(currentWeaponID - 1);  //添加武器
        }
    }
}
