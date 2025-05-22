using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.ParticleSystem;

public class WaterFlow : MonoBehaviour
{
    // public float angleTreshold = 30f;
    public float speed = 1;
    protected bool _isFlowing;
    public bool isFlowing
    {
        set
        {
            _isFlowing = value;
            obiEmitter.speed = _isFlowing ? speed : 0;
        }
        get
        {
            return _isFlowing;
        }
    }

    // public float dropVolume = 0.1f;

    public Transform outlet;

    protected ObiEmitter obiEmitter;

    void Awake()
    {
        obiEmitter = GetComponent<ObiEmitter>();
        isFlowing = false;
    }


    public virtual void Update()
    {
        if (isFlowing)
        {
            transform.position = outlet.position;
            transform.rotation = outlet.rotation;
        }
    }
}