using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.SocialPlatforms;

public class AttackPoint : MonoBehaviour
{
    public int mindamage;
    public int maxdamage;

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.GetComponent<PlayerController>().PlayerHealth(Random.Range(mindamage, maxdamage));
        }
    }
}
