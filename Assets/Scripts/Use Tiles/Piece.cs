using UnityEngine;

public class Piece : MonoBehaviour
{
    #region EnumVariables
    private enum Mode {
        SingleControl, DoubleControl
    };

    [SerializeField] private Mode mode;
    #endregion

    #region VectorVariables
    public Vector3Int[] blockCoordinates { get; private set; }
    public Vector3Int piecePosition { get; private set; }
    private Vector3Int newPiecePosition;
    private Vector2Int translation;
    #endregion

    #region FloatVariables
    [SerializeField] private float stepDelay;
    private float stepTime;
    [SerializeField] private float lockDelay;
    private float lockTime;
    private float targetPlayerTimer;
    #endregion

    #region BoolVariables
    private bool isValid;
    #endregion

    #region IntegerVariables
    private int rotationIndex;
    private int originalRotationIndex;
    private int wallKickIndex;
    #endregion

    #region OtherVariables
    public Board board { get; private set; }
    public TetrominoData tetrominoData { get; private set; }
    private PlayerPiece playerPiece;
    #endregion

    void Start()
    {
        playerPiece = GetComponent<PlayerPiece>();
         
        targetPlayerTimer = 0.0f;
    }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.tetrominoData = data;
        this.board = board;
        this.piecePosition = position;
        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        lockTime = 0.0f;

        if(blockCoordinates == null) blockCoordinates = new Vector3Int[data.blockCoordinates.Length];

        for (int i = 0; i < blockCoordinates.Length; i++) blockCoordinates[i] = (Vector3Int)data.blockCoordinates[i];
    }

    void Update()
    {
        board.ClearPiece(this);

        lockTime += Time.deltaTime;

        if(mode == Mode.SingleControl)
        {
            targetPlayerTimer += Time.deltaTime;

            if(targetPlayerTimer > 1.8f)
            {
                targetPlayerTimer = 0.0f;
                if(board.IsPlayerOnTheLeft(playerPiece, this, piecePosition, playerPiece.piecePosition)) Move(Vector2Int.left);
                else if(!board.IsPlayerOnTheLeft(playerPiece, this, piecePosition, playerPiece.piecePosition)) Move(Vector2Int.right);
            }
        }

        if(mode == Mode.DoubleControl)
        {
            if (Input.GetKeyDown(KeyCode.Q)) RotatePiece(-1);
            else if (Input.GetKeyDown(KeyCode.E)) RotatePiece(1);

            if(Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2Int.left);
            if(Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2Int.right);
        }

        if(Time.time >= stepTime) Step();

        board.SetPiece(this);
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if(lockTime >= lockDelay) Lock();
    }

    private void Lock()
    {
        board.SetPiece(this);
        board.SpawnPiece();
    }

    private bool Move(Vector2Int moveDirection)
    {
        newPiecePosition = this.piecePosition + (Vector3Int)moveDirection;

        isValid = board.IsValidPosition(this, newPiecePosition);

        if(isValid) 
        {
            piecePosition = newPiecePosition;
            lockTime = 0.0f;
        }

        return isValid;
    }

    void HardDrop()
    {
        while(Move(Vector2Int.down)) continue;

        Lock();
    }

    void RotatePiece(int direction)
    {
        originalRotationIndex = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);

        ApplyRotation(direction);

        if(!CheckWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotationIndex;
            ApplyRotation(-direction);
        }
    }

    private void ApplyRotation(int direction)
    {
        Vector3 blockCoordinate;
        int x,y;

        for(int i = 0; i < blockCoordinates.Length; i++)
        {
            blockCoordinate = blockCoordinates[i];

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

    private bool CheckWallKicks(int rotationIndex, int rotationDirection)
    {   
        wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for(int i = 0; i < tetrominoData.wallKicks.GetLength(1); i++)
        {
            translation = tetrominoData.wallKicks[wallKickIndex, i];

            if(Move(translation)) return true;
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        wallKickIndex = rotationIndex * 2;

        if(rotationDirection < 0) wallKickIndex--;

        return Wrap(wallKickIndex, 0, tetrominoData.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) return max - (min - input) % (max - min);
        else return min + (input - min) % (max - min);
    }
}
