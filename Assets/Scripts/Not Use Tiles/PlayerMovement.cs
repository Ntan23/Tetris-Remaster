using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region BoolVariables
    private bool isMoving;
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
    }

    void Update()
    {
        if(!gm.IsPlaying()) return;
        if (isMoving) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anchor = (Vector2)transform.position + new Vector2(0.5f, -0.5f);
            axis = Vector3.back;

            nextPosition = transform.position + new Vector3(1, 0, 0);
            
            if(nextPosition.x <= 9.1f) StartCoroutine(rollCube(anchor, axis));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anchor = (Vector2)transform.position + new Vector2(-0.5f, -0.5f);
            axis = Vector3.forward;

            nextPosition = transform.position + new Vector3(-1, 0, 0);

            if(nextPosition.x >= -0.1f) StartCoroutine(rollCube(anchor, axis));
        }

        if(AtTheTop()) gm.LevelUp();
    }

    private IEnumerator rollCube(Vector3 anchor, Vector3 axis)
    {
        float angleBefore = transform.rotation.z;
        float angleAfter;
        isMoving = true;
        for(int i = 0; i < (90 / rollSpeed); i++)
        {
            transform.RotateAround(anchor, axis, rollSpeed);
            angleAfter = transform.rotation.z;
            if(angleAfter - angleBefore >= 90) transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(transform.rotation.z));
            yield return new WaitForSeconds(0.01f);
        }

        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Round(transform.rotation.z));
        isMoving = false;
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
