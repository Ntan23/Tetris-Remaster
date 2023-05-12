using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    void Awake() 
    {
        if(Instance == null) Instance = this;
    }
    #endregion
    
    #region EnumVariable
    private enum State {
        IsPlaying, GameOver, GamePaused
    };

    private State gameState;
    #endregion

    #region IntegerVariable
    private static int boardWidth = 10;
    private static int boardHeight = 23;
    private int lineCount;
    private int lineCleared;
    private int bestLineCleared;
    private int emptyBlockCount;
    private int levelIndex = 1;
    private int score;
    private int maxScore;
    private int highscore;
    private int savedPieceIndex;
    #endregion
    
    #region FloatVariables
    [SerializeField] private float blockFallDelay;
    [SerializeField] private float targetTimerDelay;
    [SerializeField] private float deathTime;
    private float startTimer;
    #endregion

    #region BoolVariables
    private bool isHolding;
    private bool isSwapped;
    private bool isSoundPlayed;
    private bool pause;
    private bool canLevelUp;
    private bool isFirstTimePause = true;
    #endregion

    #region OtherVariables
    [SerializeField] private GameObject particles;
    private ParticleSystem dustEffect;
    [SerializeField] private Transform tetrominoesParent;
    [SerializeField] private Transform playerTransform;
    private TetrominoSpawnManager tetrominoSpawner;
    public static Transform[,] coordinate = new Transform[boardWidth, boardHeight + 2];
    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private LineClearedUI lineClearedUI;
    [SerializeField] private LevelUI levelUI;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject blackScreen;
    private GhostPiece ghostPiece;
    private AudioManager audioManager;
    [SerializeField] private GameObject settingMenu;
    #endregion

    void Start()
    {
        dustEffect = particles.GetComponent<ParticleSystem>();
        highscore = PlayerPrefs.GetInt("Highscore",0);
        bestLineCleared = PlayerPrefs.GetInt("BestLineCleared",0);

        ghostPiece = GhostPiece.Instance;
        audioManager = AudioManager.Instance;

        LeanTween.value(blackScreen, UpdateAlpha, 1f, 0f, 2.5f);
        gameState = State.IsPlaying;
        tetrominoSpawner = TetrominoSpawnManager.Instance;

        targetTimerDelay = 3.0f;
        maxScore = 100;

        canLevelUp = true;

        StartCoroutine(FirstTimePauseDelay());
    }

    void Update()
    {
        startTimer += Time.deltaTime;

        if(startTimer > 3.0f)
        {
            if(Input.GetKeyDown(KeyCode.C) && !isSwapped)
            {
                audioManager.PlayBlockHoldSFX();

                if(!isHolding)
                {
                    tetrominoSpawner.SetHoldPieceIndexAndSpawnNewOne();
                    isHolding = true;
                }
                else if(isHolding) tetrominoSpawner.SwapTetromino();

                isSwapped = true;
            }  
        } 

        if(Input.GetKeyDown(KeyCode.Escape) && !pause && !isFirstTimePause) 
        {
            pause = true;
            audioManager.PlayButtonClickSFX();
            PlayPauseGame();
        }
    }

    public void PlayPauseGame()
    {
        if(settingMenu.activeInHierarchy)
        {
            Time.timeScale = 1f;
            LeanTween.value(settingMenu, UpdateScale, 1f, 0f, 0.2f);
            StartCoroutine(Delay("deActivating"));
        }
        else
        {
            settingMenu.SetActive(true);
            settingsButton.interactable = false;
            LeanTween.value(settingMenu, UpdateScale, 0f, 1f, 0.2f);
            StartCoroutine(Delay("activating"));
        }
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        LeanTween.value(settingMenu, UpdateScale, 1f, 0f, 0.2f);
        playerTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        playerAnimator.Play("Teleport");
        StartCoroutine(ImminantDelay("Menu"));
    }

    private void UpdateAlpha(float alpha) => blackScreen.GetComponent<CanvasGroup>().alpha = alpha;

    void UpdateScale(float scale)
    {
        settingMenu.GetComponent<CanvasGroup>().alpha = scale;
        settingMenu.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, settingMenu.GetComponent<RectTransform>().localScale.z);
    }

    public void GameOver()
    {   
        Debug.Log("Game Over");
        ghostPiece.DestroyGhostPiece();
        CheckScore();
        CheckLineCleared();
        gameState = State.GameOver;
        StartCoroutine(RotateBackPlayer());
    } 

    public void LevelUp(bool atTop)
    {   
        Debug.Log("Level Up");
        if(atTop) DeleteAllBlocks();

        levelIndex++;
        levelUI.UpdateLevelText();
        /*if (levelIndex % 2 == 0)
        {
            Debug.Log("changeTinttoRed");
        }
        else
        {
            LeanTween.value(globalVolumeGameObject, Color.red, Color.blue, 1f);
        }*/
        audioManager.PlayLevelUpSFX();
        if(blockFallDelay >= 0.1f) blockFallDelay -= 0.1f;
        if(targetTimerDelay > 0.5f) targetTimerDelay -= 0.25f;
        if(tetrominoesParent.childCount == 0) StartCoroutine(WaitForNextSpawn());

        canLevelUp = false;
    }

    public bool ChangeBackCanLevelUp() => canLevelUp = true;

    public bool IsPlaying()
    {
        return gameState == State.IsPlaying;
    }

    public float GetBlockFallDelay()
    {
        return blockFallDelay;
    }

    public void CheckPlayerInLine()
    {
        Vector2 position = RoundPosition(playerTransform.position);

        for(int i = 0; i < boardWidth; i++)
        {
            if(coordinate[i, (int)position.y] == null) emptyBlockCount++;
        }

        if(emptyBlockCount == 1) 
        {
            DeleteLine((int) position.y);
            GameOver();
        }
        else if(emptyBlockCount != 1) emptyBlockCount = 0;
    }

    public void CheckForLineComplete()
    {
        for(int i = boardHeight - 1; i >= 0; i--)
        {
            if(HasLine(i)) 
            {
                lineCleared++;
                lineCount++;
                DeleteLine(i);
                MoveRowDown(i);
                CheckPlayerInLine();
            }
        }

        AddLineCompleteScore();
        lineClearedUI.UpdateLineClearedText();
        lineCount = 0;
    }

    private bool HasLine(int verticalCoordinate)
    {
        for(int i = 0; i < boardWidth; i++) if(coordinate[i, verticalCoordinate] == null) return false;
        
        return true;
    }

    private void DeleteLine(int verticalCoordinate)
    {
        for(int i = 0; i < boardWidth; i++)
        {
            if (coordinate[i, verticalCoordinate] != null)
            {
                Destroy(coordinate[i, verticalCoordinate].gameObject);
                coordinate[i, verticalCoordinate] = null;
            }
        }
    }

    private void MoveRowDown(int verticalCoordinate)
    {
        for(int i = verticalCoordinate; i  < boardHeight; i++)
        {
            for(int j = 0; j < boardWidth; j++)
            {
                if(coordinate[j, i] != null)
                {
                    coordinate[j, i - 1] = coordinate[j, i];
                    coordinate[j, i] = null;
                    coordinate[j, i - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }   

    private void AddLineCompleteScore()
    {
        if(lineCount == 1) AddScore(40 * levelIndex);
        if(lineCount == 2) AddScore(100 * levelIndex);
        if(lineCount == 3) AddScore(300 * levelIndex);
        if(lineCount == 4) AddScore(1200 * levelIndex);
    }

    public void DeleteAllBlocks()
    {
        for(int i = 0; i < boardWidth; i++)
        {
            for(int j = 0; j < boardHeight; j++)
            {
                if(coordinate[i, j] != null)
                {
                    Destroy(coordinate[i, j].gameObject);
                    coordinate[i, j] = null;
                }
            }
        }
    }

    public void ClearAllFallingBlocks()
    {
        GameObject[] fallingBlocks = GameObject.FindGameObjectsWithTag("FallingBlock");

        for(int i = 0; i < fallingBlocks.Length; i++) Destroy(fallingBlocks[i]);
    }

    public bool BlockAtTheTop()
    {
        for(int i = 0; i < boardWidth; i++)
        {
            if(coordinate[i, 22] != null) return true;
        }

        return false;
    }

    private void CheckScore()
    {
        if(score > highscore) PlayerPrefs.SetInt("Highscore", score);
    }

    private void CheckLineCleared()
    {
        if(lineCleared > bestLineCleared) PlayerPrefs.SetInt("BestLineCleared", lineCleared);
    }
    
    public Transform GetBlockAtPosition(Vector3 position)
    {
        return coordinate[(int)position.x, (int)position.y];
    }

    public bool IsInsidePlayfield(Vector3 position)
    {
        return (int)position.x >= 0 && (int)position.x < boardWidth && (int)position.y >= 0 && (int)position.y < boardHeight;
    }

    public Vector2 RoundPosition(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;

        CheckScoreToLevelUp();
        scoreUI.UpdateScoreText();
    }

    private void CheckScoreToLevelUp()
    {
        if(score >= maxScore) 
        {
            LevelUp(false);
            maxScore *= 2;

            CheckScoreToLevelUp();
        }
    }

    IEnumerator WaitForNextSpawn()
    {
        yield return new WaitForSeconds(2.0f);
        tetrominoSpawner.SpawnNewTetromino();
    }

    public void SetSavedPieceIndex(int index) => savedPieceIndex = index;

    public void SetBackAlreadySwap() => isSwapped = false;

    public int GetScore()
    {
        return score;
    }

    public int GetLineCleared()
    {
        return lineCleared;
    }

    public Vector3 GetPlayerPosition()
    {
        return new Vector3(Mathf.Abs(playerTransform.position.x), Mathf.Abs(playerTransform.position.y));
    }

    public float GetTargetTimerDelay()
    {
        return targetTimerDelay;
    }

    public int GetSavedPieceIndex()
    {
        return savedPieceIndex;
    }

    public int GetLevelIndex()
    {
        return levelIndex;
    }

    public bool GetCanLevelUp()
    {
        return canLevelUp;
    }

    private IEnumerator Delay(string condition)
    {
        yield return new WaitForSeconds(0.25f);

        if(condition == "deActivating") 
        {
            settingMenu.SetActive(false);
            settingsButton.interactable = true;
        }
        else if(condition == "activating") Time.timeScale = 0f;

        pause = false;
    }

    private IEnumerator ImminantDelay(string type)
    {
        LeanTween.value(blackScreen, UpdateAlpha, 0f, 1f, 1.5f);
        yield return new WaitForSeconds(2f);

        if(type == "Menu") SceneManager.LoadScene(0);
        else if(type == "Selection") SceneManager.LoadScene(3);
    }

    private IEnumerator RotateBackPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        playerTransform.rotation = Quaternion.Euler(0, 0, 0);
        playerAnimator.Play("Teleport");
        playerTransform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(ImminantDelay("Selection"));
    }

    IEnumerator FirstTimePauseDelay()
    {
        yield return new WaitForSeconds(2.5f);
        isFirstTimePause = false;
    }
}
