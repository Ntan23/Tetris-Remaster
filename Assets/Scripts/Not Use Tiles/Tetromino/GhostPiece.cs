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
    public GameObject currentPiece; // Reference to the current Tetromino
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

    // void LateUpdate()
    // {
    //     if(currentPiece == null) return;

    //     // Clone the current Tetromino and remove the Tetromino script from the clone
    //     GameObject clone = Instantiate(currentPiece, currentPiece.transform.position, currentPiece.transform.rotation);
    //     Destroy(clone.GetComponent<TetrisBlock>());

    //     // Disable the collider on the cloned Tetromino
    //     Collider2D[] colliders = clone.GetComponentsInChildren<Collider2D>();

    //     foreach (Collider2D collider in colliders) collider.enabled = false;
        
    //     // Move the cloned Tetromino down one unit at a time until it collides
    //     Vector3 lastPosition = clone.transform.position;

    //     while (true)
    //     {
    //         clone.transform.position += Vector3.down;

    //         if (!IsValidPosition(clone, clone.transform.position)) break;
            
    //         lastPosition = clone.transform.position;
    //     }
        

    //     // Update the position and rotation of the GhostPiece
    //     ghostPiece.transform.position = new Vector3(Mathf.Round(lastPosition.x), Mathf.Round(lastPosition.y), 0);
    //     ghostPiece.transform.rotation = currentPiece.transform.rotation;
        
    //     // Destroy the cloned Tetromino
    //     Destroy(clone);

    //     //Debug.Log("Ghost piece position: " + ghostPiece.transform.position);
    //     //Debug.Log("Last valid position: " + lastPosition);
    // }

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

            // if(GetBlockAtPosition(position) != null && GetBlockAtPosition(position).transform.parent.CompareTag("Tetromino")) return true;

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
        return (int)position.x >= 0 && (int)position.x < gm.GetBoardWidth() && (int)position.y >= 0;
    }

    private Vector2 RoundPosition(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
    }
}

