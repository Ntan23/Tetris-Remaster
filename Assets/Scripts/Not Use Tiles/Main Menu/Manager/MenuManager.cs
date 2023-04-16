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
    bool runOnceStart;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerMainMenu player;
    void Start()
    {
        runOnceStart = false;
        LeanTween.value(blackScreen, UpdateAlpha, 1f, 0f, 1f);
        if(SceneManager.GetActiveScene().buildIndex == 0 )
        {
            playerAnimator.Play("TeleportIncoming");
            StartCoroutine(DelayStart());
        }
    }

    void Update()
    {
        DetectCollider();

        if(playerChoice[0] && !runOnceStart) 
        {
            LeanTween.value(blackScreen, UpdateAlpha, 0f, 1f, 1.5f);
            StartCoroutine(FirstChoice());
            runOnceStart = true;
        }
        if(playerChoice[1] && !runOnceStart)
        {
            Debug.Log("Choosen");
            LeanTween.value(blackScreen, UpdateAlpha, 0f, 1f, 1.5f);
            StartCoroutine(SecondChoice());
            runOnceStart = true;
        }
    }

    private void DetectCollider()
    {
        for(int i = 0; i < 2; i++) playerChoice[i] = gameObject.transform.GetChild(i).GetComponent<HitBoxDetection>().isTriggered;
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }

    public void ButtonSetting()
    {
        if(settingMenu.activeInHierarchy)
        {
            player.canMove = true;
            LeanTween.value(settingMenu, UpdateScale, 1f, 0f, 0.2f);
            StartCoroutine(Delay());
        }
        else
        {
            player.canMove = false;
            settingMenu.SetActive(true);
            LeanTween.value(settingMenu, UpdateScale, 0f, 1f, 0.2f);
        }
    }

    private IEnumerator DelayStart()
    {
        player.canMove = false;
        yield return new WaitForSeconds(0.48f);
        player.canMove = true;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.2f);
        settingMenu.SetActive(false);
    }
}
