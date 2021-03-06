﻿using UnityEngine;
using System.Collections;

public class DestroySplash : MonoBehaviour
{
    private const float delay = -0.5f;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }
}