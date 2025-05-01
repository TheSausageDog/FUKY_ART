using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatableItem : MonoBehaviour
{
    public abstract void Active();

    public abstract void Deactive();
}
