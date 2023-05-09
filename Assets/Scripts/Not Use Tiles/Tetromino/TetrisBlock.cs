using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    #region FloatVariables
    private float fallTimer;
    private float fallTimeDelay;
    private float originalFallTimeDelay;
    private float targetTimer;
    private float targetTimeDelay;
    #endregion

    #region IntegerVariables
    private int roundedX;
    private int playerRoundedX;
    private int modeIndex;
    #endregion

    #region BoolVariables
    private bool isHardDropping;
    private bool isMovingLeft;
    private bool isChecked;
    #endregion

    #region VectorVariables
    [SerializeField] private Vector3 rotationPoint;
    #endregion

    #region OtherVariables
    private Transform playerTransform;
    private TetrominoSpawnManager tetrominoSpawner;
    private GameManager gm;
    private GhostPiece ghostPiece;
    private ParticleSystem dustEffect;
    private AudioManager audioManager;
    private CameraShake cameraShake;
    #endregion

    void Start() 
    {
        dustEffect = GameObject.FindGameObjectWithTag("HardDropParticle").GetComponent<ParticleSystem>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        tetrominoSpawner = TetrominoSpawnManager.Instance;
        gm = GameManager.Instance;
        ghostPiece = GhostPiece.Instance;
        audioManager = AudioManager.Instance;

        fallTimeDelay = gm.GetBlockFallDelay();
        targetTimeDelay = gm.GetTargetTimerDelay();
        cameraShake = FindObjectOfType<CameraShake>();

        originalFallTimeDelay = fallTimeDelay;
        isHardDropping = false;
    }

    void Update()
    {
        if(!gm.IsPlaying()) return;
        
        fallTimer += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.A)) 
        {
            isMovingLeft = true;

            if(CanMoveLeftOrRight())
            {
                transform.position += Vector3.left;

                if(IsValidMove()) audioManager.PlayBlockMoveSFX();

                if(!IsValidMove()) transform.position -= Vector3.left;
            }
        }
        else if(Input.GetKeyDown(KeyCode.D)) 
        {
            isMovingLeft = false;

            if(CanMoveLeftOrRight())
            {
                transform.position += Vector3.right;

                if(IsValidMove()) audioManager.PlayBlockMoveSFX();

                if(!IsValidMove()) transform.position -= Vector3.right;
            }
        }
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, -90);

            if(IsValidMove() )audioManager.PlayBlockRotateSFX();

            if(!IsValidMove()) transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, 90);
        } 
        else if(Input.GetKeyDown(KeyCode.E))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, 90);
                
            if(IsValidMove()) audioManager.PlayBlockRotateSFX();

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

    private void Fall()
    {
        if(IsValidMove()) 
        {
            if(ThereIsPlayer()) return;
            else if(!ThereIsPlayer()) transform.position += Vector3.down;
        }

        if(!IsValidMove()) 
        {
            transform.position += Vector3.up;

            if(!ThereIsPlayer()) AddToGrid();
            
            ghostPiece.DestroyGhostPiece();
            gm.CheckForLineComplete();  
            gm.CheckPlayerInLine();

            if(!isHardDropping) gm.AddScore(1);
            else if(isHardDropping) 
            {
                dustEffect.Play();
                cameraShake.ShakeCamera(2, 1);
                audioManager.PlayHardDropSFX();
                gm.AddScore(5);
            }

            gm.SetBackAlreadySwap();

            if(!gm.BlockAtTheTop()) tetrominoSpawner.SpawnNewTetromino();
            else if(gm.BlockAtTheTop()) gm.GameOver();

            this.enabled = false;
        }
    }

    private bool IsValidMove()
    {
        Vector2 position;

        foreach(Transform children in transform)
        {
            position = gm.RoundPosition(children.transform.position);

            if(!gm.IsInsidePlayfield(position)) return false;

            if(gm.GetBlockAtPosition(position) != null) return false;
        }

        return true;
    }

    private bool ThereIsPlayer()
    {
        Vector2 position;
        Vector2 playerPosition;

        playerPosition = gm.RoundPosition(playerTransform.position);

        foreach(Transform children in transform)
        {
            position = gm.RoundPosition(children.transform.position);

            if(position.y - playerPosition.y == 1 && position.x == playerPosition.x) return true;
        }

        return false;
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
        Vector2 position;

        foreach(Transform children in transform)
        {
            position = gm.RoundPosition(children.transform.position);
         
            children.gameObject.tag = "Block";

            GameManager.coordinate[(int)position.x, (int)position.y] = children;
        }

        dustEffect.transform.position = transform.position;
    }

    // private void TargetPlayer()
    // {   
    //     if(gm.GetPlayerPosition().x < transform.position.x) 
    //     {
    //         transform.position += Vector3.left;

    //         if(!IsValidMove()) transform.position -= Vector3.left;
    //     }
    //     else if(gm.GetPlayerPosition().x > transform.position.x)
    //     {
    //         transform.position += Vector3.right;

    //         if(!IsValidMove()) transform.position -= Vector3.right;
    //     } 
    // }

    public bool GetIsHardDropping()
    {
        return isHardDropping;
    }
}
