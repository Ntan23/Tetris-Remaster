using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    #region FloatVariables
    private float fallTimer;
    [SerializeField] private float fallTimeDelay;
    #endregion

    #region IntegerVariables
    public static int width = 10;
    public static int height = 20;
    private int roundedX;
    private int roundedY;
    #endregion

    #region VectorVariables
    [SerializeField] private Vector3 rotationPoint;
    #endregion

    #region OtherVariables
    private static Transform[,] coordinate = new Transform[width, height];
    private TetrominoSpawner tetrominoSpawner;
    #endregion

    void Start() => tetrominoSpawner = TetrominoSpawner.Instance;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) 
        {
            transform.position += new Vector3(-1, 0, 0);

            if(!IsValidMove()) transform.position -= new Vector3(-1, 0, 0);
        }
        else if(Input.GetKeyDown(KeyCode.D)) 
        {
            transform.position += new Vector3(1, 0, 0);

            if(!IsValidMove()) transform.position -= new Vector3(1, 0, 0);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), 90);

            if(!IsValidMove()) transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), -90);
        } 
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), -90);

            if(!IsValidMove()) transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0,0,1), 90);
        }

        fallTimer += Time.deltaTime;
        
        if(fallTimer > (Input.GetKey(KeyCode.S) ? fallTimeDelay / 10 : fallTimeDelay))
        {
            transform.position += new Vector3(0, -1, 0);
            
            if(!IsValidMove()) 
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckForLineComplete();
                this.enabled = false;
                tetrominoSpawner.SpawnNewTetromino();
            }

            fallTimer = 0.0f;
        }
    }

    private bool IsValidMove()
    {
        foreach(Transform children in transform)
        {
            roundedX = Mathf.RoundToInt(children.transform.position.x);
            roundedY = Mathf.RoundToInt(children.transform.position.y);

            if(roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height) return false;

            if(coordinate[roundedX, roundedY] != null) return false;
        }

        return true;
    }

    private void AddToGrid()
    {
        foreach(Transform children in transform)
        {
            roundedX = Mathf.RoundToInt(children.transform.position.x);
            roundedY = Mathf.RoundToInt(children.transform.position.y);
        
            coordinate[roundedX, roundedY] = children;
        }
    }

    private void CheckForLineComplete()
    {
        for(int i = height - 1; i >= 0; i--)
        {
            if(HasLine(i)) 
            {
                DeleteLine(i);
                MoveRowDown(i);
            }
        }
    }

    private bool HasLine(int verticalCoordinate)
    {
        for(int i = 0; i < width; i++)
        {
            if(coordinate[i, verticalCoordinate] == null) return false;
        }

        return true;
    }

    private void DeleteLine(int verticalCoordinate)
    {
        for(int i = 0; i < width; i++)
        {
            Destroy(coordinate[i, verticalCoordinate].gameObject);
            coordinate[i, verticalCoordinate] = null;
        }
    }

    private void MoveRowDown(int verticalCoordinate)
    {
        for(int i = verticalCoordinate; i  < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                if(coordinate[j, i] != null)
                {
                    coordinate[j, i - 1] = coordinate[j, i];
                    coordinate[j, i] = null;
                    coordinate[j, i - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }
}
