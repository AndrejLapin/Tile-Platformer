using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{

    [SerializeField] float contactDamage = 1f;

    public float GetContactDamage()
    {
        return contactDamage;
    }


}
