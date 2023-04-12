using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    bool isMoving;
    public float rollSpeed;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) return;

        Vector3 axis;
        Vector2 anchor;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anchor = (Vector2)transform.position + new Vector2(0.5f, -0.5f);
            axis = -Vector3.forward;

            StartCoroutine(rollCube(anchor, axis));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anchor = (Vector2)transform.position + new Vector2(-0.5f, -0.5f);
            axis = Vector3.forward;

            StartCoroutine(rollCube(anchor, axis));
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision)
        {
            Debug.Log("Not Hit");
        }
    }
}
