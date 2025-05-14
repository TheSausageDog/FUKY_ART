using Obi;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider))]
public class LiquidContainer : MonoBehaviour
{
    public ObiSolver solver;

    protected Collider containerArea;

    protected QueryShape queryShape;

    protected int queryIndex;

    void Awake()
    {
        containerArea = GetComponent<Collider>();
        if (containerArea.GetType() == typeof(BoxCollider))
        {
            BoxCollider boxCollider = (BoxCollider)containerArea;
            queryShape = new QueryShape
            {
                type = QueryShape.QueryType.Box,
                center = boxCollider.center,
                size = boxCollider.size,
                contactOffset = 0,
                maxDistance = 0,
                filter = ObiUtils.MakeFilter(ObiUtils.CollideWithEverything, 0)
            };
        }
        else
        {
            Debug.LogWarning("LiquidContainer 暂不支持其他触发器");
        }
    }

    void Update()
    {
        queryIndex = solver.EnqueueSpatialQuery(queryShape, new AffineTransform(transform.position, transform.rotation, transform.localScale));
    }

    private void Start()
    {
        solver.OnSpatialQueryResults += Solver_OnSpatialQueryResults;
    }

    private void OnDestroy()
    {
        solver.OnSpatialQueryResults -= Solver_OnSpatialQueryResults;
    }

    void Solver_OnSpatialQueryResults(ObiSolver s, ObiNativeQueryResultList queryResults)
    {
        for (int i = 0; i < queryResults.count; ++i)
        {
            if (queryResults[i].queryIndex == queryIndex)
            {
                if (queryResults[i].distance < 0)
                {
                    int particleIndex = solver.simplices[queryResults[i].simplexIndex];

                    // change the color of the particle depending on which box it is inside of:

                    solver.life[particleIndex] = 3;
                }
            }
        }
    }



    // public Material liquid_material;

    // public float threshold_start = 0f;

    // public float threshold_end = 0.4f;

    // private Color liquid_color;

    // private float liquid_volume;

    // public float liquid_full = 5;

    // public delegate void OnLiquidChanged(float delta_volume, WaterFlow source);

    // public OnLiquidChanged onLiquidChanged = (float _, WaterFlow _) => { };

    // public virtual void Start()
    // {
    //     AddLiquid(Color.black, 0);
    //     liquid_volume = 0;
    // }

    // public void AddLiquid(Color _liquid_color, float _liquid_volume, WaterFlow source = null)
    // {

    //     if (source != null) { onLiquidChanged(_liquid_volume, source); }
    //     if (liquid_volume == 0)
    //     {
    //         liquid_color = _liquid_color;
    //         liquid_volume = _liquid_volume;
    //     }
    //     else
    //     {
    //         liquid_volume += _liquid_volume;
    //         float alpha = _liquid_volume / liquid_volume;
    //         liquid_color = (1 - alpha) * liquid_color + alpha * _liquid_color;
    //     }

    //     liquid_color.a = 1;

    //     // float threshold = _threshold_start + (_threshold_end-_threshold_start) * Mathf.Pow(Mathf.Min(liquid_volume / liquid_full, 1f), 1f/2);
    //     float threshold = threshold_start + (threshold_end - threshold_start) * Mathf.Min(liquid_full, liquid_volume) / liquid_full;

    //     liquid_material.SetColor("_surface_color", liquid_color);
    //     liquid_material.SetFloat("_threshold", threshold);
    // }
}
