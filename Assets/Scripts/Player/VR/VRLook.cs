using Unity.XR.CoreUtils;

public class VRLook : XROrigin
{
    public bool RotateAroundCameraUsingOriginRight(float angleDegrees)
    {
        return RotateAroundCameraPosition(Origin.transform.right, angleDegrees);
    }
}