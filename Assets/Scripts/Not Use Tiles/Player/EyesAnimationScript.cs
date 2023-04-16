using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesAnimationScript : MonoBehaviour
{
    private GameManager gm;
    private Animator animator;
    private bool isDone;
    private float timeDelay;

    private void Start()
    {
        gm = GameManager.Instance;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        timeDelay = Random.Range(5, 10);

        if(!isDone)
        {
            isDone = true;
            StartCoroutine(delay(timeDelay));
        }
    }

    IEnumerator delay(float t)
    {
        yield return new WaitForSeconds(t);
        animator.Play("EyeBlinkAnimation");
        isDone = false;
    }
}
