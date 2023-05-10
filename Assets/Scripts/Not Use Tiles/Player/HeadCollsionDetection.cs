using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCollsionDetection : MonoBehaviour
{
    private GameManager gm;

    void Start() => gm = GameManager.Instance;

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("FallingBlock"))
        {
            GetComponentInParent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            if(!gm.GetIsDead()) gm.GameOver();
        } 
    }
}
