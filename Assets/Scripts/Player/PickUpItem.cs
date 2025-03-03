using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [Tooltip("武器旋转速度")] private float ratateSpeed;
    [Tooltip("武器编号")] public int itemID;

    private GameObject weaponmode;
    private Transform inventoryTransform;

    // Start is called before the first frame update
    void Start()
    {
        ratateSpeed = 100f;
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
            inventoryTransform = inventory.transform;
        else
            Debug.LogError("Inventory没找到.");

    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += new Vector3(0, ratateSpeed * Time.deltaTime, 0);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null && inventoryTransform != null)
            {
                weaponmode = inventoryTransform.GetChild(itemID).gameObject;
                if (weaponmode != null)
                {
                    playerController.PickUpWeapon(itemID, weaponmode);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogError($"Weapon mode at index {itemID} is null.");
                }
            }
            else
            {
                Debug.LogError("PlayerController or Inventory transform is null.");
            }
        }
    }
}

