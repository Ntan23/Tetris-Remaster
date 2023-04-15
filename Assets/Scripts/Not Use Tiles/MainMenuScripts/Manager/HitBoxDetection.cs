using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxDetection : MonoBehaviour
{
    public bool isTriggered;

    private void OnTriggerEnter2D(Collider2D collDetection)
    {
        if(collDetection.CompareTag("Player")) isTriggered = true;
    }
}
