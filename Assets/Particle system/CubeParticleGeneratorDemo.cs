using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Demo script that creates a pepper shaker particle generator in the scene.
/// Attach this to any GameObject in your scene to automatically create a pepper particle generator.
/// </summary>
public class CubeParticleGeneratorDemo : MonoBehaviour
{
    [Header("Generator Settings")]
    [Tooltip("Position where the pepper particle generator will be created")]
    public Vector3 generatorPosition = new Vector3(0, 3, 0);
    
    [Tooltip("Reference to a material to apply to the particles (optional)")]
    public Material particleMaterial;
    
    [Header("Pepper Shaker Settings")]
    [Tooltip("Size of the plane that generates particles")]
    public Vector2 planeSize = new Vector2(0.5f, 0.5f);
    
    [Tooltip("Minimum and maximum number of particles to spawn each time")]
    public Vector2Int particleCountRange = new Vector2Int(5, 10);
    
    [Tooltip("Size of each particle")]
    public Vector3 particleSize = new Vector3(0.1f, 0.1f, 0.1f);
    
    [Tooltip("Layer to use for particles (should be configured to ignore self-collisions)")]
    [Range(8, 31)]
    public int particleLayer = 10;
    
    [Header("Generation Control")]
    [Tooltip("Whether to start generating particles automatically when the scene starts")]
    public bool autoStart = true;
    
    [Tooltip("Whether to continuously generate particles or only generate once when triggered")]
    public bool loopGeneration = true;
    
    [Tooltip("Time between particle generation batches (in seconds)")]
    [Range(0.1f, 5.0f)]
    public float spawnInterval = 0.5f;
    
    [Tooltip("Duration for particles to fade out before being destroyed (in seconds)")]
    [Range(0.1f, 3.0f)]
    public float fadeOutDuration = 1.0f;
    
    [Header("Events")]
    [Tooltip("Event triggered when particle generation starts")]
    public UnityEvent onGenerationStarted;
    
    [Tooltip("Event triggered when particle generation stops")]
    public UnityEvent onGenerationStopped;
    
    private GameObject particleGenerator;
    private CubeParticleGenerator generatorComponent;
    
    void Start()
    {
        // Create the pepper particle generator
        CreatePepperParticleGenerator();
    }
    
    /// <summary>
    /// Creates a pepper particle generator GameObject with the CubeParticleGenerator component
    /// </summary>
    private void CreatePepperParticleGenerator()
    {
        // Create an empty GameObject
        particleGenerator = new GameObject("PepperParticleGenerator");
        
        // Position it at the specified position
        particleGenerator.transform.position = generatorPosition;
        
        // Add the CubeParticleGenerator component
        generatorComponent = particleGenerator.AddComponent<CubeParticleGenerator>();
        
        // Configure the generator with our settings
        ConfigureGenerator();
        
        Debug.Log("Pepper Particle Generator created at position " + generatorPosition);
    }
    
    private void ConfigureGenerator()
    {
        if (generatorComponent == null) return;
        
        // Use reflection to set the private fields
        System.Type generatorType = typeof(CubeParticleGenerator);
        
        // Set the material if provided
        if (particleMaterial != null)
        {
            var materialField = generatorType.GetField("cubeMaterial", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            
            if (materialField != null)
            {
                materialField.SetValue(generatorComponent, particleMaterial);
            }
        }
        
        // Set the plane size
        var planeSizeField = generatorType.GetField("planeSize", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (planeSizeField != null)
        {
            planeSizeField.SetValue(generatorComponent, planeSize);
        }
        
        // Set the particle count range
        var particleCountRangeField = generatorType.GetField("particleCountRange", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (particleCountRangeField != null)
        {
            particleCountRangeField.SetValue(generatorComponent, particleCountRange);
        }
        
        // Set the particle size
        var cubeSizeField = generatorType.GetField("cubeSize", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (cubeSizeField != null)
        {
            cubeSizeField.SetValue(generatorComponent, particleSize);
        }
        
        // Set the particle layer
        var particleLayerField = generatorType.GetField("particleLayer", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (particleLayerField != null)
        {
            particleLayerField.SetValue(generatorComponent, particleLayer);
        }
        
        // Set auto start
        var autoStartField = generatorType.GetField("autoStart", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (autoStartField != null)
        {
            autoStartField.SetValue(generatorComponent, autoStart);
        }
        
        // Set loop generation
        var loopGenerationField = generatorType.GetField("loopGeneration", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (loopGenerationField != null)
        {
            loopGenerationField.SetValue(generatorComponent, loopGeneration);
        }
        
        // Set spawn interval
        var spawnIntervalField = generatorType.GetField("spawnInterval", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (spawnIntervalField != null)
        {
            spawnIntervalField.SetValue(generatorComponent, spawnInterval);
        }
        
        // Set fade out duration
        var fadeOutDurationField = generatorType.GetField("fadeOutDuration", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (fadeOutDurationField != null)
        {
            fadeOutDurationField.SetValue(generatorComponent, fadeOutDuration);
        }
        
        // Set events
        var onGenerationStartedField = generatorType.GetField("onGenerationStarted", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (onGenerationStartedField != null && onGenerationStarted != null)
        {
            onGenerationStartedField.SetValue(generatorComponent, onGenerationStarted);
        }
        
        var onGenerationStoppedField = generatorType.GetField("onGenerationStopped", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        
        if (onGenerationStoppedField != null && onGenerationStopped != null)
        {
            onGenerationStoppedField.SetValue(generatorComponent, onGenerationStopped);
        }
    }
    
    // Public methods to control the generator from other scripts or UI
    
    /// <summary>
    /// Starts continuous particle generation
    /// </summary>
    public void StartGeneration()
    {
        if (generatorComponent != null)
        {
            generatorComponent.StartGeneration();
        }
    }
    
    /// <summary>
    /// Stops particle generation
    /// </summary>
    public void StopGeneration()
    {
        if (generatorComponent != null)
        {
            generatorComponent.StopGeneration();
        }
    }
    
    /// <summary>
    /// Triggers a single batch of particles
    /// </summary>
    public void TriggerSingleGeneration()
    {
        if (generatorComponent != null)
        {
            generatorComponent.TriggerSingleGeneration();
        }
    }
    
    void OnDestroy()
    {
        // Clean up when this script is destroyed
        if (particleGenerator != null)
        {
            Destroy(particleGenerator);
        }
    }
    
    // Draw gizmo to show where the generator will be placed
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(generatorPosition, 0.2f);
        
        // Draw the plane area
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f); // Orange semi-transparent
        Matrix4x4 originalMatrix = Gizmos.matrix;
        
        // Create a transformation matrix for the plane
        Matrix4x4 planeMatrix = Matrix4x4.TRS(
            generatorPosition,
            Quaternion.identity,
            new Vector3(planeSize.x, 0.01f, planeSize.y)
        );
        
        Gizmos.matrix = planeMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        
        // Restore original matrix
        Gizmos.matrix = originalMatrix;
    }
}
