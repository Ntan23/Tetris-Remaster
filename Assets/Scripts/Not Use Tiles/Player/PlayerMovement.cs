using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region BoolVariables
    private bool isMoving;
    public bool[] detectionCollider = new bool[8];
    public bool[] isFallingDetected = new bool[2];
    // private bool runOnce;
    // private bool runFirstTime;
    private bool isFalling;
    private bool isFallingFromHeight;
    private bool canCheckPosition;
    // private bool moveFirstTime;
    #endregion

    #region FloatVariables
    [SerializeField] private float rollSpeed;
    [SerializeField] private float distance;
    #endregion

    #region VectorVariables
    private Vector3 axis;
    private Vector3 anchor;
    private Vector3 nextPosition;
    private Vector3 firstPos;
    private Vector3 lastPos;
    #endregion

    #region OtherVariables
    private Rigidbody2D rb;
    private GameManager gm;
    private AudioManager audioManager;
    private ParticleSystem dustEffect;
    private CameraShake cameraShake;
    #endregion

    private void Awake()
    {
        dustEffect = gameObject.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystem>();
        firstPos.y = transform.position.y;
    }

    void Start() 
    {
        audioManager = AudioManager.Instance;
        cameraShake = FindObjectOfType<CameraShake>();
        rb = GetComponent<Rigidbody2D>();
        gm = GameManager.Instance;
        transform.position = new Vector2(5, 25);

        // runFirstTime = true;
        // moveFirstTime = true;

        StartCoroutine(Wait());
    }

    void Update()
    {
        DetectCollision();
        HeroLandingEffect();

        if (!gm.IsPlaying()) return;
        if (isMoving) return;
        
        if(rb.velocity.y < 0f && !detectionCollider[6]) isFalling = true;
        
        if(isFalling && detectionCollider[6]) 
        {
            gm.CheckPlayerInLine();
            isFalling = false;
        }   

        if(!isFalling)
        {
            if((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.RightBracket)) && (!Input.GetKeyDown(KeyCode.LeftArrow) || !Input.GetKeyDown(KeyCode.LeftBracket)) && !detectionCollider[2] && detectionCollider[6]) MoveRight();

            if((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.LeftBracket)) && (!Input.GetKeyDown(KeyCode.RightArrow) || !Input.GetKeyDown(KeyCode.RightBracket)) && !detectionCollider[0] && detectionCollider[6]) MoveLeft();
        }

        if(!IsTherePossibleMove() && gm.IsPlaying()) gm.GameOver();

        if(AtTheTop() && canCheckPosition)
        {
            if(gm.GetCanLevelUp()) 
            {
                gm.LevelUp(true);
                StartCoroutine(WaitForPositionCheck());
            }
        } 
    }

    private void MoveRight()
    {
        transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;

        if (detectionCollider[4]) anchor = (Vector2)transform.position + new Vector2(0.5f, 0.5f);
        else anchor = (Vector2)transform.position + new Vector2(0.5f, -0.5f);
        axis = Vector3.back;

        nextPosition = transform.position + new Vector3(1, 0, 0);
                    
        if(nextPosition.x <= 9.1f) StartCoroutine(RollCube(anchor, axis, true));
    }

    private void MoveLeft()
    {
        transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;

        if (detectionCollider[3]) anchor = (Vector2)transform.position + new Vector2(-0.5f, 0.5f);
        else anchor = (Vector2)transform.position + new Vector2(-0.5f, -0.5f);
        axis = Vector3.forward;

        nextPosition = transform.position + new Vector3(-1, 0, 0);

        if(nextPosition.x >= -0.1f) StartCoroutine(RollCube(anchor, axis, false));
    }

    //direction true = kanan 
    //!direction = kiri
    private IEnumerator RollCube(Vector3 anchor, Vector3 axis, bool direction)
    {
        audioManager.PlayPlayerMovementSFX();
        rb.gravityScale = 0;
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
        rb.gravityScale = 1;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gm.CheckPlayerInLine();
        isMoving = false;
    }

    private void DetectCollision()
    {
        isFallingDetected[0] = transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<PositionDetection>().isFallingBlock;
        isFallingDetected[1] = transform.GetChild(0).transform.GetChild(1).transform.GetChild(2).GetComponent<PositionDetection>().isFallingBlock;

        for(int i = 0; i < detectionCollider.Length; i++) detectionCollider[i] = transform.GetChild(0).transform.GetChild(1).transform.GetChild(i).GetComponent<PositionDetection>().isInside;
    }

    private bool IsTherePossibleMove()
    {
        //Ensure that the player is grounded too
        if (detectionCollider[6])
        {
            if (Mathf.RoundToInt(transform.position.x) == 0) if (detectionCollider[2] && !isFallingDetected[1]) return false;

            if (Mathf.RoundToInt(transform.position.x) == 9) if (detectionCollider[0] && !isFallingDetected[0]) return false;

            if (Mathf.RoundToInt(transform.position.x) > 0 && Mathf.RoundToInt(transform.position.x) < 9)
            {
                if ((detectionCollider[0] && detectionCollider[2]) && !isFallingDetected[0] && !isFallingDetected[1]) return false;
                if (detectionCollider[0] && detectionCollider[2] && detectionCollider[3] && detectionCollider[4] && !(isFallingDetected[0] && isFallingDetected[1])) return false;
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
        if(Mathf.Round(transform.position.y) >= 18) return true;
        else return false;
    }

    private void HeroLandingEffect()
    {
        //Debug.Log(distance + " Last Pos: " + firstPos.y);
        // if (!detectionCollider[6])
        // {
        //     if (!runOnce && !IsMoving())
        //     {
        //         firstPos.y = transform.position.y;
        //         runOnce = true;
        //     }
        // }
        // if (detectionCollider[6])
        // {
        //     lastPos.y = transform.position.y;
        // }
        // distance = firstPos.y - lastPos.y;
        // if (detectionCollider[6] && distance > 4)
        // {
        //     runOnce = false;
        //     firstPos.y = transform.position.y;
        //     distance = 0;
        //     cameraShake.ShakeCamera(2, 1);
        //     audioManager.PlayHugeStomp();
        //     dustEffect.Play();
        // }

        if(rb.velocity.y < -5.0f && !detectionCollider[6]) isFallingFromHeight = true;
        
        if(isFallingFromHeight && detectionCollider[6]) 
        {
            cameraShake.ShakeCamera(2, 1);
            audioManager.PlayHugeStomp();
            dustEffect.Play();
            isFallingFromHeight = false;
        } 
    }

    public bool IsFalling()
    {
        return isFalling;
    }

    private void ThannosSlap()
    {
        gameObject.SetActive(false);
    }

    private void PlayTeleportAudio()
    {
        audioManager.PlayTeleport();
    }
    
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2.0f);
        canCheckPosition = true;
    }

    IEnumerator WaitForPositionCheck()
    {
        yield return new WaitForSeconds(2.0f);
        gm.ChangeBackCanLevelUp();
    }
}
