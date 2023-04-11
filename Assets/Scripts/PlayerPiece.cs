using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPiece : MonoBehaviour
{
    #region VectorVariables
    public Vector3Int[] blockCoordinates { get; private set; }
    public Vector3Int piecePosition { get; private set; }
    private Vector3Int newPiecePosition;
    #endregion

    #region BoolVariables
    private bool isValid;
    private bool moveLeft;
    private bool canMove;
    #endregion

    #region FloatVarible
    [SerializeField] private float rotateSpeed;
    #endregion

    #region OtherVariables
    public Board board { get; private set; }
    private Tile playerTile;
    private Tilemap tilemap;
    public TetrominoData tetrominoData { get; private set; }
    private TileChangeData tileChangeData;
    private Matrix4x4 tileTransform;
    #endregion

    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        canMove = true;
    }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.tetrominoData = data;
        this.board = board;
        this.piecePosition = position;
        playerTile = board.tetrominoes[board.tetrominoes.Length - 1].tile;

        if(blockCoordinates == null) blockCoordinates = new Vector3Int[data.blockCoordinates.Length];

        for (int i = 0; i < blockCoordinates.Length; i++) blockCoordinates[i] = (Vector3Int)data.blockCoordinates[i];
    }

    void Update()
    {
        if(board.IsHitAboveHead(this, piecePosition)) 
        {
            Debug.Log("Game Over");
            return;
        }

        if(Input.GetKeyDown(KeyCode.A) && canMove) 
        {
            moveLeft = true;
            canMove = false;
            Move(Vector2Int.left);
        }
        if(Input.GetKeyDown(KeyCode.D) && canMove)
        {
            moveLeft = false;
            canMove = false;
            Move(Vector2Int.right);
        } 

        board.SetPlayer(this);
    }

    private bool Move(Vector2Int moveDirection)
    {
        newPiecePosition = piecePosition + (Vector3Int)moveDirection;

        isValid = board.IsPlayerValidPosition(this, newPiecePosition);

        if(isValid) StartCoroutine(RotateTiles());
        else if(!isValid) StartCoroutine(MoveDelay());

        return isValid;
    }

    IEnumerator RotateTiles()
    {
        if(moveLeft) tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 30));
        
        if(!moveLeft) tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -30));

        tileChangeData = new TileChangeData
        {
            position = newPiecePosition,
            tile = playerTile,
            color = Color.white,
            transform = tileTransform
        };

        tilemap.SetTile(tileChangeData, false);

        board.ClearPlayer(this);
        piecePosition = newPiecePosition;

        yield return new WaitForSeconds(0.1f);

        if(moveLeft) tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 30));
        
        if(!moveLeft) tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -60));

        tileChangeData = new TileChangeData
        {
            position = newPiecePosition,
            tile = playerTile,
            color = Color.white,
            transform = tileTransform
        };

        tilemap.SetTile(tileChangeData, false);

        yield return new WaitForSeconds(0.1f);

        if(moveLeft) tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90));
        
        if(!moveLeft)
        {
            tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90));
        } 

        tileChangeData = new TileChangeData
        {
            position = newPiecePosition,
            tile = playerTile,
            color = Color.white,
            transform = tileTransform
        };

        tilemap.SetTile(tileChangeData, false);
        StartCoroutine(MoveDelay());
    }

    IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
    }
}
