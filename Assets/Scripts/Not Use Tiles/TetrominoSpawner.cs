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

    [SerializeField] private GameObject[] tetrominoes;
    [SerializeField] private Transform tetrominoesParent;

    void Start() => SpawnNewTetromino();
    public void SpawnNewTetromino() 
    {
        GameObject obj = Instantiate(tetrominoes[Random.Range(0, tetrominoes.Length)], transform.position, Quaternion.identity);

        obj.transform.SetParent(tetrominoesParent);
    }
}
