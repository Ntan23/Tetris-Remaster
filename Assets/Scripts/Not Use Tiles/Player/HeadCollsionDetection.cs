using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCollsionDetection : MonoBehaviour
{
    private GameManager gm;
    private TetrisBlock tetrisBlock;
    [SerializeField] private bool isTriggered;
    void Start() => gm = GameManager.Instance;

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        tetrisBlock = FindObjectOfType<TetrisBlock>();
        if(tetrisBlock != null)
        {
            if (collisionInfo.gameObject.CompareTag("FallingBlock") && tetrisBlock.GetIsHardDropping()) gm.GameOverImminant();
        }
        else if (collisionInfo.gameObject.CompareTag("FallingBlock")) gm.GameOver();
    }
}
