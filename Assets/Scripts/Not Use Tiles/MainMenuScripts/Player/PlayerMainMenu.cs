using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMainMenu : MonoBehaviour
{
    #region BoolVariables
    private bool isMoving;
    public bool[] detectionCollider = new bool[8];
    #endregion

    #region FloatVariables
    [SerializeField] private float rollSpeed;
    private float distance;
    #endregion

    #region VectorVariables
    private Vector3 axis;
    private Vector3 anchor;
    private Vector3 nextPosition;
    private Vector3 firstPos;
    #endregion

    #region OtherVariables
    private Rigidbody2D rb;
    private GameManager gm;
    private ParticleSystem dustEffect;
    #endregion

    private void Awake()
    {
        dustEffect = gameObject.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystem>();
        firstPos.y = transform.position.y;
    }

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        gm = GameManager.Instance;
    }


    void Update()
    {
        DetectCollision();
        HeroLandingEffect();
        if (isMoving) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) && !detectionCollider[2] && detectionCollider[6])
        {
            transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
            if (detectionCollider[4]) anchor = (Vector2)transform.position + new Vector2(0.5f, 0.5f);
            else anchor = (Vector2)transform.position + new Vector2(0.5f, -0.5f);
            axis = Vector3.back;

            StartCoroutine(RollCube(anchor, axis, true));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !detectionCollider[0] && detectionCollider[6])
        {
            transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
            if (detectionCollider[3]) anchor = (Vector2)transform.position + new Vector2(-0.5f, 0.5f);
            else anchor = (Vector2)transform.position + new Vector2(-0.5f, -0.5f);
            axis = Vector3.forward;

            StartCoroutine(RollCube(anchor, axis, false));
        }
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
        if (direction)
        {
            transform.position = new Vector2(Mathf.Round(transform.position.x) + 0.5f, Mathf.Round(transform.position.y) + 0.5f);
        }
        else transform.position = new Vector2(Mathf.Round(transform.position.x) - 0.5f, Mathf.Round(transform.position.y) + 0.5f);

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(transform.eulerAngles.z));
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        isMoving = false;
    }

    private void DetectCollision()
    {
        for (int i = 0; i < detectionCollider.Length; i++) detectionCollider[i] = transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).GetComponent<PositionDetection>().isInside;
    }


    public bool IsMoving()
    {
        return isMoving;
    }

    public bool AtTheTop()
    {
        if (transform.position.y == 19) return true;
        else return false;
    }

    private void HeroLandingEffect()
    {
        distance = firstPos.y - transform.position.y;
        if (!detectionCollider[6])
        {
            firstPos.y = transform.position.y;
            
        }
        else if (detectionCollider[6] && distance > 4)
        {
            distance = 0;
            dustEffect.Play();
        }
    }
}