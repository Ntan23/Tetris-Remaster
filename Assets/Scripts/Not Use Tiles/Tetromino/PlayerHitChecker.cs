using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitChecker : MonoBehaviour
{
    private RaycastHit2D hit;
    private GameManager gm;

    void Start() => gm = GameManager.Instance;

    void Update()
    {
        if(this.tag == "FallingBlock")
        {
            hit = Physics2D.Raycast(transform.position, Vector2.down);

            if(hit.collider != null)
            {
                if(hit.distance <= 0.0f && hit.collider.CompareTag("Player"))
                {
                    if(gm.IsPlaying())
                    {
                        Debug.Log("Hit");
                        gm.GameOver();
                    }
                }
            } 
        }
    }
}
