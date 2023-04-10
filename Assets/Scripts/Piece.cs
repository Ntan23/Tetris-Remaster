using UnityEngine;

public class Piece : MonoBehaviour
{
    #region VectorVariables
    public Vector3Int[] blocks { get; private set; }
    public Vector3Int position { get; private set; }
    #endregion

    #region OtherVariables
    public Board board { get; private set; }
    public TetrominoData tetrominoData { get; private set; }
    #endregion

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.tetrominoData = data;
        this.board = board;
        this.position = position;

        if(blocks == null) blocks = new Vector3Int[data.blocks.Length];

        for (int i = 0; i < blocks.Length; i++) blocks[i] = (Vector3Int)data.blocks[i];
    }

}
