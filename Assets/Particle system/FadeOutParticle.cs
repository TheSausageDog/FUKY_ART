using UnityEngine;

public class FadeOutParticle : MonoBehaviour
{
    public Material material;

    private float delay = 0f;
    private float duration = 1f;
    private float startTime;
    // private Renderer rendererComponent;
    private Color originalColor;
    private bool isFading = false;

    public void Initialize(float delay, float duration)
    {
        this.delay = delay;
        this.duration = duration;
        this.startTime = Time.time;

        // rendererComponent = GetComponent<Renderer>();
        // if (rendererComponent != null && rendererComponent.material != null)
        // {
        //     originalColor = rendererComponent.material.color;
        // }

        // Destroy the object after the total lifetime
        Destroy(gameObject, delay + duration);
    }

    private void Update()
    {
        // Check if it's time to start fading
        if (!isFading && Time.time >= startTime + delay)
        {
            isFading = true;
        }

        // Handle fading
        if (isFading && material != null)
        {
            float elapsedTime = Time.time - (startTime + delay);
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);

            // Update alpha
            // Color newColor = originalColor;
            // newColor.a = Mathf.Lerp(originalColor.a, 0f, normalizedTime);

            // Apply the new color
            // rendererComponent.material.color = newColor;
            material.color = new Color(1, 1, 1, normalizedTime);
            // Make sure the material is set to fade
            // if (rendererComponent.material.HasProperty("_Mode"))
            // {
            //     rendererComponent.material.SetFloat("_Mode", 2); // Fade mode
            // }

            // Enable transparency
            // rendererComponent.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            // rendererComponent.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // rendererComponent.material.SetInt("_ZWrite", 0);
            // rendererComponent.material.DisableKeyword("_ALPHATEST_ON");
            // rendererComponent.material.EnableKeyword("_ALPHABLEND_ON");
            // rendererComponent.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            // rendererComponent.material.renderQueue = 3000;
        }
    }
}
