using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenCheck : MonoBehaviour
{
    void Update()
    {
        if(transform.childCount == 0) Destroy(this.gameObject);
    }
}
