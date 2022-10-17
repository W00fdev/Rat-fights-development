using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("���� �����")]
    [SerializeField]
    private Camera _isometricCamera;
    [SerializeField]
    private Camera _thirdPersonCamera;

    [Header("��������� ���������")]
    [SerializeField][Tooltip("�������� ����������� ���������")]
    private float _axisMovementVelocity;
    [SerializeField][Tooltip("�������� ����������� �����")]
    private float _mouseMovementVelocity;
    [SerializeField][Tooltip("�������� ����������� / ���������")]
    private float _scrollingVelocity;

    [Header("�������� �����������")]
    [SerializeField]
    private Vector2 _fieldOfViewBand;

    private CursorFreezer _freezer;
    private float FovFactor => _isometricCamera.fieldOfView / _fieldOfViewBand.y;

    private bool IsIsometricEnabled
    {
        get
        {
            if (_thirdPersonCamera != null)
                if (Input.GetKeyUp(KeyCode.C))
                {
                    _thirdPersonCamera.enabled = _isometricCamera.enabled;
                    _isometricCamera.enabled = !_isometricCamera.enabled;
                }

            return _isometricCamera.enabled;
        }
    }

    private bool IsCursorFreezed => _freezer != null && (_freezer.enabled = Input.GetKey(KeyCode.Mouse2));
    private bool IsCursorOffScreen => Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width ||
                                      Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height;

    private void Start() => _freezer = gameObject.AddComponent<CursorFreezer>();

    private void Update()
    {
        if (IsIsometricEnabled)
            IsometricUpdate();
    }

    private void IsometricUpdate()
    {
        // ����������� ���������
        Move(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 
             _axisMovementVelocity);

        // ����������� �����
        if (IsCursorOffScreen)
            Move(Input.mousePosition.x - Screen.width / 2f, Input.mousePosition.y - Screen.height / 2f,
                 _mouseMovementVelocity);
        
        // ����������� ���������
        if (IsCursorFreezed)
            Move(_freezer.Drag.x, _freezer.Drag.y,
                 _mouseMovementVelocity);

        // �����������
        if (Input.mouseScrollDelta.y != 0)
            Zoom(Input.mouseScrollDelta.y,
                 _scrollingVelocity);
    }

    private void Move(float x, float y, float velocity) =>
        _isometricCamera.transform.position += velocity * Time.deltaTime * FovFactor * Vector3.ClampMagnitude(new Vector3(x, 0, y), 1);

    private void Zoom(float value, float velocity) => 
        _isometricCamera.fieldOfView = Mathf.Clamp(_isometricCamera.fieldOfView - velocity * Time.deltaTime * Mathf.Clamp(value, -1f, 1f), _fieldOfViewBand.x, _fieldOfViewBand.y);
}
