using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionDetection : MonoBehaviour
{
    private Transform playerPosition;
    public bool isInside;
    public bool isFallingBlock;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Block") || collision.CompareTag("Ground")) 
        {
            isFallingBlock = false;
            isInside = true;
        }
        else if(collision.CompareTag("FallingBlock")){
            isInside = true; 
            isFallingBlock = true;
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Block") || collision.CompareTag("Ground") || collision.CompareTag("FallingBlock")) 
        {
            isFallingBlock = false;
            isInside = false;
        } 
    }
}