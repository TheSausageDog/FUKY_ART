using Obi;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;


public class LiquidContainer : MonoBehaviour
{
    public Material liquid_material;

    public float threshold_start = 0f;

    public float threshold_end = 0.4f;

    private Color liquid_color;

    private float liquid_volume;

    public float liquid_full = 5;

    public virtual void Start()
    {
        AddLiquid(Color.black, 0);
        liquid_volume = 0;
    }

    public void AddLiquid(Color _liquid_color, float _liquid_volume)
    {

        if (liquid_volume == 0)
        {
            liquid_color = _liquid_color;
            liquid_volume = _liquid_volume;
        }
        else
        {
            liquid_volume += _liquid_volume;
            float alpha = _liquid_volume / liquid_volume;
            liquid_color = (1 - alpha) * liquid_color + alpha * _liquid_color;
        }

        liquid_color.a = 1;

        // float threshold = _threshold_start + (_threshold_end-_threshold_start) * Mathf.Pow(Mathf.Min(liquid_volume / liquid_full, 1f), 1f/2);
        float threshold = threshold_start + (threshold_end - threshold_start) * Mathf.Min(liquid_full, liquid_volume) / liquid_full;

        liquid_material.SetColor("_surface_color", liquid_color);
        liquid_material.SetFloat("_threshold", threshold);
    }
}
