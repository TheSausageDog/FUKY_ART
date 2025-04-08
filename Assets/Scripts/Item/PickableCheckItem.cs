using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableCheckItem : BasePickableItem
{
    public override InteractionType interactionType{ get{ return InteractionType.Check; } }
}
