using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoSpawnManager : MonoBehaviour
{
    #region Singleton
    public static TetrominoSpawnManager Instance;

    void Awake()
    {
        if(Instance == null) Instance = this;
    }
    #endregion

    #region IntegerVariables
    private int index;
    private int currentIndex;
    #endregion

    #region OtherVariables
    [SerializeField] private GameObject[] tetrominoes;
    [SerializeField] private GameObject[] shadowTetrominoes;
    [SerializeField] private Transform tetrominoesParent;
    [SerializeField] private NextPieceUI nextPieceUI;
    [SerializeField] private HoldPieceUI holdPieceUI;
    private GameObject objectToSpawn;
    private GameObject shadow;
    private GameManager gm;
    private GhostPiece ghostPiece;
    #endregion

    void Start() 
    {
        gm = GameManager.Instance;

        index = Random.Range(0, tetrominoes.Length);
        nextPieceUI.UpdateSprite(index);
        StartCoroutine(FirstSpawn());
    }
    
    public void SpawnNewTetromino() 
    {
        objectToSpawn = Instantiate(tetrominoes[index], transform.position, Quaternion.identity);
        objectToSpawn.transform.SetParent(tetrominoesParent);

        shadow = Instantiate(shadowTetrominoes[index], transform.position, Quaternion.identity);
        shadow.transform.SetParent(tetrominoesParent);

        ghostPiece = shadow.GetComponent<GhostPiece>();
        ghostPiece.currentPiece = objectToSpawn;
        
        currentIndex = index;

        index = Random.Range(0, tetrominoes.Length);
        nextPieceUI.UpdateSprite(index);
    }

    private void SpawnHoldTetromino()
    {
        objectToSpawn = Instantiate(tetrominoes[gm.GetSavedPieceIndex()], transform.position, Quaternion.identity);
        objectToSpawn.transform.SetParent(tetrominoesParent);

        shadow = Instantiate(shadowTetrominoes[gm.GetSavedPieceIndex()], transform.position, Quaternion.identity);
        shadow.transform.SetParent(tetrominoesParent);

        ghostPiece = shadow.GetComponent<GhostPiece>();
        ghostPiece.currentPiece = objectToSpawn;
    }

    IEnumerator FirstSpawn()
    {
        yield return new WaitForSeconds(2.5f);
        SpawnNewTetromino();
    }

    public void SetHoldPieceIndexAndSpawnNewOne()
    {
        gm.SetSavedPieceIndex(currentIndex);
        ClearAllFallingBlocks();
        ghostPiece.DestroyGhostPiece();
        SpawnNewTetromino();
        holdPieceUI.UpdateSprite(gm.GetSavedPieceIndex());
    }

    public void SwapTetromino()
    {
        ClearAllFallingBlocks();
        ghostPiece.DestroyGhostPiece();
        SpawnHoldTetromino();
        gm.SetSavedPieceIndex(currentIndex);
        holdPieceUI.UpdateSprite(currentIndex);
    }

    private void ClearAllFallingBlocks()
    {
        GameObject[] fallingBlocks = GameObject.FindGameObjectsWithTag("FallingBlock");

        for(int i = 0; i < fallingBlocks.Length; i++) Destroy(fallingBlocks[i]);
    }
}
