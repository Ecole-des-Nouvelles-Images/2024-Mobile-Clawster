using System;
using Joystick_Pack.Scripts.Joysticks;
using UnityEngine;
using UnityEngine.UI;

namespace Local.Integration.Scripts.Game
{
    public class ClawsterController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Joystick _joystick;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Collider _grabCollider;
        
        [Header("Movement Settings")]
        [SerializeField] private float _speed;
        [SerializeField] private float _sprintSpeedMultiplier = 1.5f;
        [SerializeField] private float _speedCap;
        [SerializeField] private float _slowFactor;

        [Header("Stamina Settings")]
        [SerializeField] private float _maxStamina;
        private float _stamina;
        private bool _staminaExhausted;

        [Header("UI Elements")]
        [SerializeField] private Image _redWheel;
        [SerializeField] private Image _greenWheel;

        private float _targetAngle;
        private GameObject _hitObj;
        private Animator _handAnimator;


        private void Awake()
        {
            _handAnimator = GetComponent<Animator>();
        }


        private void Start()
        {
            _stamina = _maxStamina;
        }

        private void Update()
        {
            if (!GameManager.instance.HasStarted) return;

            HandleInput();
            HandleStamina();
        }

        private void FixedUpdate()
        {
            if (!GameManager.instance.HasStarted) return;

            HandleMovement();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HandleGrab();
                _handAnimator.SetTrigger("Grab");

            }
        }

        private void HandleStamina()
        {
            if (Input.GetKey(KeyCode.Space) && !_staminaExhausted)
            {
                if (_stamina > 0)
                {
                    _stamina -= 30 * Time.deltaTime;
                }
                else
                {
                    _greenWheel.enabled = false;
                    _staminaExhausted = true;
                }

                _redWheel.fillAmount = (_stamina / _maxStamina + 0.07f);
            }
            else
            {
                if (_stamina < _maxStamina)
                {
                    _stamina += 30 * Time.deltaTime;
                }
                else
                {
                    _greenWheel.enabled = true;
                    _staminaExhausted = false;
                }

                _redWheel.fillAmount = (_stamina / _maxStamina);
            }

            _greenWheel.fillAmount = (_stamina / _maxStamina);
        }

        private void HandleMovement()
        {
            Vector3 inputDirection = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical).normalized;

            Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

            Vector3 moveDirection = inputDirection.z * cameraForward + inputDirection.x * cameraRight;
            moveDirection.Normalize();

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    return; 
                }
            }

            if (moveDirection.magnitude > 0.1f)
            {
                _targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, _targetAngle, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
            }

            float currentSpeed = _speed;
            if (Input.GetKey(KeyCode.Space) && !_staminaExhausted)
            {
                currentSpeed *= _sprintSpeedMultiplier;
            }

            Vector3 move = moveDirection * currentSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(_rigidbody.position + move);
        }
        
        private void HandleGrab()
        {
            if (_hitObj != null && _hitObj.CompareTag("Item"))
            {
                _handAnimator.SetTrigger("Grab");
                _hitObj.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                _hitObj = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {

            if (other.CompareTag("Item"))
            {
                _hitObj = null;
            }
        }
    }
}
