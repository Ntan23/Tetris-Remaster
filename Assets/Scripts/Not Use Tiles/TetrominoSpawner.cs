using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    #region Singleton
    public static TetrominoSpawner Instance;

    void Awake()
    {
        if(Instance == null) Instance = this;
    }
    #endregion

    private int index;
    [SerializeField] private GameObject[] tetrominoes;
    [SerializeField] private Transform tetrominoesParent;

    void Start() 
    {
        index = Random.Range(0, tetrominoes.Length);
        SpawnNewTetromino();
    }
    
    public void SpawnNewTetromino() 
    {
        GameObject obj = Instantiate(tetrominoes[index], transform.position, Quaternion.identity);

        obj.transform.SetParent(tetrominoesParent);

        index = Random.Range(0, tetrominoes.Length);
    }
}
