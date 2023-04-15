using UnityEngine;

// public class GhostPiece : MonoBehaviour
// {
//     #region Singleton
//     public static GhostPiece Instance;

//     void Awake()
//     {
//         if(Instance == null) Instance = this;
//     }
//     #endregion

//     private Vector3 position;
//     private Vector3Int roundedPosition;
//     private int roundedX;
//     private int roundedY;
//     private bool hasBlock;
//     public GameObject currentPiece;
//     private GameObject currentGhostPiece;
//     [SerializeField] private GameObject[] ghostPieces;
//     private GameManager gm;

//     void Start() => gm = GameManager.Instance;

//     public void Initialize(int index)
//     {
//         currentGhostPiece = Instantiate(ghostPieces[index], transform.position, Quaternion.identity);

//         currentGhostPiece.transform.position = currentPiece.transform.position;

//         currentGhostPiece.transform.SetParent(this.transform);
//     }

//     void Update()
//     {
//         if(currentPiece == null) return;

//         // Calculate potential drop positions for each block of the Tetromino
//         float minDistance = float.MaxValue;
//         foreach (Transform block in currentPiece.transform)
//         {
//             RaycastHit2D hit = Physics2D.Raycast(block.position, Vector2.down);
//             Debug.DrawRay(block.position, Vector2.down);
//             if(hit.collider != null && hit.collider.CompareTag("Block"))
//             {
//                 float distance = hit.distance; // Subtract half the block size
//                 if (distance < minDistance) minDistance = distance;
//             }
//             else
//             {
//                 // Block is at the bottom of the playfield
//                 minDistance = 0;
//                 break;
//             }
//         }
            
//         // Move the GhostPiece to the lowest potential drop position
//         Vector3 targetPosition = currentGhostPiece.transform.position;
//         targetPosition.y = currentPiece.transform.position.y - minDistance;
//         currentGhostPiece.transform.position = Vector3.MoveTowards(currentGhostPiece.transform.position, targetPosition, Time.deltaTime * 20f);
            
//         // Update the position and rotation of the GhostPiece
//         currentGhostPiece.transform.position = new Vector3(Mathf.Round(currentGhostPiece.transform.position.x), Mathf.Round(currentGhostPiece.transform.position.y), 0);
//         currentGhostPiece.transform.rotation = currentPiece.transform.rotation;
//     }

//     private bool IsValidPosition()
//     {
//         foreach(Transform children in currentGhostPiece.transform)
//         {
//             roundedPosition = RoundPosition(children.transform.position);

//             RaycastHit2D hit = Physics2D.Raycast((Vector2Int)roundedPosition, Vector2.zero);

//             if(hit.collider != null && hit.collider.gameObject.CompareTag("Blocks")) return false;

//             // if(roundedX < 0 || roundedX >= gm.GetBoardWidth() || roundedY < 0 || roundedY >= gm.GetBoardHeight()) return false;
         
//             // if(GameManager.coordinate[roundedX, roundedY] != null) return false;
//         }

//         return true;
//     }

//     private Vector3Int RoundPosition(Vector3 position) 
//     {
//         return new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
//     }

//     public void DestroyGhostPiece() => Destroy(currentGhostPiece);
// }


public class GhostPiece : MonoBehaviour
{
    //[SerializeField] private GameObject[] ghostPieces;
    public GameObject currentPiece; // Reference to the current Tetromino
    private GameObject ghostPiece;
    private GameManager gm;

    void Start() => gm = GameManager.Instance;

    void Update()
    {
        if(currentPiece == null) return;

        // Clone the current Tetromino and remove the Tetromino script from the clone
        GameObject clone = Instantiate(currentPiece, currentPiece.transform.position, currentPiece.transform.rotation);
        Destroy(clone.GetComponent<TetrisBlock>());

        // Disable the collider on the cloned Tetromino
        Collider2D[] colliders = clone.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders) collider.enabled = false;
        
        // Move the cloned Tetromino down one unit at a time until it collides
        Vector3 lastPosition = clone.transform.position;

        while(true)
        {
            clone.transform.position += Vector3.down;

            if (!IsValidPosition(clone)) break;
            
            lastPosition = clone.transform.position;
        }

        // Move the GhostPiece to the last valid position of the cloned Tetromino
        transform.position = lastPosition;

        // Update the position and rotation of the GhostPiece
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0);
        transform.rotation = currentPiece.transform.rotation;
        

        // Destroy the cloned Tetromino
        Destroy(clone);

        //Debug.Log("Ghost piece position: " + transform.position);
        // Debug.Log("Last valid position: " + lastPosition);
    }

    bool IsValidPosition(GameObject tetromino)
    {
        Vector3 position;

        foreach (Transform block in tetromino.transform)
        {
            position = block.position;
            if (!IsInsidePlayfield(position)) return false;

            if(GetBlockAtPosition(position) != null && GetBlockAtPosition(position).parent != tetromino.transform) return false;
        }

        return true;
    }

    private Transform GetBlockAtPosition(Vector3 position)
    {
        return GameManager.coordinate[(int)position.x, (int)position.y];
    }

    private bool IsInsidePlayfield(Vector3 position)
    {
        return position.x >= 0 && position.x < gm.GetBoardWidth() && position.y >= 0 && position.y < gm.GetBoardHeight();
    }

    public void DestroyGhostPiece() => Destroy(this.gameObject);
}

