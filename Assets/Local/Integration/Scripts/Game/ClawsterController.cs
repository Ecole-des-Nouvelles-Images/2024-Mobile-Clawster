using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Local.Integration.Scripts.MainMenu;
using Local.Integration.Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Local.Integration.Scripts.Game
{
    public class ClawsterController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Joystick _joystick;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Collider _grabCollider;
        private Rigidbody _rigidbody;

        [Header("Movement Settings")]
        [SerializeField] private float _speed;
        [SerializeField] private float _sprintSpeedMultiplier = 1.5f;
        [SerializeField] private float _slowFactor;
        private float _currentSpeed;

        [Header("Stamina Settings")]
        [SerializeField] private float _maxStamina;
        [SerializeField] private float _speedRecover = 20f;
        private float _stamina;
        private bool _staminaExhausted;

        [Header("Health & Damage Settings")]
        [SerializeField] private int _maxHealth;

        [Header("Weight Settings")]
        [SerializeField] private float _weightMaxCapacity;
        private float _weightHold = 0;

        [Header("QTE Settings")]
        [SerializeField] private float _maxQteTime;
        [SerializeField] private int _maxTouchCount;

        [Header("UI Elements")]
        [SerializeField] private Image _weightFillImage;
        [SerializeField] private CanvasGroup _wholeWheelCanvasGroup;
        [SerializeField] private Image _redWheel;
        [SerializeField] private Image _greenWheel;
        [SerializeField] private CanvasGroup _grabButton;

        [Header("Sound Effects")]
        private AudioClip _grabSound;
        
        private GameObject _hitObj;
        private bool _canTakeDamage;
        private int _health;
        private int _holdScore;
        private float _targetAngle;
        private Animator _handAnimator;
        private bool _isAFish;
        private bool _isInQte;
        private int _currentTouchCount;
        private bool _isSprinting;
        private Dictionary<string, CollectedItemData> _collectedItems = new Dictionary<string, CollectedItemData>();
        private float _nextDamageTime = 0f;

        private bool CanGrab()
        {
            return _hitObj != null && !_isInQte;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _handAnimator = GetComponent<Animator>();
            HideWholeWheel();
        }

        private void Start()
        {
            _weightHold = 0;
            _stamina = _maxStamina;
            _health = _maxHealth;
            _weightFillImage.fillAmount = _weightHold / _weightMaxCapacity;
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
                if (_isInQte)
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

        private void Update()
        {
            if (!GameManager.instance.HasStarted) return;

            _grabButton.alpha = CanGrab() ? 1f : 0.5f;

            if (!_isSprinting && _stamina < _maxStamina)
            {
                _stamina +=  _speedRecover * Time.deltaTime;
                _stamina = Mathf.Clamp(_stamina, 0, _maxStamina);
                _redWheel.fillAmount = (_stamina / _maxStamina);
                _greenWheel.fillAmount = (_stamina / _maxStamina);

                if (_stamina >= _maxStamina)
                {
                    _greenWheel.enabled = true;
                    _staminaExhausted = false;
                    HideWholeWheel();
                }
            }
        }

        public void OnSprint()
        {
            if (!GameManager.instance.HasStarted) return;
            _isSprinting = true;
            ShowWholeWheel();
        }

        public void OnUnsprint()
        {
            if (!GameManager.instance.HasStarted) return;
            _isSprinting = false;
        }

        public void HandleStamina()
        {
            if (_isSprinting && !_staminaExhausted)
            {
                if (_stamina > 0)
                {
                    _stamina -= 30 * Time.deltaTime;
                    _stamina = Mathf.Clamp(_stamina, 0, _maxStamina);
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
                    _stamina += 10 * Time.deltaTime;
                    _stamina = Mathf.Clamp(_stamina, 0, _maxStamina);
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
            Vector3 cameraForward = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;
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

            if (_weightHold > 0)
            {
                currentSpeed *= (1 - (_weightHold / _weightMaxCapacity) * _slowFactor);
            }

            if (_isSprinting && !_staminaExhausted)
            {
                currentSpeed *= _sprintSpeedMultiplier;
                HandleStamina();
            }
            else
            {
                _isSprinting = false;
            }

            Vector3 move = moveDirection * currentSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(_rigidbody.position + move);
        }

        private void HandleDamage()
        {
            if (_health == 1)
            {
                GameManager.instance.GameOver();
                return;
            }

            _health--;
            GameObject heart = _healthBar.HeartIcons[_health].gameObject;
            heart.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBounce);
            _nextDamageTime = Time.time + 0.5f;
        }

        private void Grab()
        {
            if (_hitObj != null && _hitObj.CompareTag("Item"))
            {
                ItemStats itemStats = _hitObj.GetComponent<ItemStats>();
                if (itemStats != null && itemStats.Item != null)
                {
                    string itemName = itemStats.Item.Name;
                    int itemWeight = itemStats.Item.Weight;
                    int itemScore = itemStats.Item.Score;

                    if (_weightHold + itemWeight <= _weightMaxCapacity)
                    {
                        _weightHold += itemWeight;
                        _holdScore += itemScore;

                        if (_collectedItems.ContainsKey(itemName))
                        {
                            _collectedItems[itemName].Quantity++;
                        }
                        else
                        {
                            _collectedItems[itemName] = new CollectedItemData(itemName, itemWeight, itemScore);
                        }

                        SoundFXManager.instance.PlaySoundFXClip(_grabSound, transform, 1f);
                        float targetFillAmount = _weightHold / _weightMaxCapacity;
                        _weightFillImage.DOFillAmount(targetFillAmount, 0.5f).SetEase(Ease.InOutQuad);
                        _hitObj.SetActive(false);
                        _hitObj = null;
                    }

                    if (_weightHold + itemWeight >= _weightMaxCapacity)
                    {
                        if (_hitObj != null) GameManager.instance.ShowFloatingText(_hitObj.transform.position, 1f);
                    }
                }
            }
        }

        private IEnumerator QTEGrab()
        {
            _isInQte = true;
            for (float dt = 0f; dt < _maxQteTime; dt += Time.deltaTime)
            {
                if (_currentTouchCount >= _maxTouchCount)
                {
                    ItemStats itemStats = _hitObj.GetComponent<ItemStats>();
                    if (itemStats != null && itemStats.Item != null)
                    {
                        string itemName = itemStats.Item.Name;
                        int itemWeight = itemStats.Item.Weight;
                        int itemScore = itemStats.Item.Score;

                        if (_weightHold + itemWeight <= _weightMaxCapacity)
                        {
                            _weightHold += itemWeight;
                            _holdScore += itemScore;

                            if (_collectedItems.ContainsKey(itemName))
                            {
                                _collectedItems[itemName].Quantity++;
                            }
                            else
                            {
                                _collectedItems[itemName] = new CollectedItemData(itemName, itemWeight, itemScore);
                            }

                            float targetFillAmount = _weightHold / _weightMaxCapacity;
                            _weightFillImage.DOFillAmount(targetFillAmount, 0.5f).SetEase(Ease.InOutQuad);
                            _hitObj.SetActive(false);
                            _hitObj = null;
                            _isInQte = false;
                            _isAFish = false;
                            yield break;
                        }
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            _isInQte = false;
        }

        public void ValidateScore()
        {
            GameManager.instance.AddScore(_holdScore);
            GameManager.instance.DisplayCollectedItems(_collectedItems);
            _holdScore = 0;
            _weightHold = 0;
            _collectedItems.Clear();
            _weightFillImage.DOFillAmount(0, 0.2f).SetEase(Ease.InOutQuad);
            GameManager.instance.UpdateScoreUI();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                _hitObj = other.gameObject;
                other.GetComponent<Renderer>().material.SetVector("_OutlineColor", Vector4.one);
                _isAFish = false;
                _isInQte = false;
            }
            if (other.CompareTag("Fish"))
            {
                StopAllCoroutines();
                _hitObj = other.gameObject;
                other.GetComponent<Renderer>().material.SetVector("_OutlineColor", Vector4.one);
                _isAFish = true;
                _isInQte = false;
            }
            if (other.CompareTag("Enemy"))
            {
                if (Time.time >= _nextDamageTime)
                {
                    HandleDamage();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Item") || other.CompareTag("Fish"))
            {
                StopAllCoroutines();
                other.GetComponent<Renderer>().material.SetVector("_OutlineColor", new Vector4(0, 0, 0, 1));
                _hitObj = null;
                _isAFish = false;
                _currentTouchCount = 0;
            }
        }

        private void ShowWholeWheel()
        {
            _wholeWheelCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.InOutQuad);
        }

        private void HideWholeWheel()
        {
            _wholeWheelCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InOutQuad);
        }
    }
}

public class CollectedItemData
{
    public string Name { get; set; }
    public int Weight { get; set; }
    public int Score { get; set; }
    public int Quantity { get; set; }

    public CollectedItemData(string name, int weight, int score, int quantity = 1)
    {
        Name = name;
        Weight = weight;
        Score = score;
        Quantity = quantity;
    }
}
