using UnityEngine;

public class Steak : Food
{
    Bounds bounds;

    void Awake()
    {
        bounds = VolumeCalculator.CalculateWorldBounds(gameObject);

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}