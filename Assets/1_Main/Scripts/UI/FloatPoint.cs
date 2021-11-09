using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatPoint : MonoBehaviour
{
    public float destroyTime;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
