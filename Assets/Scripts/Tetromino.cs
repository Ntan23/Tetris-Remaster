using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I, O, T, J, L, S, Z, Player
}

[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] blocks { get; private set; }

    public void Initialize()
    {
        blocks = Data.blocks[tetromino];
    }

}
