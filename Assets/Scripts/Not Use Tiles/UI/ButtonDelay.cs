using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDelay : MonoBehaviour
{
    private Button button;

    void Awake() => button = GetComponent<Button>();

    void Start() => StartCoroutine(Delay());

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1.0f);
        button.interactable = true;
    }
}
