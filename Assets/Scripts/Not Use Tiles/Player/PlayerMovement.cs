using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region BoolVariables
    private bool isMoving;
    public bool[] detectionCollider = new bool[8];
    #endregion

    #region FloatVariables
    [SerializeField] private float rollSpeed;
    #endregion

    #region VectorVariables
    private Vector3 axis;
    private Vector3 anchor;
    private Vector3 nextPosition;
    #endregion

    #region OtherVariables
    [SerializeField] private Transform playableArea;
    private Rigidbody2D rb;
    private GameManager gm;

    #endregion

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        gm = GameManager.Instance;
        transform.position = new Vector2(playableArea.transform.localScale.x / 2, playableArea.transform.localScale.y);
    }

    void Update()
    {
        collissionDetection();
        if(!gm.IsPlaying()) return;
        if (isMoving) return;
        
        if (Input.GetKeyDown(KeyCode.RightArrow) && !detectionCollider[2] && detectionCollider[6])
        {
            if (detectionCollider[4]) anchor = (Vector2)transform.position + new Vector2(0.5f, 0.5f);
            else anchor = (Vector2)transform.position + new Vector2(0.5f, -0.5f);
            axis = Vector3.back;

            nextPosition = transform.position + new Vector3(1, 0, 0);
            
            if(nextPosition.x <= 9.1f) StartCoroutine(rollCube(anchor, axis, true));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !detectionCollider[0] && detectionCollider[6])
        {
            if (detectionCollider[3]) anchor = (Vector2)transform.position + new Vector2(-0.5f, 0.5f);
            else anchor = (Vector2)transform.position + new Vector2(-0.5f, -0.5f);
            axis = Vector3.forward;

            nextPosition = transform.position + new Vector3(-1, 0, 0);

            if(nextPosition.x >= -0.1f) StartCoroutine(rollCube(anchor, axis, false));
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach(bool yes in detectionCollider)
            {
                Debug.Log("Test " + detectionCollider);
            }
            
        }
        if (detectionCollider[0] && detectionCollider[2]) gm.GameOver(); //kalo stuck kanan kiri ada 2 block ya tewas 
        if (AtTheTop()) gm.LevelUp();
    }
    //direction true = kanan 
    //!direction = kiri
    private IEnumerator rollCube(Vector3 anchor, Vector3 axis, bool direction)
    {
        float angleBefore = transform.rotation.z;
        float angleAfter;
        isMoving = true;
        if (detectionCollider[4] && direction) //kanan naik
        {
            for (int i = 0; i < (180 / rollSpeed); i++)
            {
                transform.RotateAround(anchor, axis, rollSpeed);
                angleAfter = transform.rotation.z;
                if (Mathf.Abs(angleAfter - angleBefore) >= 180)
                {
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(transform.rotation.z));
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (detectionCollider[3] && !direction) //kiri naik
        {
            for (int i = 0; i < (180 / rollSpeed); i++)
            {
                transform.RotateAround(anchor, axis, rollSpeed);
                angleAfter = transform.rotation.z;
                if (Mathf.Abs(angleAfter - angleBefore) >= 180)
                {
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(transform.rotation.z));
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (!detectionCollider[5] && !direction)
        {
            for (int i = 0; i < (180 / rollSpeed); i++)
            {
                transform.RotateAround(anchor, axis, rollSpeed);
                angleAfter = transform.rotation.z;
                if (Mathf.Abs(angleAfter - angleBefore) >= 180)
                {
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(transform.rotation.z));
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (!detectionCollider[7] && direction)
        {
            for (int i = 0; i < (180 / rollSpeed); i++)
            {
                transform.RotateAround(anchor, axis, rollSpeed);
                angleAfter = transform.rotation.z;
                if (Mathf.Abs(angleAfter - angleBefore) >= 180)
                {
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(transform.rotation.z));
                }
                yield return new WaitForSeconds(0.01f);
            }
        }

        else
        {
            for (int i = 0; i < (90 / rollSpeed); i++)
            {
                transform.RotateAround(anchor, axis, rollSpeed);
                angleAfter = transform.rotation.z;
                if (Mathf.Abs(angleAfter - angleBefore) >= 90)
                {
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(transform.rotation.z));
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(transform.eulerAngles.z));
        isMoving = false;

    }

    public void collissionDetection()
    {
        int i = 0;
        foreach(bool detection in detectionCollider)
        {
            detectionCollider[i] = transform.GetChild(0).transform.GetChild(2).transform.GetChild(i).GetComponent<PositionDetection>().isInside;
            i++;
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool AtTheTop()
    {
        if(transform.position.y == 15) return true;
        else return false;
    }
}
