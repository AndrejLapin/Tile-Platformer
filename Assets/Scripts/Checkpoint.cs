using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    Animator myAnimator;
    Player player;
    const string ACTIVE_BOOL = "Active";

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        player.DeactivateChekpoint();
        myAnimator.SetBool(ACTIVE_BOOL, true);
        player.SetChekpoint(this);
    }

    public void DeactivateThisChekpoint()
    {
        myAnimator.SetBool(ACTIVE_BOOL, false);
    }
}
