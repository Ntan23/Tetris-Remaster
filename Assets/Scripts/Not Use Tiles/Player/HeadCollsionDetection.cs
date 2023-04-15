using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCollsionDetection : MonoBehaviour
{
    private GameManager gm;
    [SerializeField] private bool isTriggered;
    void Start() => gm = GameManager.Instance;

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("FallingBlock")) gm.GameOver();
    }
}
