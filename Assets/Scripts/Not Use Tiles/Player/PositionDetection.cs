using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionDetection : MonoBehaviour
{
    private Transform playerPosition;
    public bool isInside;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Block")) isInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Block")) isInside = false;
    }
}