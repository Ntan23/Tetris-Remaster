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
    private int indexChecker;
    #endregion

    #region OtherVariables
    [SerializeField] private GameObject[] tetrominoes;
    [SerializeField] private Transform tetrominoesParent;
    [SerializeField] private NextPieceUI nextPieceUI;
    [SerializeField] private HoldPieceUI holdPieceUI;
    private GameObject objectToSpawn;
    private GameManager gm;
    [SerializeField] private GhostPiece ghostPiece;
    #endregion

    void Start() 
    {
        gm = GameManager.Instance;

        index = Random.Range(0, tetrominoes.Length);
        indexChecker = index;
        nextPieceUI.UpdateSprite(index);
        StartCoroutine(FirstSpawn());
    }
    
    public void SpawnNewTetromino() 
    {
        objectToSpawn = Instantiate(tetrominoes[index], transform.position, Quaternion.identity);
        objectToSpawn.transform.SetParent(tetrominoesParent);

        ghostPiece.currentPiece = objectToSpawn;
        ghostPiece.Initialize(index);
        
        currentIndex = index;

        index = Random.Range(0, tetrominoes.Length);

        while(index == indexChecker) index = Random.Range(0, tetrominoes.Length);
        
        if(index != indexChecker) indexChecker = index;

        nextPieceUI.UpdateSprite(index);
    }

    private void SpawnHoldTetromino()
    {
        objectToSpawn = Instantiate(tetrominoes[gm.GetSavedPieceIndex()], transform.position, Quaternion.identity);
        objectToSpawn.transform.SetParent(tetrominoesParent);

        ghostPiece.currentPiece = objectToSpawn;
        ghostPiece.Initialize(gm.GetSavedPieceIndex());
    }

    IEnumerator FirstSpawn()
    {
        yield return new WaitForSeconds(2.5f);
        SpawnNewTetromino();
    }

    public void SetHoldPieceIndexAndSpawnNewOne()
    {
        gm.SetSavedPieceIndex(currentIndex);
        gm.ClearAllFallingBlocks();
        ghostPiece.DestroyGhostPiece();
        SpawnNewTetromino();
        holdPieceUI.UpdateSprite(gm.GetSavedPieceIndex());
    }

    public void SwapTetromino()
    {
        gm.ClearAllFallingBlocks();
        ghostPiece.DestroyGhostPiece();
        SpawnHoldTetromino();
        gm.SetSavedPieceIndex(currentIndex);
        holdPieceUI.UpdateSprite(currentIndex);
    }
}
