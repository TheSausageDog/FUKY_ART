using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HighLightedItem : MonoBehaviour
{
    protected List<Material> materials = new List<Material>();

    public bool isHighlighted = false;

    public float speed = 1f;

    protected float deltaIntensity;

    protected float intensity;

    void Awake()
    {
        Shader targetShader = Shader.Find("ToonLit/SceneToonLit");
        intensity = 0;
        deltaIntensity = speed;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
                foreach (var material in renderer.materials)
                {
                    if(material.shader == targetShader){
                        materials.Add(material);
                    }
                } 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isHighlighted || intensity != 0){
            intensity += deltaIntensity * Time.deltaTime;
            if(intensity < 0){ intensity = 0; if(deltaIntensity < 0){ deltaIntensity = -deltaIntensity; } }
            if(intensity > 1){ intensity = 1; if(deltaIntensity > 0){ deltaIntensity = -deltaIntensity; } }
            foreach (var material in materials)
            {
                material.SetFloat("_HalftoneEffect", intensity);
            }
        }
    }
}
