using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private bool[] playerChoice = new bool[2];
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private GameObject settingMenu;
    bool isPlayerStart;
    bool isPlayerEnd;
    bool runOnceStart;
    void Start()
    {
        runOnceStart = false;
        LeanTween.value(blackScreen, UpdateAlpha, 1f, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        DetectCollider();
        if(isPlayerStart && !runOnceStart) {
            LeanTween.value(blackScreen, UpdateAlpha, 0f, 1f, 2.5f);
            StartCoroutine(FirstChoice());
            runOnceStart = true;
        }
        if (isPlayerEnd && !runOnceStart)
        {
            LeanTween.value(blackScreen, UpdateAlpha, 0f, 1f, 2.5f);
            StartCoroutine(SecondChoice());
            runOnceStart = true;
        }
    }

    private void DetectCollider()
    {
        int i = 0;
        foreach (bool hitBoxes in playerChoice) playerChoice[i] = gameObject.transform.GetChild(i).GetComponent<HitBoxDetection>().isTriggered;
        isPlayerEnd = playerChoice[1];
        isPlayerStart = playerChoice[0];
    }

    void UpdateAlpha(float alpha)
    {
        blackScreen.GetComponent<CanvasGroup>().alpha = alpha;
    }

    void UpdateScale(float scale)
    {
        settingMenu.GetComponent<CanvasGroup>().alpha = scale;
        settingMenu.GetComponent<RectTransform>().localScale = new Vector2(scale, scale);
    }

    IEnumerator FirstChoice(){
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator SecondChoice()
    {
        yield return new WaitForSeconds(2f);
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            Application.Quit();
        }
    }

    public void ButtonSetting()
    {
        if (settingMenu.active)
        {
            LeanTween.value(settingMenu, UpdateScale, 1f, 0f, 0.2f);
            StartCoroutine(Delay());
        }
        else
        {
            settingMenu.SetActive(true);
            LeanTween.value(settingMenu, UpdateScale, 0f, 1f, 0.2f);
        }

    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.2f);
        settingMenu.SetActive(false);
    }
}
