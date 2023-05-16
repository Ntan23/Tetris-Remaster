using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCollsionDetection : MonoBehaviour
{
    private GameManager gm;
    private PlayerMovement playerMovement;

    void Start() 
    {
        gm = GameManager.Instance;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        if(collisionInfo.gameObject.CompareTag("FallingBlock") || collisionInfo.gameObject.CompareTag("Block"))
        {
            if(gm.IsPlaying() && playerMovement.IsMoving()) 
            {
                Debug.Log("Hit By Body Detection");
                gm.GameOver();
            }
        } 
    }
}
