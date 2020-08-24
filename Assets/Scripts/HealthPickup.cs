using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float amountOfHealth = 1f;

    Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if(player.GetCurrentHealth() < player.GetMaxHealth())
        {
            player.GetMoreHealth(amountOfHealth);
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
}
