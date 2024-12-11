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
        [SerializeField] private ItemGrab _itemGrab;
        [SerializeField] private Transform cameraTransform; // Référence à la caméra

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
        private bool isMoving = false;
        private float _sf;

        private void Start()
        {
            _stamina = _maxStamina;
            _sf = 1f;
        }

        private void Update()
        {
            if (!GameManager.instance.HasStarted) return;

            Vector3 direction = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical).normalized;
            isMoving = direction.magnitude >= 0.1f;

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

        private void FixedUpdate() {
            
            if (!GameManager.instance.HasStarted) return;

            if (isMoving && !_itemGrab.IsGrabbing)
            {
                Vector3 inputDirection = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical).normalized;

                Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

                Vector3 moveDirection = inputDirection.z * cameraForward + inputDirection.x * cameraRight;
                moveDirection.Normalize();

                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
                {
                    moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal).normalized;
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

                Vector3 move = moveDirection * (currentSpeed / _sf) * Time.fixedDeltaTime;
                _rigidbody.MovePosition(_rigidbody.position + move);
            }
            else if (!isMoving)
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }

        private void OnTriggerStay(Collider water) {

            if (water.gameObject.CompareTag("Water")) {
                _sf = _slowFactor;
            }
            else {
                return;
            }
        }

        private void OnTriggerExit(Collider other) {
            _sf = 1f;
        }
    }
}
