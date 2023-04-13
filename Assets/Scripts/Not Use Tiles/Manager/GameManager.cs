using UnityEngine;

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
    private int levelIndex;
    private int score;
    #endregion
    
    #region FloatVariables
    [SerializeField] private float blockFallDelay;
    #endregion

    #region OtherVariables
    [SerializeField] private Transform tetrominoesParent;
    private TetrominoSpawner tetrominoSpawner;
    public static Transform[,] coordinate = new Transform[boardWidth, boardHeight];
    [SerializeField] private ScoreUI scoreUI;
    #endregion

    void Start()
    {
        gameState = State.IsPlaying;
        tetrominoSpawner = TetrominoSpawner.Instance;

        levelIndex = 1;
    }

    public void GameOver()
    {
        gameState = State.GameOver;
        Debug.Log("Game Over");
    }   

    public void LevelUp()
    {
        levelIndex++;
        DeleteAllBlocks();
        if(blockFallDelay > 0.2f) blockFallDelay -= 0.1f;
        if(tetrominoesParent.childCount == 0) tetrominoSpawner.SpawnNewTetromino();
    }

    public bool IsPlaying()
    {
        return gameState == State.IsPlaying;
    }

    public float GetBlockFallDelay()
    {
        return blockFallDelay;
    }

    public void CheckForLineComplete()
    {
        for(int i = boardHeight - 1; i >= 0; i--)
        {
            if(HasLine(i)) 
            {
                lineCount++;
                AddLineCompleteScore();
                DeleteLine(i);
                MoveRowDown(i);
            }
        }

        lineCount = 0;
    }

    private bool HasLine(int verticalCoordinate)
    {
        for(int i = 0; i < boardWidth; i++)
        {
            if(coordinate[i, verticalCoordinate] == null) return false;
        }

        return true;
    }

    private void DeleteLine(int verticalCoordinate)
    {
        for(int i = 0; i < boardWidth; i++)
        {
            Destroy(coordinate[i, verticalCoordinate].gameObject);
            coordinate[i, verticalCoordinate] = null;
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

    public bool BlockAtTheTop()
    {
        for(int i = 0; i < boardWidth; i++)
        {
            if(coordinate[i, 22] != null) return true;
        }

        return false;
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;

        scoreUI.UpdateScoreText();
    }

    public int GetBoardWidth()
    {
        return boardWidth;
    }

    public int GetBoardHeight()
    {
        return boardHeight;
    }

    public int GetScore()
    {
        return score;
    }
}
