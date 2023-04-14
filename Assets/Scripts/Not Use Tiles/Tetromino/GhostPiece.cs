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

    private Vector3 position;
    private int roundedX;
    private int roundedY;
    public GameObject currentPiece;
    private GameObject currentGhostPiece;
    [SerializeField] private GameObject[] ghostPieces;
    private GameManager gm;

    void Start() => gm = GameManager.Instance;

    public void Initialize(int index)
    {
        currentGhostPiece = Instantiate(ghostPieces[index], transform.position, Quaternion.identity);

        currentGhostPiece.transform.position = currentPiece.transform.position;

        currentGhostPiece.transform.SetParent(this.transform);
    }

    void LateUpdate()
    {
        if(currentPiece != null)
        {
            if(IsValidPosition()) position += Vector3.down;
            else if(!IsValidPosition()) position -= Vector3.down;
            
            currentGhostPiece.transform.position = new Vector3(currentPiece.transform.position.x, position.y, 0);
            currentGhostPiece.transform.rotation = currentPiece.transform.rotation;
        }
    }

    private bool IsValidPosition()
    {
        foreach(Transform children in currentGhostPiece.transform)
        {
            roundedX = Mathf.RoundToInt(children.position.x);
            roundedY = Mathf.RoundToInt(children.position.y);

            if(roundedX < 0 || roundedX >= gm.GetBoardWidth() || roundedY < 0 || roundedY >= gm.GetBoardHeight()) return false;
         
            if(GameManager.coordinate[roundedX, roundedY] != null) return false;
        }

        return true;
    }

    public void DestroyGhostPiece() => Destroy(currentGhostPiece);
}
