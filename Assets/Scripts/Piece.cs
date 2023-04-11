using UnityEngine;

public class Piece : MonoBehaviour
{
    #region VectorVariables
    public Vector3Int[] blockCoordinates { get; private set; }
    public Vector3Int piecePosition { get; private set; }
    private Vector3Int newPiecePosition;
    #endregion

    #region FloatVariables
    private float timer = 0.0f;
    #endregion

    #region BoolVariables
    private bool isValid;
    #endregion

    #region OtherVariables
    public Board board { get; private set; }
    public TetrominoData tetrominoData { get; private set; }
    #endregion

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.tetrominoData = data;
        this.board = board;
        this.piecePosition = position;

        if(blockCoordinates == null) blockCoordinates = new Vector3Int[data.blockCoordinates.Length];

        for (int i = 0; i < blockCoordinates.Length; i++) blockCoordinates[i] = (Vector3Int)data.blockCoordinates[i];
    }

    void Update()
    {
        this.board.ClearPiece(this);

        // timer += Time.deltaTime;

        // if(timer > 1.0f)
        // {
        //     timer = 0.0f;
        //     Move(Vector2Int.down);
        // }

        this.board.SetPiece(this);
    }

    private bool Move(Vector2Int moveDirection)
    {
        newPiecePosition = this.piecePosition + (Vector3Int)moveDirection;

        isValid = this.board.IsValidPosition(this, newPiecePosition);

        if(isValid) this.piecePosition = newPiecePosition;

        return isValid;
    }

    void HardDrop()
    {
        while(Move(Vector2Int.down)) continue;
    }
}
