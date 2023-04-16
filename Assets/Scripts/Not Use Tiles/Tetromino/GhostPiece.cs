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

    void Start() 
    {
        gm = GameManager.Instance;

        if(gm.IsSingleControl()) this.enabled = false;
    } 

    public void Initialize(int index) => ghostPiece = Instantiate(ghostPieces[index], currentPiece.transform.position, Quaternion.identity);
    
    void LateUpdate()
    {
        if(currentPiece != null && gm.IsPlaying())
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
            position = gm.RoundPosition(block.position);

            if(!gm.IsInsidePlayfield(position)) return false;  

            if(gm.GetBlockAtPosition(position) != null && gm.GetBlockAtPosition(position).transform.parent != ghostPiece.transform) return false;
        }

        return true;
    }   

    public void DestroyGhostPiece() => Destroy(ghostPiece);
}

