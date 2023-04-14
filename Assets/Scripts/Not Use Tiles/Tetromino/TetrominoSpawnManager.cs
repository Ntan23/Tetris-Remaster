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
    [SerializeField] private GameObject[] tetrominoes;
    [SerializeField] private Transform tetrominoesParent;
    [SerializeField] private NextPieceUI nextPieceUI;

    void Start() 
    {
        index = Random.Range(0, tetrominoes.Length);
        nextPieceUI.UpdateSprite(index);
        StartCoroutine(FirstSpawn());
    }
    
    public void SpawnNewTetromino() 
    {
        GameObject obj = Instantiate(tetrominoes[index], transform.position, Quaternion.identity);

        obj.transform.SetParent(tetrominoesParent);

        index = Random.Range(0, tetrominoes.Length);

        nextPieceUI.UpdateSprite(index);
    }

    IEnumerator FirstSpawn()
    {
        yield return new WaitForSeconds(2.0f);
        SpawnNewTetromino();
    }
}
