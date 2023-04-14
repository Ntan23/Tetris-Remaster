using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    #region EnumVariables
    private enum Mode{
        SingleControl, DoubleControl
    }

    [SerializeField] private Mode mode;
    #endregion

    #region FloatVariables
    private float fallTimer;
    private float fallTimeDelay;
    private float originalFallTimeDelay;
    private float targetTimer;
    private float targetTimeDelay;
    #endregion

    #region IntegerVariables
    private int boardWidth;
    private int boardHeight;
    private int roundedX;
    private int roundedY;
    private int playerRoundedX;
    #endregion

    #region BoolVariables
    private bool isHardDropping;
    private bool isMovingLeft;
    private bool isLock;
    #endregion

    #region VectorVariables
    [SerializeField] private Vector3 rotationPoint;
    #endregion

    #region OtherVariables
    private TetrominoSpawnManager tetrominoSpawner;
    private GameManager gm;
    #endregion

    void Start() 
    {
        tetrominoSpawner = TetrominoSpawnManager.Instance;
        gm = GameManager.Instance;

        fallTimeDelay = gm.GetBlockFallDelay();
        boardWidth = gm.GetBoardWidth();
        boardHeight = gm.GetBoardHeight();
        targetTimeDelay = gm.GetTargetTimerDelay();

        originalFallTimeDelay = fallTimeDelay;
        isHardDropping = false;
    }

    void Update()
    {
        if(!gm.IsPlaying()) return;

        fallTimer += Time.deltaTime;

        if(mode == Mode.DoubleControl)
        {
            if(Input.GetKeyDown(KeyCode.A)) 
            {
                isMovingLeft = true;

                if(CanMoveLeftOrRight())
                {
                    transform.position += Vector3.left;

                    if(!IsValidMove()) transform.position -= Vector3.left;
                }
            }
            else if(Input.GetKeyDown(KeyCode.D)) 
            {
                isMovingLeft = false;

                if(CanMoveLeftOrRight())
                {
                    transform.position += Vector3.right;

                    if(!IsValidMove()) transform.position -= Vector3.right;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Q))
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, -90);

                if(!IsValidMove()) transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, 90);
            } 
            else if(Input.GetKeyDown(KeyCode.E))
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, 90);

                if(!IsValidMove()) transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, -90);
            }
            else if(Input.GetKeyDown(KeyCode.Space)) 
            {
                fallTimeDelay /= 1000;
                isHardDropping = true;
            }

            if(fallTimer > (Input.GetKey(KeyCode.S) ? fallTimeDelay / 10 : fallTimeDelay))
            {
                Fall();

                fallTimer = 0.0f;
            }
        }

        if(mode == Mode.SingleControl)
        {
            targetTimer += Time.deltaTime;

            if(targetTimer > targetTimeDelay && !isLock)
            {
                TargetPlayer();
                targetTimer = 0;
            }

            if(fallTimer > fallTimeDelay)
            {
                Fall();

                fallTimer = 0.0f;
            }
        }
    }

    private void Fall()
    {
        transform.position += Vector3.down;

        if(!IsValidMove()) 
        {
            transform.position += Vector3.up;
            AddToGrid();
            gm.CheckForLineComplete();  

            isLock = true;

            if(!isHardDropping) gm.AddScore(1);
            else if(isHardDropping) gm.AddScore(5);

            if(!gm.BlockAtTheTop()) tetrominoSpawner.SpawnNewTetromino();
            else if(gm.BlockAtTheTop()) gm.GameOver();

            gm.SetBackAlreadySwap();
            
            this.enabled = false;
        }
    }

    private bool IsValidMove()
    {
        foreach(Transform children in transform)
        {
            roundedX = Mathf.RoundToInt(children.transform.position.x);
            roundedY = Mathf.RoundToInt(children.transform.position.y);

            if(roundedX < 0 || roundedX >= boardWidth || roundedY < 0 || roundedY >= boardHeight) return false;

            if(GameManager.coordinate[roundedX, roundedY] != null) return false;
        }

        return true;
    }

    private bool CanMoveLeftOrRight()
    {
        playerRoundedX = Mathf.RoundToInt(gm.GetPlayerPosition().x);

        foreach(Transform children in transform)
        {
            roundedX = Mathf.RoundToInt(children.transform.position.x);

            if(isMovingLeft) if(roundedX - 1 == playerRoundedX && children.transform.position.y - gm.GetPlayerPosition().y <= 1.0f) return false;
            else if(!isMovingLeft) if(roundedX + 1 == playerRoundedX && children.transform.position.y - gm.GetPlayerPosition().y <= 1.0f) return false;
        }

        return true;
    }

    private void AddToGrid()
    {
        foreach(Transform children in transform)
        {
            roundedX = Mathf.RoundToInt(children.transform.position.x);
            roundedY = Mathf.RoundToInt(children.transform.position.y);
            children.gameObject.tag = "Block";
            GameManager.coordinate[roundedX, roundedY] = children;
        }
    }

    private void TargetPlayer()
    {   
        if(gm.GetPlayerPosition().x < transform.position.x) 
        {
            transform.position += Vector3.left;

            if(!IsValidMove()) transform.position -= Vector3.left;
        }
        else if(gm.GetPlayerPosition().x > transform.position.x)
        {
            transform.position += Vector3.right;

            if(!IsValidMove()) transform.position -= Vector3.right;
        } 
    }
}
