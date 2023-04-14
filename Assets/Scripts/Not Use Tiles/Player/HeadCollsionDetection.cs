using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCollsionDetection : MonoBehaviour
{
    private GameManager gm;

    void Start() => gm = GameManager.Instance;

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(collisionInfo.gameObject.CompareTag("FallingBlock") && !GetComponentInParent<PlayerMovement>().IsMoving()) gm.GameOver();
    }
}
