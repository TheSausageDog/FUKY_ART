using UnityEngine;
using UnityEditor;

public class CubeParticleGeneratorSetup
{
    [MenuItem("GameObject/Effects/Pepper Particle Generator (Legacy)")]
    static void CreateCubeParticleGenerator()
    {
        // Create an empty GameObject
        GameObject particleGenerator = new GameObject("PepperParticleGenerator");

        // Add the CubeParticleGenerator component
        CubeParticleGenerator generator = particleGenerator.AddComponent<CubeParticleGenerator>();

        // Configure default values for pepper shaker effect
        SetPepperShakeDefaults(generator);

        // Position the GameObject at the scene view pivot point or at zero if no scene view is available
        SceneView view = SceneView.lastActiveSceneView;
        if (view != null)
        {
            particleGenerator.transform.position = view.pivot;
        }
        else
        {
            particleGenerator.transform.position = new Vector3(0, 3, 0); // Default position higher up
        }

        // Select the created GameObject
        Selection.activeGameObject = particleGenerator;

        // Register the creation for undo
        Undo.RegisterCreatedObjectUndo(particleGenerator, "Create Pepper Particle Generator");

        // Log a message to the console
        Debug.Log("Pepper Particle Generator created. It will generate pepper-like particles from a plane area.");

        // Create the directory if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Particle system"))
        {
            AssetDatabase.CreateFolder("Assets", "Particle system");
        }

        // Create a prefab in the Particle system folder
        string prefabPath = "Assets/Particle system/PepperParticleGenerator.prefab";
        bool success = false;

#if UNITY_2018_3_OR_NEWER
        // Unity 2018.3 or newer uses PrefabUtility.SaveAsPrefabAsset
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(particleGenerator, prefabPath, out success);
#else
        // Older Unity versions use CreatePrefab
        Object prefab = PrefabUtility.CreatePrefab(prefabPath, particleGenerator);
        success = (prefab != null);
#endif

        if (success)
        {
            Debug.Log("Pepper Particle Generator prefab created at: " + prefabPath);
        }
        else
        {
            Debug.LogError("Failed to create Pepper Particle Generator prefab!");
        }
    }

    private static void SetPepperShakeDefaults(CubeParticleGenerator generator)
    {
        // Use reflection to set the private fields with pepper-appropriate defaults
        System.Type generatorType = typeof(CubeParticleGenerator);

        // Set smaller cube size for pepper-like particles
        var cubeSizeField = generatorType.GetField("cubeSize",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (cubeSizeField != null)
        {
            cubeSizeField.SetValue(generator, new Vector3(0.1f, 0.1f, 0.1f));
        }

        // Set particle count range
        var particleCountRangeField = generatorType.GetField("particleCountRange",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (particleCountRangeField != null)
        {
            particleCountRangeField.SetValue(generator, new Vector2Int(5, 10));
        }

        // Set smaller mass for pepper-like particles
        var massField = generatorType.GetField("mass",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (massField != null)
        {
            massField.SetValue(generator, 0.1f);
        }

        // Set plane size
        var planeSizeField = generatorType.GetField("planeSize",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (planeSizeField != null)
        {
            planeSizeField.SetValue(generator, new Vector2(0.5f, 0.5f));
        }

        // Set fade out duration
        var fadeOutDurationField = generatorType.GetField("fadeOutDuration",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (fadeOutDurationField != null)
        {
            fadeOutDurationField.SetValue(generator, 1.0f);
        }

        // Set auto start and loop generation
        var autoStartField = generatorType.GetField("autoStart",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (autoStartField != null)
        {
            autoStartField.SetValue(generator, true);
        }

        var loopGenerationField = generatorType.GetField("loopGeneration",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (loopGenerationField != null)
        {
            loopGenerationField.SetValue(generator, true);
        }
    }
}
