using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class ContinuousTurnAndNodProvider : LocomotionProvider
{
    [SerializeField]
    [Tooltip("The number of degrees/second clockwise to rotate when turning clockwise.")]
    float m_TurnSpeed = 60f;
    /// <summary>
    /// The number of degrees/second clockwise to rotate when turning clockwise.
    /// </summary>
    public float turnSpeed
    {
        get => m_TurnSpeed;
        set => m_TurnSpeed = value;
    }

    bool m_IsTurningXROrigin;

    [SerializeField]
    [Tooltip("The Input System Action that will be used to read Turn data from the left hand controller. Must be a Value Vector2 Control.")]
    InputActionProperty m_LeftHandTurnAction = new InputActionProperty(new InputAction("Left Hand Turn", expectedControlType: "Vector2"));
    /// <summary>
    /// The Input System Action that Unity uses to read Turn data from the left hand controller. Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.
    /// </summary>
    public InputActionProperty leftHandTurnAction
    {
        get => m_LeftHandTurnAction;
        set => SetInputActionProperty(ref m_LeftHandTurnAction, value);
    }

    [SerializeField]
    [Tooltip("The Input System Action that will be used to read Turn data from the right hand controller. Must be a Value Vector2 Control.")]
    InputActionProperty m_RightHandTurnAction = new InputActionProperty(new InputAction("Right Hand Turn", expectedControlType: "Vector2"));
    /// <summary>
    /// The Input System Action that Unity uses to read Turn data from the right hand controller. Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.
    /// </summary>
    public InputActionProperty rightHandTurnAction
    {
        get => m_RightHandTurnAction;
        set => SetInputActionProperty(ref m_RightHandTurnAction, value);
    }

    /// <summary>
    /// See <see cref="MonoBehaviour"/>.
    /// </summary>
    protected void OnEnable()
    {
        m_LeftHandTurnAction.EnableDirectAction();
        m_RightHandTurnAction.EnableDirectAction();
    }

    /// <summary>
    /// See <see cref="MonoBehaviour"/>.
    /// </summary>
    protected void OnDisable()
    {
        m_LeftHandTurnAction.DisableDirectAction();
        m_RightHandTurnAction.DisableDirectAction();
    }

    /// <inheritdoc />
    protected Vector2 ReadInput()
    {
        var leftHandValue = m_LeftHandTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
        var rightHandValue = m_RightHandTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero;

        return leftHandValue + rightHandValue;
    }

    void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
    {
        if (Application.isPlaying)
            property.DisableDirectAction();

        property = value;

        if (Application.isPlaying && isActiveAndEnabled)
            property.EnableDirectAction();
    }

    /// <summary>
    /// See <see cref="MonoBehaviour"/>.
    /// </summary>
    protected void Update()
    {
        m_IsTurningXROrigin = false;

        // Use the input amount to scale the turn speed.
        var input = ReadInput();
        var turnAmount = GetTurnAmount(input);
        var nodAmount = GetNodAmount(input);

        TurnRig(turnAmount);
        NodRig(nodAmount);

        switch (locomotionPhase)
        {
            case LocomotionPhase.Idle:
            case LocomotionPhase.Started:
                if (m_IsTurningXROrigin)
                    locomotionPhase = LocomotionPhase.Moving;
                break;
            case LocomotionPhase.Moving:
                if (!m_IsTurningXROrigin)
                    locomotionPhase = LocomotionPhase.Done;
                break;
            case LocomotionPhase.Done:
                locomotionPhase = m_IsTurningXROrigin ? LocomotionPhase.Moving : LocomotionPhase.Idle;
                break;
            default:
                Assert.IsTrue(false, $"Unhandled {nameof(LocomotionPhase)}={locomotionPhase}");
                break;
        }
    }

    /// <summary>
    /// Determines the turn amount in degrees for the given <paramref name="input"/> vector.
    /// </summary>
    /// <param name="input">Input vector, such as from a thumbstick.</param>
    /// <returns>Returns the turn amount in degrees for the given <paramref name="input"/> vector.</returns>
    protected virtual float GetTurnAmount(Vector2 input)
    {
        if (input == Vector2.zero)
            return 0f;

        var cardinal = CardinalUtility.GetNearestCardinal(input);
        switch (cardinal)
        {
            case Cardinal.North:
            case Cardinal.South:
                break;
            case Cardinal.East:
            case Cardinal.West:
                return input.magnitude * (Mathf.Sign(input.x) * m_TurnSpeed * Time.deltaTime);
            default:
                Assert.IsTrue(false, $"Unhandled {nameof(Cardinal)}={cardinal}");
                break;
        }

        return 0f;
    }

    /// <summary>
    /// Determines the turn amount in degrees for the given <paramref name="input"/> vector.
    /// </summary>
    /// <param name="input">Input vector, such as from a thumbstick.</param>
    /// <returns>Returns the turn amount in degrees for the given <paramref name="input"/> vector.</returns>
    protected virtual float GetNodAmount(Vector2 input)
    {
        if (input == Vector2.zero)
            return 0f;

        var cardinal = CardinalUtility.GetNearestCardinal(input);
        switch (cardinal)
        {
            case Cardinal.North:
            case Cardinal.South:
                return -input.magnitude * (Mathf.Sign(input.y) * m_TurnSpeed * Time.deltaTime);
            case Cardinal.East:
            case Cardinal.West:
                break;
            default:
                Assert.IsTrue(false, $"Unhandled {nameof(Cardinal)}={cardinal}");
                break;
        }

        return 0f;
    }

    /// <summary>
    /// Rotates the rig by <paramref name="turnAmount"/> degrees.
    /// </summary>
    /// <param name="turnAmount">The amount of rotation in degrees.</param>
    protected void TurnRig(float turnAmount)
    {
        if (Mathf.Approximately(turnAmount, 0f))
            return;

        if (CanBeginLocomotion() && BeginLocomotion())
        {
            var xrOrigin = (VRLook)system.xrOrigin;
            if (xrOrigin != null)
            {
                m_IsTurningXROrigin = true;
                xrOrigin.RotateAroundCameraPosition(Vector3.up, turnAmount);
            }

            EndLocomotion();
        }
    }

    protected void NodRig(float turnAmount)
    {
        if (Mathf.Approximately(turnAmount, 0f))
            return;

        if (CanBeginLocomotion() && BeginLocomotion())
        {
            var xrOrigin = (VRLook)system.xrOrigin;
            if (xrOrigin != null)
            {
                m_IsTurningXROrigin = true;
                xrOrigin.RotateAroundCameraUsingOriginRight(turnAmount);
            }

            EndLocomotion();
        }
    }
}