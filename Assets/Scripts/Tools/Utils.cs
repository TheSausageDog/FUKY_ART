using UnityEngine;

public static class Utils
{
    public static void SetLayerRecursive(Transform trans, string layer)
    {
        trans.gameObject.layer = LayerMask.NameToLayer(layer);
        foreach (Transform child in trans)
        {
            SetLayerRecursive(child, layer);
        }
    }
}