using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private int emptyBlockCount;
    private int levelIndex;
    private int score;
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
    #endregion

    #region OtherVariables
    [SerializeField] private Transform tetrominoesParent;
    [SerializeField] private Transform playerTransform;
    private TetrominoSpawnManager tetrominoSpawner;
    public static Transform[,] coordinate = new Transform[boardWidth, boardHeight];
    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject blackScreen;
    private GhostPiece ghostPiece;
    private AudioManager audioManager;
    #endregion

    void Start()
    {
        ghostPiece = GhostPiece.Instance;
        audioManager = AudioManager.Instance;

        LeanTween.value(blackScreen, UpdateAlpha, 1f, 0f, 2.5f);
        gameState = State.IsPlaying;
        tetrominoSpawner = TetrominoSpawnManager.Instance;

        levelIndex = 1;
        targetTimerDelay = 3.0f;
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
    }

    private void UpdateAlpha(float alpha) => blackScreen.GetComponent<CanvasGroup>().alpha = alpha;

    public void GameOver(bool isHardDropDead)
    {
        ghostPiece.DestroyGhostPiece();
        gameState = State.GameOver;
        playerTransform.rotation = Quaternion.Euler(0, 0, 0);

        if(isHardDropDead)
        {
            playerAnimator.Play("Teleport");
            playerTransform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
            playerTransform.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            StartCoroutine(ImminantDelay());
        }
        else if(!isHardDropDead) StartCoroutine(DeathCoolDown(deathTime));
    } 

    public void LevelUp()
    {
        levelIndex++;
        DeleteAllBlocks();
        if(blockFallDelay > 0.2f) blockFallDelay -= 0.1f;
        if(targetTimerDelay > 0.5f) targetTimerDelay -= 0.25f;
        if(tetrominoesParent.childCount == 0) StartCoroutine(WaitForNextSpawn());
    }

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
            GameOver(false);
        }
        else if(emptyBlockCount != 1) emptyBlockCount = 0;
    }

    public void CheckForLineComplete()
    {
        for(int i = boardHeight - 1; i >= 0; i--)
        {
            if(HasLine(i)) 
            {
                lineCount++;
                DeleteLine(i);
                MoveRowDown(i);
                CheckPlayerInLine();
            }
        }

        AddLineCompleteScore();
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
            if(coordinate[i, verticalCoordinate] != null)
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
        if(levelIndex <= 3)
        {
            if(levelIndex == 1)
            {
                if(lineCount == 1) AddScore(40);
                if(lineCount == 2) AddScore(100);
                if(lineCount == 3) AddScore(300);
                if(lineCount >= 4) AddScore(1200);
            }
            else if(levelIndex == 2)
            {
                if(lineCount == 1) AddScore(80);
                if(lineCount == 2) AddScore(200);
                if(lineCount == 3) AddScore(600);
                if(lineCount >= 4) AddScore(2400);
            }
            else if(levelIndex == 3)
            {
                if(lineCount == 1) AddScore(120);
                if(lineCount == 2) AddScore(300);
                if(lineCount == 3) AddScore(900);
                if(lineCount >= 4) AddScore(3600);
            }
        }
        else if(levelIndex > 3)
        {
            if(lineCount == 1) AddScore(40 * (levelIndex + 1));
            if(lineCount == 2) AddScore(100 * (levelIndex + 1));
            if(lineCount == 3) AddScore(300 * (levelIndex + 1));
            if(lineCount >= 4) AddScore(1200 * (levelIndex + 1));
        }
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

        scoreUI.UpdateScoreText();
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

    private IEnumerator DeathCoolDown(float coolDown)
    {
        playerAnimator.Play("DeathBeep");
        playerTransform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);

        audioManager.PlayBeepingSFX();
        
        while (coolDown >= 0)
        {
            coolDown -= Time.deltaTime;
            yield return null;
        }
        LeanTween.value(blackScreen, UpdateAlpha, 0f, 1f, 2.5f);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }

    private IEnumerator ImminantDelay()
    {
        LeanTween.value(blackScreen, UpdateAlpha, 0f, 1f, 2.5f);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }
}
