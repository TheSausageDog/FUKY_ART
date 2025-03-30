using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CubeParticleGenerator : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private Vector2Int particleCountRange = new Vector2Int(5, 10); // Min-Max range for particle count
    [SerializeField] private Vector3 cubeSize = new Vector3(0.1f, 0.1f, 0.1f); // Smaller cubes for pepper effect
    [SerializeField] private Material cubeMaterial;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private Vector2 planeSize = new Vector2(1f, 1f); // Size of the plane to spawn from

    [Header("Physics Settings")]
    [SerializeField] private float mass = 0.1f;
    [SerializeField] private float drag = 0.1f;
    [SerializeField] private float angularDrag = 0.05f;
    [SerializeField] private float initialDownwardForce = 0.1f; // Small downward force

    [Header("Layer Settings")]
    [SerializeField] private int particleLayer = 10; // Layer for particles (default: 10)

    [Header("Generation Settings")]
    [SerializeField] private bool autoStart = true; // Whether to start generating automatically
    [SerializeField] private bool loopGeneration = true; // Whether to continuously generate particles
    [SerializeField] private float fadeOutDuration = 1.0f; // Duration for particles to fade out before being destroyed

    [Header("Events")]
    [SerializeField] private UnityEvent onGenerationStarted; // Event triggered when generation starts
    [SerializeField] private UnityEvent onGenerationStopped; // Event triggered when generation stops

    private bool isGenerating = false;
    private Coroutine generationCoroutine = null;

    // Public interface for external triggering
    public void StartGeneration()
    {
        if (!isGenerating)
        {
            isGenerating = true;

            if (generationCoroutine != null)
            {
                StopCoroutine(generationCoroutine);
            }

            generationCoroutine = StartCoroutine(GenerateCubes());
            onGenerationStarted?.Invoke();
        }
    }

    public void StopGeneration()
    {
        if (isGenerating)
        {
            isGenerating = false;

            if (generationCoroutine != null)
            {
                StopCoroutine(generationCoroutine);
                generationCoroutine = null;
            }

            onGenerationStopped?.Invoke();
        }
    }

    public void TriggerSingleGeneration()
    {
        StartCoroutine(GenerateSingleBatch());
    }

    private void Start()
    {
        // Create a layer for the particles if it doesn't exist
        // This is a reminder for the user to set up the layer in the Unity editor
        if (!LayerMask.LayerToName(particleLayer).Equals("PepperParticles"))
        {
            Debug.LogWarning("Please create a layer named 'PepperParticles' with index " + particleLayer +
                " and configure layer collision settings to ignore collisions between particles in this layer.");
        }

        // Ignore collisions between particles in the same layer
        Physics.IgnoreLayerCollision(particleLayer, particleLayer, true);

        // Create the plane if it doesn't exist as a child
        if (transform.childCount == 0)
        {
            // CreatePlane();
        }

        // Start generating cubes if autoStart is enabled
        if (autoStart)
        {
            StartGeneration();
        }
    }

    private void OnDisable()
    {
        // Stop generation when disabled
        StopGeneration();
    }

    private void CreatePlane()
    {
        // Create a simple plane as a child of this object
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "PepperShakerPlane";
        plane.transform.SetParent(transform);

        // Scale the plane to the desired size (default plane is 10x10 units)
        plane.transform.localScale = new Vector3(
            planeSize.x / 10f,
            1f,
            planeSize.y / 10f
        );

        // Position at the local origin
        plane.transform.localPosition = Vector3.zero;

        // Make the plane invisible but keep the transform
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        if (planeRenderer != null)
        {
            planeRenderer.enabled = false;
        }

        // Remove the collider from the plane
        Collider planeCollider = plane.GetComponent<Collider>();
        if (planeCollider != null)
        {
            Destroy(planeCollider);
        }
    }

    private IEnumerator GenerateCubes()
    {
        while (isGenerating)
        {
            // Generate a batch of cubes
            yield return GenerateSingleBatch();

            // If not looping, stop after one batch
            if (!loopGeneration)
            {
                StopGeneration();
                yield break;
            }

            // Wait for the specified interval
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator GenerateSingleBatch()
    {
        // Generate random number of cubes within the specified range
        int particleCount = Random.Range(particleCountRange.x, particleCountRange.y + 1);

        for (int i = 0; i < particleCount; i++)
        {
            CreateCube();

            // Small delay between each particle in the batch for more natural effect
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void CreateCube()
    {
        // Create a cube GameObject
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "PepperParticle";
        cube.tag = "PepperParticle";

        // Set random position within plane area
        Vector3 randomOffset = new Vector3(
            Random.Range(-planeSize.x / 2f, planeSize.x / 2f),
            0f,
            Random.Range(-planeSize.y / 2f, planeSize.y / 2f)
        );
        cube.transform.position = transform.position + randomOffset;

        // Set the cube size
        cube.transform.localScale = cubeSize;

        // Set material if provided
        if (cubeMaterial != null)
        {
            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material = new Material(cubeMaterial); // Create instance to allow individual fading
            renderer.material.name = "PepperParticleMaterial_Instance";
        }

        // Set the particle layer
        cube.layer = particleLayer;

        // Add Rigidbody for physics
        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.useGravity = true;

        rb.freezeRotation = true;

        // Add a small random rotation but only apply downward force
        rb.AddTorque(Random.insideUnitSphere * 0.5f, ForceMode.Impulse);
        rb.AddForce(Vector3.down * initialDownwardForce, ForceMode.Impulse);

        // Add fade out component
        FadeOutParticle fadeOut = cube.AddComponent<FadeOutParticle>();
        fadeOut.Initialize(lifeTime - fadeOutDuration, fadeOutDuration);
    }

    // Gizmo to visualize the spawn area in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, new Vector3(planeSize.x, 0.01f, planeSize.y));
    }


}