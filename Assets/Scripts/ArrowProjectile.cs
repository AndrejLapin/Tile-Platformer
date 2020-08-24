using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{

    Rigidbody2D myRigidBody;
    BoxCollider2D myCollider;

    float damage;

    private void Awake()
    {
        damage = FindObjectOfType<Player>().GetBaseDamage();
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RotateProjectile();
    }

    private void RotateProjectile()
    {
        transform.localRotation = Quaternion.Euler(0, 0, Vector2.Angle(new Vector2(myRigidBody.velocity.x, myRigidBody.velocity.y), new Vector2(1, 0)) * Mathf.Sign(myRigidBody.velocity.y));
    }

    private void OnCollisionEnter2D(Collision2D other) // apperantly I can change this to Collider2D, should chek how this works later
    {
        /*if (myCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            //Debug.Log("I hit an enemy!");
            
        }
        else
        {
            //Debug.Log("I hit the GROUND");
        }*/
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (myCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            collision.GetComponent<EnemyHealth>().GetDamadged(damage);

        }
        Destroy(gameObject);
    }

}
