using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private bool[] playerChoice = new bool[2];
    bool isPlayerStart;
    bool isPlayerEnd;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectCollider();
        if(isPlayerStart) {
            StartCoroutine(Fade());
        }
    }

    private void DetectCollider()
    {
        int i = 0;
        foreach (bool hitBoxes in playerChoice) playerChoice[i] = gameObject.transform.GetChild(i).GetComponent<HitBoxDetection>().isTriggered;
        isPlayerEnd = playerChoice[1];
        isPlayerStart = playerChoice[0];
    }

    IEnumerator Fade()
    {
        yield return null;
    }
}
