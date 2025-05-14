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
    protected bool _isFlowing;
    public bool isFlowing
    {
        set
        {
            _isFlowing = value;
            emitter.gameObject.SetActive(_isFlowing);
        }
        get
        {
            return _isFlowing;
        }
    }

    // public float dropVolume = 0.1f;

    public Transform emitter;

    public Transform outlet;

    void Awake()
    {
        isFlowing = false;
    }


    public virtual void Update()
    {
        if (isFlowing)
        {
            emitter.position = outlet.position;
            emitter.rotation = outlet.rotation;
        }
    }
}