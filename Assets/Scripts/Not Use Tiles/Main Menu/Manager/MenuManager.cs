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
    [SerializeField] private Button settingsButton;
    bool runOnceStart;
    private bool pause;
    private bool isFirstTimePause = true;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerMainMenu player;
    private AudioManager audioManager;

    void Start()
    {
        runOnceStart = false;
        audioManager = AudioManager.Instance;
        LeanTween.value(blackScreen, UpdateAlpha, 1f, 0f, 1f);

        playerAnimator.Play("TeleportIncoming");
        StartCoroutine(DelayStart());

        StartCoroutine(FirstTimePauseDelay());
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
            LeanTween.value(blackScreen, UpdateAlpha, 0f, 1f, 1.5f);
            StartCoroutine(SecondChoice());
            runOnceStart = true;
        }

        if(Input.GetKeyDown(KeyCode.Escape) && !pause && !isFirstTimePause) 
        {
            pause = true;
            audioManager.PlayButtonClickSFX();
            OpenCloseSetting();
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

        if(SceneManager.GetActiveScene().buildIndex == 3) SceneManager.LoadScene(4);
        else if(SceneManager.GetActiveScene().buildIndex == 1) SceneManager.LoadScene(2);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    IEnumerator SecondChoice()
    {
        yield return new WaitForSeconds(2f);

        if(SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 3) SceneManager.LoadScene(1);
        
        if(SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }

    public void OpenCloseSetting()
    {
        if(settingMenu.activeInHierarchy)
        {
            Time.timeScale = 1f;
            LeanTween.value(settingMenu, UpdateScale, 1f, 0f, 0.2f);
            StartCoroutine(Delay("Closing"));
        }
        else
        {
            settingMenu.SetActive(true);
            settingsButton.interactable = false;
            LeanTween.value(settingMenu, UpdateScale, 0f, 1f, 0.2f);
            StartCoroutine(Delay("Opening"));
        }
    }

    private IEnumerator DelayStart()
    {
        player.canMove = false;
        yield return new WaitForSeconds(0.5f);
        player.canMove = true;
    }

    private IEnumerator Delay(string condition)
    {
        yield return new WaitForSeconds(0.25f);

        if(condition == "Closing") 
        {
            settingMenu.SetActive(false);
            settingsButton.interactable = true;
        }
        else if(condition == "Opening") Time.timeScale = 0f;

        pause = false;
    }

    IEnumerator FirstTimePauseDelay()
    {
        yield return new WaitForSeconds(2.0f);
        isFirstTimePause = false;
    }
}
