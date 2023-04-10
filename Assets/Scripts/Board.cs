using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    #region VectorVariables
    public Vector3Int spawnPosition;
    public Vector3Int playerSpawnPosition;
    private Vector3Int tilePosition;
    #endregion;

    #region IntegerVariables
    private int randomIndex;
    #endregion

    #region OtherVariables
    [SerializeField] private Tile player;
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public PlayerPiece playerPiece;
    public TetrominoData[] tetrominoes;
    #endregion

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
        randomIndex = Random.Range(0, tetrominoes.Length);

        activePiece.Initialize(this, spawnPosition, tetrominoes[randomIndex]);
        Set(this.activePiece);
    }

    private void SpawnPlayer()
    {
        playerPiece.Initialize(this, playerSpawnPosition, tetrominoes[tetrominoes.Length - 1]);
        SetPlayer(this.playerPiece);
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.blocks.Length; i++)
        {
            tilePosition = piece.blocks[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.tetrominoData.tile);
        }
    }

    public void SetPlayer(PlayerPiece playerPiece)
    {
        for (int i = 0; i < playerPiece.blocks.Length; i++)
        {
            tilePosition = playerPiece.blocks[i] + playerPiece.position;
            tilemap.SetTile(tilePosition, playerPiece.tetrominoData.tile);
        }
    }
}
