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

    private int index;
    private int currentIndex;
    private int previousIndex;
    [SerializeField] private GameObject[] tetrominoes;
    [SerializeField] private Transform tetrominoesParent;
    [SerializeField] private NextPieceUI nextPieceUI;
    [SerializeField] private HoldPieceUI holdPieceUI;
    private GameManager gm;

    void Start() 
    {
        gm = GameManager.Instance;

        index = Random.Range(0, tetrominoes.Length);
        nextPieceUI.UpdateSprite(index);
        StartCoroutine(FirstSpawn());
    }
    
    public void SpawnNewTetromino() 
    {
        GameObject obj = Instantiate(tetrominoes[index], transform.position, Quaternion.identity);
        obj.transform.SetParent(tetrominoesParent);

        currentIndex = index;

        index = Random.Range(0, tetrominoes.Length);
        nextPieceUI.UpdateSprite(index);
    }

    private void SpawnHoldTetromino()
    {
        GameObject obj = Instantiate(tetrominoes[gm.GetSavedPieceIndex()], transform.position, Quaternion.identity);
    }

    IEnumerator FirstSpawn()
    {
        yield return new WaitForSeconds(2.0f);
        SpawnNewTetromino();
    }

    public void SetHoldPieceIndexAndSpawnNewOne()
    {
        gm.SetSavedPieceIndex(currentIndex);
        ClearAllFallingBlocks();
        SpawnNewTetromino();
        holdPieceUI.UpdateSprite(gm.GetSavedPieceIndex());
    }

    public void SwapTetromino()
    {
        ClearAllFallingBlocks();
        SpawnHoldTetromino();
        gm.SetSavedPieceIndex(currentIndex);
        holdPieceUI.UpdateSprite(currentIndex);
        currentIndex = gm.GetSavedPieceIndex();
    }

    private void ClearAllFallingBlocks()
    {
        GameObject[] fallingBlocks = GameObject.FindGameObjectsWithTag("FallingBlock");

        for(int i = 0; i < fallingBlocks.Length; i++)
        {
            Destroy(fallingBlocks[i]);
        }
    }
}
