using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInfoCell : MonoBehaviour
{
    public float time = 2;
    private void OnEnable()
    {
        Destroy(gameObject, time);
    }
}
