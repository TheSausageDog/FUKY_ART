# Pepper Particle Generator

This system creates a pepper shaker effect with small cube particles falling from a plane area.

## Features

- Generates multiple cube particles at regular intervals (default: every 0.5 seconds)
- **NEW**: Random number of particles per batch (default range: 5-10)
- **NEW**: Gradual fading out of particles instead of sudden disappearance
- **NEW**: External trigger interface for other scripts to control particle generation
- **NEW**: Toggle for continuous vs. one-time generation
- Particles spawn from a plane area rather than a single point
- Particles fall downward uniformly rather than flying in random directions
- Particles don't collide with each other but do collide with other scene objects
- Customizable settings for particle count, size, lifetime, spawn area, and physics properties

## How to Use

### Method 1: Using the Editor Menu

You have multiple options to create the Pepper Particle Generator:

- Go to **GameObject > Effects > Pepper Particle Generator**
- Go to **Assets > Create > Particle System > Pepper Particle Generator Prefab**

Any of these methods will create a GameObject with the CubeParticleGenerator component attached.

### Method 2: Using the Demo Script

1. Create an empty GameObject in your scene
2. Add the `CubeParticleGeneratorDemo` component to it
3. Adjust the position, plane size, particle count, and other settings in the Inspector
4. Play the scene, and the demo script will automatically create a pepper particle generator

### Method 3: Manual Setup

1. Create an empty GameObject in your scene
2. Add the `CubeParticleGenerator` component to it
3. Adjust the settings in the Inspector as needed
4. Create a layer named "PepperParticles" (or use another layer) and configure it to ignore collisions with itself

## Settings

### Particle Settings

- **Spawn Interval**: Time between particle spawns (default: 0.5 seconds)
- **Particle Count Range**: Min-Max range for number of particles to generate each time (default: 5-10)
- **Cube Size**: Size of the generated cubes (default: 0.1x0.1x0.1)
- **Cube Material**: Optional material to apply to the cubes
- **Life Time**: How long each cube exists before being destroyed (default: 5 seconds)
- **Fade Out Duration**: How long particles take to fade out before being destroyed (default: 1 second)
- **Plane Size**: Size of the plane area from which particles spawn (default: 1x1)

### Physics Settings

- **Mass**: Mass of each cube (default: 0.1)
- **Drag**: Linear drag applied to each cube (default: 0.1)
- **Angular Drag**: Angular drag applied to each cube (default: 0.05)
- **Initial Downward Force**: Force applied to make particles fall downward (default: 0.1)

### Generation Settings

- **Auto Start**: Whether to start generating particles automatically when the scene starts (default: true)
- **Loop Generation**: Whether to continuously generate particles or only generate once when triggered (default: true)

### Layer Settings

- **Particle Layer**: Layer to use for particles (default: 10)
  - This layer should be configured to ignore collisions with itself
  - You can set this up in Edit > Project Settings > Physics > Layer Collision Matrix

## External Triggering

The generator provides public methods that can be called from other scripts:

```csharp
// Start continuous particle generation
public void StartGeneration()

// Stop particle generation
public void StopGeneration()

// Generate a single batch of particles
public void TriggerSingleGeneration()
```

You can access these methods either directly from the CubeParticleGenerator component or through the CubeParticleGeneratorDemo component.

### Example: Triggering from another script

```csharp
// Reference to the generator
public CubeParticleGenerator pepperGenerator;

// Call this method to trigger a single batch of particles
public void SpawnPepper()
{
    pepperGenerator.TriggerSingleGeneration();
}

// Call this to start continuous generation
public void StartPepperFlow()
{
    pepperGenerator.StartGeneration();
}

// Call this to stop generation
public void StopPepperFlow()
{
    pepperGenerator.StopGeneration();
}
```

### Unity Events

The generator also provides Unity Events that you can subscribe to in the Inspector:

- **On Generation Started**: Triggered when particle generation starts
- **On Generation Stopped**: Triggered when particle generation stops

## Included Scripts

This package includes the following scripts:

1. **CubeParticleGenerator.cs**
   - The main component that generates pepper-like particles with physics
   - Creates an invisible plane from which particles spawn
   - Handles particle collision settings and fading

2. **FadeOutParticle.cs** (included in CubeParticleGenerator.cs)
   - Helper component added to each particle
   - Handles the gradual fading out of particles

3. **CubeParticleGeneratorDemo.cs**
   - A demo script that automatically creates a pepper particle generator
   - Provides easy access to all customizable settings
   - Includes visualization gizmos in the editor

4. **CubeParticleGeneratorPrefabSetup.cs**
   - Editor utility script that adds menu items for creating the generator
   - Provides multiple ways to create the generator in the Unity Editor
   - Sets appropriate defaults for the pepper shaker effect

5. **CubeParticleGeneratorSetup.cs** (in Editor folder)
   - Additional editor utility for creating the generator
   - Creates both a GameObject in the scene and a prefab in the project

## Notes

- The particles are automatically destroyed after their lifetime to prevent memory issues
- For best results, position the generator above other objects to allow particles to fall onto them
- The invisible plane that generates particles can be seen in the editor using gizmos
- You may need to adjust the particle size and count based on your scene scale
- For transparent materials, make sure they are set to the "Fade" rendering mode for proper alpha blending
