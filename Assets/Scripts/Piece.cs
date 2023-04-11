using UnityEngine;

public class Piece : MonoBehaviour
{
    #region VectorVariables
    public Vector3Int[] blockCoordinates { get; private set; }
    public Vector3Int piecePosition { get; private set; }
    private Vector3Int newPiecePosition;
    #endregion

    #region FloatVariables
    private float timer;
    private float targetPlayerTimer;
    #endregion

    #region BoolVariables
    private bool isValid;
    #endregion

    #region IntegerVariables
    private int rotationIndex;
    #endregion

    #region OtherVariables
    public Board board { get; private set; }
    public TetrominoData tetrominoData { get; private set; }
    private PlayerPiece playerPiece;
    #endregion

    void Start()
    {
        playerPiece = GetComponent<PlayerPiece>();
         
        timer = 0.0f;
        targetPlayerTimer = 0.0f;
    }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.tetrominoData = data;
        this.board = board;
        this.piecePosition = position;
        rotationIndex = 0;

        if(blockCoordinates == null) blockCoordinates = new Vector3Int[data.blockCoordinates.Length];

        for (int i = 0; i < blockCoordinates.Length; i++) blockCoordinates[i] = (Vector3Int)data.blockCoordinates[i];
    }

    void Update()
    {
        board.ClearPiece(this);

        timer += Time.deltaTime;
        targetPlayerTimer += Time.deltaTime;

        if(timer > 1.0f)
        {
            timer = 0.0f;
            Move(Vector2Int.down);
        }

        // if (Input.GetKeyDown(KeyCode.Q)) RotatePiece(-1);
        // else if (Input.GetKeyDown(KeyCode.E)) RotatePiece(1);

        // if(Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector2Int.down);
        // if(Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2Int.left);
        // if(Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2Int.right);
        
        if(targetPlayerTimer > 2.0f)
        {
            targetPlayerTimer = 0.0f;
            if(board.IsPlayerOnTheLeft(playerPiece, this, piecePosition, playerPiece.piecePosition)) Move(Vector2Int.left);
            else if(!board.IsPlayerOnTheLeft(playerPiece, this, piecePosition, playerPiece.piecePosition)) Move(Vector2Int.right);
        }

        board.SetPiece(this);
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

    void RotatePiece(int direction)
    {
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);

        for(int i = 0; i < blockCoordinates.Length; i++)
        {
            Vector3 blockCoordinate = blockCoordinates[i];
            int x,y;

            switch(tetrominoData.tetromino)
            {
                case Tetromino.I :
                case Tetromino.O :
                    blockCoordinate.x -= 0.5f;
                    blockCoordinate.y -= 0.5f;

                    x = Mathf.CeilToInt((blockCoordinate.x * Data.RotationMatrix[0] * direction) + (blockCoordinate.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((blockCoordinate.x * Data.RotationMatrix[2] * direction) + (blockCoordinate.y * Data.RotationMatrix[3] * direction));
                    break;
                default :
                    x = Mathf.RoundToInt((blockCoordinate.x * Data.RotationMatrix[0] * direction) + (blockCoordinate.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((blockCoordinate.x * Data.RotationMatrix[2] * direction) + (blockCoordinate.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            blockCoordinates[i] = new Vector3Int(x, y, 0);
        }
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) return max - (min - input) % (max - min);
        else return min + (input - min) % (max - min);
    }
}
