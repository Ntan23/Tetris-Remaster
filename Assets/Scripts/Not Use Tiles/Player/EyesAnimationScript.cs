using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesAnimationScript : MonoBehaviour
{
    private GameManager gm;
    bool isDone = true;

    private void Start()
    {
        gm = GameManager.Instance;
    }

    void Update()
    {
        float timeDelay = Random.Range(7, 10);
        if (isDone)
        {
            isDone = false;
            StartCoroutine(delay(timeDelay));
        }
       
    }

    IEnumerator delay(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.GetComponent<Animator>().Play("EyeBlinkAnimation");
        isDone= true;
    }
}
