using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [InspectorName("Movement")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _sprintSpeed = 5f;
    private InputAction _moveAction;
    private InputAction _sprintAction;
    
    [InspectorName("Rotation")]
    [SerializeField] private float _rotationSmoothTime = 0.12f;
    [SerializeField] private float _rotationVelocity = 10;
    private float _targetRotation;
    
    private CharacterController _characterController;
    private GameObject _mainCamera;


    private void Awake()
    {
        GetMainCamera();
        GetInputActions();
        SetInitialConfig();
    }

    private void Start()
    {
        GetCharacterControllerComponent();
    }

    private void Update()
    {    
        Move();
    }

    private void SetInitialConfig()
    {
        _movementSpeed = _walkSpeed;
    }

    private void GetCharacterControllerComponent()
    {
        if(TryGetComponent(out CharacterController component))
            _characterController = component;
    }

    private void GetInputActions()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    private void Move()
    {
        var moveValue = _moveAction.ReadValue<Vector2>() * (Time.deltaTime * _movementSpeed);
        
        _characterController.Move(new Vector3(moveValue.x, 0, moveValue.y));

        if (IsMoving(moveValue))
        {
            RotateByMovingDirection(moveValue);
        }
    }

    private  bool IsMoving(Vector2 moveValue)
    {
        return moveValue != Vector2.zero;
    }

    private void RotateByMovingDirection(Vector2 moveValue)
    {
        _targetRotation = Mathf.Atan2(moveValue.x, moveValue.y) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
        var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
            _rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    private void GetMainCamera()
    {
        if (_mainCamera == null && Camera.main != null)
        {
            _mainCamera = Camera.main.gameObject;
        }
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        _movementSpeed = _sprintSpeed;
    }
    
    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _movementSpeed = _walkSpeed;
    }
    
    private void OnEnable()
    {
        _sprintAction.started += OnSprintStarted;
        _sprintAction.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        _sprintAction.started -= OnSprintStarted;
        _sprintAction.canceled -= OnSprintCanceled;
    }
}
