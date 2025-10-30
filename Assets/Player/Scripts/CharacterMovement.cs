using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Player.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        private const float IDLE_SPEED = 0;
        private const float DAMP_TIME = 0.1f;
        private const string SPEED_ANIMATOR_PARAMETER = "Speed";

        [Header("Movement Settings")]
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deceleration;
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _sprintSpeed;
        
        [Header("Rotation Settings")]
        [SerializeField] private float _rotationSpeed;
        
        private CharacterController _characterController;
        private Animator _animator;
        
        private InputAction _moveAction;
        private InputAction _sprintAction;
        private float _resultSpeed;
        private float _targetSpeed;
        private float _maxSpeed;
        
        private Vector3 _lastPosition;
        private float _currentSpeed;
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _moveAction = InputSystem.actions.FindAction("Move");
            _sprintAction = InputSystem.actions.FindAction("Sprint");
            _targetSpeed = _walkSpeed;
            _lastPosition = transform.position;
        }
        
        private void Update()
        {
            Move();
            SetSpeedParameterOnAnimator();

        }

        private void Move()
        {
            var moveValue = _moveAction.ReadValue<Vector2>();
            var direction = moveValue.normalized;
            
            if (moveValue != Vector2.zero)
            {
                _targetSpeed = _sprintAction.IsPressed() ? _sprintSpeed : _walkSpeed;

                transform.forward = GetSmoothRotation(direction); //TODO: hacer que la rotación sea fluida
            }
            else
            {
                _targetSpeed = IDLE_SPEED;
            }
            
            var rate = _targetSpeed > _resultSpeed ? _acceleration : _deceleration;
            _resultSpeed = Mathf.MoveTowards(_resultSpeed, _targetSpeed, rate * Time.deltaTime);
            
            var movement = transform.forward * (_resultSpeed * Time.deltaTime);

            _characterController.Move(movement);
        }
        
        private void SetSpeedParameterOnAnimator()
        {
            _currentSpeed = _characterController.velocity.magnitude;
            _lastPosition = transform.position;

            var normalizedSpeed = 0f;

            if (_currentSpeed > 0f)
            {
                if (_currentSpeed < _walkSpeed)
                    normalizedSpeed = Mathf.InverseLerp(0f, _walkSpeed, _currentSpeed) * 0.5f;
                else
                    normalizedSpeed = 0.5f + Mathf.InverseLerp(_walkSpeed, _sprintSpeed, _currentSpeed) * 0.5f;
            }

            _animator.SetFloat(SPEED_ANIMATOR_PARAMETER, normalizedSpeed, DAMP_TIME, Time.deltaTime); 
        }

        private Vector3 GetSmoothRotation(Vector2 direction)
        {
            var targetDirection = direction.ToFixedVector3();
            
            var resultDirection = Vector3.Lerp(transform.forward, targetDirection, _rotationSpeed * Time.deltaTime);
            return resultDirection;
        }

        #if UNITY_EDITOR
        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 200, 20), "Velocidad: " + _resultSpeed.ToString("F1"));
        }
        #endif
    }
}
