using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
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
        [SerializeField] private float _slowFactor;

        [Header("Stamina Settings")]
        [SerializeField] private float _maxStamina;
        private float _stamina;
        private bool _staminaExhausted;
        
        [Header("Weight Settings")]
        [SerializeField] private float _weightMaxCapacity;
        [SerializeField] private Image _weightFillImage;
        private float _weightHold = 0;

        [Header("QTE Settings")] 
        [SerializeField] private float _maxQTETime;
        [SerializeField] private int _maxTouchCount;

        [Header("UI Elements")]
        [SerializeField] private Image _redWheel;
        [SerializeField] private Image _greenWheel;

        private float _targetAngle;
        private GameObject _hitObj;
        private Animator _handAnimator;
        private bool _isAFish;              //Self-explanatory
        private bool _isInQTE;              //Is the QTE running ?
        private int _currentTouchCount;


        private void Awake()
        {
            _handAnimator = GetComponent<Animator>();
        }
        
        private void Start()
        {
            _stamina = _maxStamina;
            _weightFillImage.fillAmount = _weightHold;
        }

        private void Test()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                HandleSprint();
            }
        }


        private void FixedUpdate()
        {
            if (!GameManager.instance.HasStarted) return;

            HandleMovement();
        }

        [UsedImplicitly]
        public void HandleGrab()
        {
            if (!GameManager.instance.HasStarted) return;

            if (_isAFish)
            {
                if (_isInQTE)
                {
                    _currentTouchCount++;
                }
                else
                {
                    _currentTouchCount = 0;
                    StartCoroutine(QTEGrab());
                }
            }
            else
            {
                Grab();
                _handAnimator.SetTrigger("Grab");
            }
            
        }

        public void HandleSprint()
        {
            if (!_staminaExhausted)
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
        
        private void Grab()
        {
            if (_hitObj != null && _hitObj.CompareTag("Item"))
            {
                ItemStats itemStats = _hitObj.GetComponent<ItemStats>();
                if (itemStats != null && itemStats.Item != null)
                {
                    float itemWeight = itemStats.Item.Weight;

                    if (_weightHold + itemWeight <= _weightMaxCapacity)
                    {
                        _weightHold += itemWeight;

                        float targetFillAmount = _weightHold / _weightMaxCapacity;
                        _weightFillImage.DOFillAmount(targetFillAmount, 0.5f).SetEase(Ease.InOutQuad);

                        Debug.Log($"Poids ajouté : {itemWeight}, Poids total : {_weightHold}/{_weightMaxCapacity}");

                        _hitObj.SetActive(false);
                        _hitObj = null;
                    }
                }
            }
        }

        private IEnumerator QTEGrab()
        {
            _isInQTE = true;
            
            for(float dt = 0f; dt < _maxQTETime; dt += Time.deltaTime)
            {
                if (_currentTouchCount >= _maxTouchCount)
                {
                    Debug.Log("QTE Successful!");
                    ItemStats itemStats = _hitObj.GetComponent<ItemStats>();
                    if (itemStats != null && itemStats.Item != null)
                    {
                        float itemWeight = itemStats.Item.Weight;

                        if (_weightHold + itemWeight <= _weightMaxCapacity)
                        {
                            _weightHold += itemWeight;

                            float targetFillAmount = _weightHold / _weightMaxCapacity;
                            _weightFillImage.DOFillAmount(targetFillAmount, 0.5f).SetEase(Ease.InOutQuad);

                            Debug.Log($"Poids ajouté : {itemWeight}, Poids total : {_weightHold}/{_weightMaxCapacity}");

                            _hitObj.SetActive(false);
                            _hitObj = null;
                        }
                    }
                    yield break;
                }
                yield return new WaitForEndOfFrame();
            }

            if (_currentTouchCount < _maxTouchCount)
            {
                Debug.Log("QTE Failed");
            }
            _isInQTE = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                _hitObj = other.gameObject;
                other.GetComponent<Renderer>().material.SetVector("_OutlineColor", Vector4.one);
                _isAFish = false;
            }

            if (other.CompareTag("Fish"))
            {
                _hitObj = other.gameObject;
                other.GetComponent<Renderer>().material.SetVector("_OutlineColor", Vector4.one); 
                _isAFish = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Item") || other.CompareTag("Fish"))
            {
                _hitObj = null;
                other.GetComponent<Renderer>().material.SetVector("_OutlineColor", new Vector4(0, 0, 0, 1));
                _isAFish = false;
                _currentTouchCount = 0;
            }
        }
    }
}
