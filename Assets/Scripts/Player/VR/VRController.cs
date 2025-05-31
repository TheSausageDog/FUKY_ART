using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRController : ActionBasedController
{
    protected Vector3 offset;

    protected bool has_record_offset = false;

    protected override void ApplyControllerState(XRInteractionUpdateOrder.UpdatePhase updatePhase, XRControllerState controllerState)
    {
        if (controllerState == null)
            return;

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic ||
            updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender ||
            updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
        {
            if ((controllerState.inputTrackingState & InputTrackingState.Position) != 0)
            {
                if (has_record_offset)
                {
                    transform.localPosition = controllerState.position + offset;
                }
                else
                {
                    offset = transform.localPosition - controllerState.position;
                    has_record_offset = true;
                }
            }
        }

        base.ApplyControllerState(updatePhase, controllerState);
    }
}