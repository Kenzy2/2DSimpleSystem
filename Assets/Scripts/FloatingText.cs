using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float destroyTime = 3f;
    private Vector3 Offset = new Vector3(0, 1, 0);
    public Vector3 RandomizeIntensity = new Vector3();
    void Start()
    {
        Destroy(this.gameObject, destroyTime);
        transform.localPosition += Offset;
        transform.localPosition += new Vector3(UnityEngine.Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
        UnityEngine.Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y), 0);
    }
}