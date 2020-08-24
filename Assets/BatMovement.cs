using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatMovement : MonoBehaviour
{

    [SerializeField] float moveSpeed = 7;

    CircleCollider2D myAgroRange;
    Vector2 myTarget;
    Rigidbody2D myRigidBody;

    float newXPos;
    float newYPos;

    //state
    bool agressive = false;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAgroRange = GetComponentInChildren<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(myTarget);
        //MoveToTarget();
        MoveToTargetVelocityBased();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (myAgroRange.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            myTarget = new Vector2(collision.GetComponent<Transform>().position.x, collision.GetComponent<Transform>().position.y);
            agressive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (myAgroRange.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            agressive = false;
        }
    }

    private void MoveToTarget()
    {
        if(agressive)
        {
            if(myTarget.x > transform.position.x)
            {
                newXPos = transform.position.x + (moveSpeed * Time.deltaTime) ;
            }
            else if(myTarget.x < transform.position.x)
            {
                newXPos = transform.position.x - (moveSpeed * Time.deltaTime) ;
            }
            if (myTarget.y > transform.position.y)
            {
                newYPos = transform.position.y + (moveSpeed * Time.deltaTime);
            }
            else if (myTarget.y < transform.position.y)
            {
                newYPos = transform.position.y - (moveSpeed * Time.deltaTime);
            }
            transform.position = new Vector2(newXPos, newYPos);
        }
    }

    private void MoveToTargetVelocityBased()
    {
        if (agressive)
        {
            if (myTarget.x > transform.position.x)
            {
                newXPos = moveSpeed;
            }
            else if (myTarget.x < transform.position.x)
            {
                newXPos = -moveSpeed;
            }
            if (myTarget.y > transform.position.y)
            {
                newYPos = moveSpeed;
            }
            else if (myTarget.y < transform.position.y)
            {
                newYPos = -moveSpeed;
            }
            myRigidBody.velocity = new Vector2(newXPos, newYPos);
        }
        else
        {
            myRigidBody.velocity = new Vector2(0, 0);
        }
    }


}
