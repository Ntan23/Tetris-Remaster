using UnityEngine;

public class GhostPiece : MonoBehaviour
{
    #region Singleton
    public static GhostPiece Instance;

    void Awake()
    {
        if(Instance == null) Instance = this;
    }
    #endregion

    [SerializeField] private GameObject[] ghostPieces;
    public GameObject currentPiece;
    private GameObject ghostPiece;
    private GameManager gm;

    void Start() => gm = GameManager.Instance;

    public void Initialize(int index) => ghostPiece = Instantiate(ghostPieces[index], currentPiece.transform.position, Quaternion.identity);
    
    void LateUpdate()
    {
        if(currentPiece != null)
        {
            FollowActiveTetromino();
            MoveDown();
        }
    }

    private void FollowActiveTetromino()
    {
        ghostPiece.transform.position = currentPiece.transform.position;
        ghostPiece.transform.rotation = currentPiece.transform.rotation;
    }

    private void MoveDown()
    {
        while(IsValidPosition()) ghostPiece.transform.position += Vector3.down;

        if(!IsValidPosition()) ghostPiece.transform.position += Vector3.up;
    }

    private bool IsValidPosition()
    {
        Vector3 position;

        foreach (Transform block in ghostPiece.transform)
        {
            position = RoundPosition(block.position);

            if(!IsInsidePlayfield(position)) return false;  

            if(GetBlockAtPosition(position) != null && GetBlockAtPosition(position).transform.parent != ghostPiece.transform) return false;
        }

        return true;
    }   

    public void DestroyGhostPiece() => Destroy(ghostPiece);

    private Transform GetBlockAtPosition(Vector3 position)
    {
        return GameManager.coordinate[(int)position.x, (int)position.y];
    }

    private bool IsInsidePlayfield(Vector3 position)
    {
        return position.x >= 0 && position.x < gm.GetBoardWidth() && position.y >= 0 && position.y < gm.GetBoardHeight();
    }

    private Vector2 RoundPosition(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }
}

