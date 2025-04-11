using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    InteractionType interactionType { get; }

    void OnPickup(Transform holdPos);
    void OnThrow();

    Rigidbody _rb { get; }
    Collider _cd { get; }
    Transform _transform { get; }
    float _pickDelay { get; }
    float _objectSize { get; }
}

public enum InteractionType
{
    Pick,
    Check,
    Drag,
    Interact
}