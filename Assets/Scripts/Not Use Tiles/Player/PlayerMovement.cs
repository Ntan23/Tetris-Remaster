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
    private Rigidbody2D rb;
    private GameManager gm;

    #endregion

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        gm = GameManager.Instance;
        transform.position = new Vector2(5, 22);
    }

    void Update()
    {
        DetectCollision();
        if(!gm.IsPlaying()) return;
        if (isMoving) return;
        
        if (Input.GetKeyDown(KeyCode.RightArrow) && !detectionCollider[2] && detectionCollider[6])
        {
            transform.GetChild(0).transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
            if (detectionCollider[4]) anchor = (Vector2)transform.position + new Vector2(0.5f, 0.5f);
            else anchor = (Vector2)transform.position + new Vector2(0.5f, -0.5f);
            axis = Vector3.back;

            nextPosition = transform.position + new Vector3(1, 0, 0);
            
            if(nextPosition.x <= 9.1f) StartCoroutine(RollCube(anchor, axis, true));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !detectionCollider[0] && detectionCollider[6])
        {
            transform.GetChild(0).transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
            if (detectionCollider[3]) anchor = (Vector2)transform.position + new Vector2(-0.5f, 0.5f);
            else anchor = (Vector2)transform.position + new Vector2(-0.5f, -0.5f);
            axis = Vector3.forward;

            nextPosition = transform.position + new Vector3(-1, 0, 0);

            if(nextPosition.x >= -0.1f) StartCoroutine(RollCube(anchor, axis, false));
        }

        if (!IsTherePossibleMove())
        {
            gameObject.GetComponent<Animator>().Play("DeathBeep");
            gm.GameOver();
        }
        if(AtTheTop()) gm.LevelUp();
    }

    //direction true = kanan 
    //!direction = kiri
    private IEnumerator RollCube(Vector3 anchor, Vector3 axis, bool direction)
    {
        float angleBefore = transform.rotation.z;
        float angleAfter;
        isMoving = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
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
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        isMoving = false;
    }

    private void DetectCollision()
    {
        for(int i = 0; i < detectionCollider.Length; i++) detectionCollider[i] = transform.GetChild(0).transform.GetChild(2).transform.GetChild(i).GetComponent<PositionDetection>().isInside;
    }

    private bool IsTherePossibleMove()
    {
        //Ensure that the player is grounded too
        if (detectionCollider[6])
        {
            if (Mathf.RoundToInt(transform.position.x) == 0) if (detectionCollider[2]) return false;

            if (Mathf.RoundToInt(transform.position.x) == 9) if (detectionCollider[0]) return false;

            if (Mathf.RoundToInt(transform.position.x) > 0 && Mathf.RoundToInt(transform.position.x) < 9)
            {
                if (detectionCollider[0] && detectionCollider[2]) return false;
                if (detectionCollider[0] && detectionCollider[2] && detectionCollider[3] && detectionCollider[4]) return false;
            }
        }


        return true;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool AtTheTop()
    {
        if(transform.position.y == 19) return true;
        else return false;
    }
}
