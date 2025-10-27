using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [InspectorName("Movement")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _sprintSpeed = 5f;
    [SerializeField] private float _speedChangeRate = 10.0f;
    private InputAction _moveAction;
    private InputAction _sprintAction;
    
    [InspectorName("Rotation")]
    [SerializeField] private float _rotationSmoothTime = 0.12f;
    [SerializeField] private float _rotationVelocity = 10;
    private float _targetRotation;
    
    [InspectorName("SFX")]
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    
    private CharacterController _characterController;
    private GameObject _mainCamera;
    private Animator _animator;
    private float _animationBlend;
    private int _animIDSpeed;

    private void Awake()
    {
        GetMainCamera();
        GetInputActions();
        InitialAnimationConfig();
        InitialMovementConfig();
    }

    private void Start()
    {
        GetCharacterControllerComponent();
    }

    private void Update()
    {    
        Move();
    }

    private void InitialAnimationConfig()
    {
        TryGetComponent(out _animator);
        _animIDSpeed = Animator.StringToHash("Speed");
    }

    private void InitialMovementConfig()
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
        if (_characterController == null || _moveAction == null) 
            return;
        
        var normalizedMovingInputValue = _moveAction.ReadValue<Vector2>().normalized;
        var direction = normalizedMovingInputValue.normalized;
        
        var movement = new Vector3(direction.x, 0, direction.y) * (_movementSpeed * Time.deltaTime);
        
        _characterController.Move(movement);

        var isMoving = IsMoving(movement);
        
        if (isMoving)
        {
            RotateByMovingDirection(direction);
        }
        
        AnimateMovement(isMoving);
    }

    private void AnimateMovement(bool isMoving)
    {
        _animationBlend = Mathf.Lerp(_animationBlend, isMoving? _movementSpeed : 0, Time.deltaTime * _speedChangeRate);
        if (_animationBlend < 0.01f)
        {
            _animationBlend = 0f;
        }
        _animator.SetFloat(_animIDSpeed, _animationBlend);
    }

    private  bool IsMoving(Vector3 moveValue)
    {
        return moveValue != Vector3.zero;
    }

    private void RotateByMovingDirection(Vector2 direction)
    {
        _targetRotation = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
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
    
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_characterController.center), FootstepAudioVolume);
            }
        }
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
