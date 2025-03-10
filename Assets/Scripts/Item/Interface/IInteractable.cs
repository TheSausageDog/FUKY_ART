using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    void Interact(InteractionType type,params object[] args);
    Rigidbody _rb { get; }
    Transform _transform { get; }
    float _pickDelay { get;}
}

public enum InteractionType
{
    Pick,
    Throw,
    Interact
}