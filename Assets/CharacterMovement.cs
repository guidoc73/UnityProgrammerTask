using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _sprintSpeed = 5f;
    
    private CharacterController _characterController;
    private InputAction _moveAction;
    private InputAction _sprintAction;

    private void Awake()
    {
        _sprintAction = InputSystem.actions.FindAction("Sprint");
        _movementSpeed = _walkSpeed;
    }

    private void Start()
    {
        GetCharacterControllerComponent();
        GetMovementAction();
    }

    private void Update()
    {    
        Move();
    }

    private void GetCharacterControllerComponent()
    {
        if(TryGetComponent(out CharacterController component))
            _characterController = component;
    }

    private void GetMovementAction()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Move()
    {
        var moveValue = _moveAction.ReadValue<Vector2>() * (Time.deltaTime * _movementSpeed);
        
        _characterController.Move(new Vector3(moveValue.x, 0, moveValue.y));
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
