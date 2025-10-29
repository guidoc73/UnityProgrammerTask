using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Player.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deceleration;
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _sprintSpeed;
        
        private CharacterController _characterController;
        
        private InputAction _moveAction;
        private InputAction _sprintAction;
        private float _targetSpeed;
        private float _resultSpeed;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _moveAction = InputSystem.actions.FindAction("Move");
            _targetSpeed = _walkSpeed;
            _sprintAction = InputSystem.actions.FindAction("Sprint");
            _sprintAction.started += OnSprintPressed;
            _sprintAction.canceled += OnSprintReleased;
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            var moveValue = _moveAction.ReadValue<Vector2>();
            var direction = moveValue.normalized;
            
            if (moveValue != Vector2.zero)
            {
                _resultSpeed += _acceleration * Time.deltaTime;
                transform.forward = direction.ToFixedVector3(); //TODO: hacer que la rotación sea fluida
            }
            else
            {
                _resultSpeed -= _deceleration * Time.deltaTime;
            }
            
            _resultSpeed = Mathf.Clamp(_resultSpeed, 0, _targetSpeed);
            
            var movement = transform.forward * (_resultSpeed * Time.deltaTime);

            _characterController.Move(movement);
        }
        
        private void OnSprintPressed(InputAction.CallbackContext context)
        {
            _targetSpeed = _sprintSpeed;
        }
        
        private void OnSprintReleased(InputAction.CallbackContext context)
        {
            _targetSpeed = _walkSpeed;
        }

        private void OnDestroy()
        {
            _sprintAction.started -= OnSprintPressed;
            _sprintAction.canceled -= OnSprintReleased;
        }

        #if UNITY_EDITOR
        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 200, 20), "Velocidad: " + _resultSpeed.ToString("F1"));
        }
        #endif
    }
}
