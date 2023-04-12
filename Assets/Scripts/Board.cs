using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    #region VectorVariables
    public Vector3Int spawnPosition;
    public Vector3Int playerSpawnPosition { get; private set; }
    private Vector3Int tilePosition;
    private Vector3Int playerPosition;
    private Vector3Int playerHeadPosition;
    private Vector3Int playerNextLocation;
    private Vector3Int playerLeftRightLocation;
    public Vector2Int boardSize;
    #endregion;

    #region IntegerVariables
    private int randomIndex;
    #endregion

    #region OtherVariables
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public PlayerPiece playerPiece { get; private set; }
    public TetrominoData[] tetrominoes;
    private RectInt bounds;
    #endregion

    //For The Bporad Bounds
    public RectInt BoardBound
    {
        get 
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2 , -boardSize.y / 2);

            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        playerPiece = GetComponentInChildren<PlayerPiece>();

        for (int i = 0; i < tetrominoes.Length; i++) tetrominoes[i].Initialize();
    }

    private void Start() 
    {
        SpawnPiece();
        SpawnPlayer();
    }

    public void SpawnPiece()
    {
        randomIndex = Random.Range(0, tetrominoes.Length - 1);

        spawnPosition = new Vector3Int(Random.Range(-4, 3), 8, 0); 
        activePiece.Initialize(this, spawnPosition, tetrominoes[randomIndex]);
        SetPiece(activePiece);
    }

    private void SpawnPlayer()
    {   
        playerSpawnPosition = new Vector3Int(Random.Range(-5, 4), -10, 0); 

        playerPiece.Initialize(this, playerSpawnPosition, tetrominoes[tetrominoes.Length - 1]);
        SetPlayer(playerPiece);
    }

    public void SetPiece(Piece piece)
    {
        for (int i = 0; i < piece.blockCoordinates.Length; i++)
        {
            tilePosition = piece.blockCoordinates[i] + piece.piecePosition;
            tilemap.SetTile(tilePosition, piece.tetrominoData.tile);
        }
    }

    public void ClearPiece(Piece piece)
    {
        for (int i = 0; i < piece.blockCoordinates.Length; i++)
        {
            tilePosition = piece.blockCoordinates[i] + piece.piecePosition;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public void SetPlayer(PlayerPiece playerPiece)
    {
        for (int i = 0; i < playerPiece.blockCoordinates.Length; i++)
        {
            playerPosition = playerPiece.blockCoordinates[i] + playerPiece.piecePosition;
            tilemap.SetTile(playerPosition, playerPiece.tetrominoData.tile);
        }
    }

    public void ClearPlayer(PlayerPiece playerPiece)
    {
        for (int i = 0; i < playerPiece.blockCoordinates.Length; i++)
        {
            playerPosition = playerPiece.blockCoordinates[i] + playerPiece.piecePosition;
            tilemap.SetTile(playerPosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        bounds = BoardBound;

        for(int i = 0; i < piece.blockCoordinates.Length; i++)
        {
            tilePosition = piece.blockCoordinates[i] + position;

            if(!bounds.Contains((Vector2Int)tilePosition)) return false;

            if(tilemap.HasTile(tilePosition)) return false;
        }

        return true;
    }

    public bool IsPlayerValidPosition(PlayerPiece playerPiece, Vector3Int position)
    {
        bounds = BoardBound;

        for(int i = 0; i < playerPiece.blockCoordinates.Length; i++)
        {
            playerPosition = playerPiece.blockCoordinates[i] + position;

            if(!bounds.Contains((Vector2Int)playerPosition)) return false;

            if(tilemap.HasTile(playerPosition)) return false;
        }

        return true;
    }

    public bool IsHitAboveHead(PlayerPiece playerPiece, Vector3Int position)
    {
        for(int i = 0; i < playerPiece.blockCoordinates.Length; i++) 
        {
            playerHeadPosition = playerPiece.blockCoordinates[i] + position + new Vector3Int(0, 1, 0);

            if(tilemap.HasTile(playerHeadPosition)) return true;
        }

        return false;
    }

    public bool IsPlayerOnTheLeft(PlayerPiece playerPiece, Piece piece, Vector3Int position, Vector3Int playerPosition)
    {
        for(int i = 0; i < piece.blockCoordinates.Length; i++)
        {
            tilePosition = piece.blockCoordinates[i] + position;

            for(int j = 0; j < playerPiece.blockCoordinates.Length; j++)
            {
                playerPosition = playerPiece.blockCoordinates[j] + playerPosition;
    
                if(tilePosition.x > playerPosition.x) return true;
            }
        }
        
        return false;
    }

    //Direction --> -1 Left , 1 Right
    public bool PlayerCanMoveUp(PlayerPiece playerPiece, Vector3Int position, int direction)
    {   
        for(int i = 0; i < playerPiece.blockCoordinates.Length; i++) 
        {
            playerNextLocation = playerPiece.blockCoordinates[i] + position + new Vector3Int(direction, 1, 0);
            playerLeftRightLocation = playerPiece.blockCoordinates[i] + position + new Vector3Int(direction, 0, 0);

            if(!tilemap.HasTile(playerNextLocation) && tilemap.HasTile(playerLeftRightLocation) && bounds.Contains((Vector2Int) playerNextLocation)) return true;
        }   
    
        return false;
    }
}
