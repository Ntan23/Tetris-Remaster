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
    public Vector2Int[] blockCoordinates { get; private set; }

    public void Initialize()
    {
        blockCoordinates = Data.blockCoordinates[tetromino];
    }

}
