using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonRotatingPart : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
