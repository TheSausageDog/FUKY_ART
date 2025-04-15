using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WaterFlow : MonoBehaviour
{
    public float angleTreshold = 30f;
    bool isUp =true;
    // private Coroutine generationCoroutine = null;
    
    // public GameObject item;
    // public Transform generatePos;

    public Color liquid_color;

    public ParticleSystem particle;

    public GameObject spray;

    public List<ParticleCollisionEvent> collisionEvents= new List<ParticleCollisionEvent>();

    void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent<LiquidContainer>(out var _bowl))
        {
            _bowl.AddLiquid(liquid_color, 1);
        }else{
            return;
            // particle.GetCollisionEvents(other, collisionEvents);
            // GameObject new_spray = Instantiate(spray);
            // new_spray.transform.forward = collisionEvents[0].normal;
            // new_spray.transform.position = collisionEvents[0].intersection;
            // // Debug.Log(collisionEvents[0].intersection + " " + other.name);
            // if(other.layer == LayerMask.NameToLayer("Default")){
            //     new_spray.transform.parent = other.transform;
            // }else{
            //     FadeOutParticle fadeOut = new_spray.AddComponent<FadeOutParticle>();
            //     fadeOut.Initialize(1, 1);
            // }
        }
    }

    private void Update()
    {
        if(isUp && Vector3.Angle(transform.up,-Vector3.up) < angleTreshold)
        {
            isUp = false;
            StartDrop();
        }
        else if(!isUp && Vector3.Angle(transform.up,-Vector3.up) > angleTreshold)
        {
            isUp = true;
            EndDrop();
        }
    }

    private void StartDrop()
    {
        if(particle != null){
            particle.Play();
            // particle.Emit(1);
        }
        // Debug.Log("start");
        // StartGeneration();
    }

    // private void StartGeneration()
    // {
    //     generationCoroutine = StartCoroutine(Generate());
    // }
    // IEnumerator Generate()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(0.5f);
    //         Instantiate(item, generatePos.position, Quaternion.identity);
    //     }  
    // }
    private void EndDrop()
    {
        particle.Stop();
        // Debug.Log("end");
        // StopGeneration();
    }

    // private void StopGeneration()
    // {
    //     if(generationCoroutine != null)
    //     {
    //         StopCoroutine(generationCoroutine);
    //         generationCoroutine = null;
    //     }
    // }
}