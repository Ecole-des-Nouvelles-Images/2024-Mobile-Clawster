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
        [SerializeField] private DynamicJoystick _dynamicJoystick;
        [SerializeField] private ItemGrab _itemGrab;
        

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

        private Vector3 _velocity;
        private float _targetAngle;
        private Ray _ray;
        private RaycastHit _hit;
        private GameObject _grabTarget;
        private bool isMoving = false;

        private void Start()
        {
            _stamina = _maxStamina;
        }

        private void Update()
        {
            Debug.Log("no started");
            if (!GameManager.instance.HasStarted) return;
            Debug.Log("go");

            Vector3 direction = new Vector3(_dynamicJoystick.Horizontal, 0f, _dynamicJoystick.Vertical).normalized;
            isMoving = direction.magnitude >= 0.1f;
            _rigidbody.useGravity = isMoving;

            if (Input.GetKey(KeyCode.Space) && !_staminaExhausted)
            {
                Debug.Log("space");

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

        private void FixedUpdate()
        {
            if (!GameManager.instance.HasStarted) return;

            if (isMoving && _rigidbody.velocity.magnitude < _speedCap && !_itemGrab.IsGrabbing)
            {
                Vector3 direction = new Vector3(_dynamicJoystick.Horizontal, 0f, _dynamicJoystick.Vertical).normalized;
                _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                Vector3 moveDir = (Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward).normalized;
                transform.rotation = Quaternion.LookRotation(direction, transform.up);

                float currentSpeed = _speed;
                if (Input.GetKey(KeyCode.Space) && !_staminaExhausted)
                {
                    currentSpeed *= _sprintSpeedMultiplier;
                }

                _rigidbody.AddForce(moveDir * ((currentSpeed / _slowFactor + 1) * Time.fixedDeltaTime), ForceMode.Force);
            }
            else if (!isMoving)
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }
    }
}
