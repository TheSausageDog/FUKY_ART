using UnityEngine;

[DisallowMultipleComponent]
public class ObjectOrientationController : MonoBehaviour
{
    [Header("��ת���Ʋ���")]
    [Tooltip("������ת�ٶ�")]
    public float normalRotationSpeed = 3f;
    [Tooltip("����ģʽ��ת�ٶ�")]
    public float adjustRotationSpeed = 1.5f;
    [Tooltip("��תƽ������ϵ��")]
    public float rotationSmoothing = 5f;

    [Header("״ָ̬ʾ")]
    [SerializeField] private bool _isAdjusting;  // Serialized for debug

    // ��ת��׼ֵ�͵���ֵ
    private Quaternion _baseRotation;
    private Quaternion _adjustmentRotation;

    void Start()
    {
        InitializeRotation();
    }

    void Update()
    {
        HandleRotationInput();
        ApplySmoothRotation();
    }

    // ��ʼ����ת��׼
    void InitializeRotation()
    {
        _baseRotation = transform.rotation;
        _adjustmentRotation = Quaternion.identity;
    }

    // ���������߼�
    void HandleRotationInput()
    {
        // ����������ʱ��ʼ����
        if (Input.GetMouseButtonDown(0))
        {
            StartAdjustment();
        }

        // �������ɿ�ʱ��������
        if (Input.GetMouseButtonUp(0))
        {
            EndAdjustment();
        }

        // ����������ת����
        ProcessRotation();
    }

    // ��ʼ����ģʽ
    void StartAdjustment()
    {
        _isAdjusting = true;
        _adjustmentRotation = transform.rotation;
    }

    // ��������ģʽ
    void EndAdjustment()
    {
        _isAdjusting = false;
        _baseRotation = transform.rotation;
    }

    // ������ת����
    void ProcessRotation()
    {
        // ��ȡ�������
        Vector2 mouseDelta = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );

        // ����ģʽѡ����ת�ٶ�
        float speed = _isAdjusting ? adjustRotationSpeed : normalRotationSpeed;

        // ������ת����Y��ˮƽ��X�ᴹֱ��
        Quaternion yaw = Quaternion.AngleAxis(mouseDelta.x * speed, Vector3.up);
        Quaternion pitch = Quaternion.AngleAxis(-mouseDelta.y * speed, Vector3.right);

        // Ӧ����ת
        if (_isAdjusting)
        {
            _adjustmentRotation = yaw * pitch * _adjustmentRotation;
        }
        else
        {
            _baseRotation = yaw * pitch * _baseRotation;
        }
    }

    // Ӧ��ƽ����ת
    void ApplySmoothRotation()
    {
        // ����ģʽѡ��Ŀ����ת
        Quaternion targetRotation = _isAdjusting ?
            _adjustmentRotation :
            _baseRotation;

        // ʹ�������ֵƽ������
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSmoothing
        );
    }

    // ���������ù���
    public void ResetOrientation()
    {
        InitializeRotation();
        transform.rotation = _baseRotation;
    }
}