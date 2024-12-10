using Joystick_Pack.Scripts.Joysticks;
using UnityEngine;

namespace Local.Integration.Scripts.Game
{
    public class ClawsterController : MonoBehaviour {
        
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private DynamicJoystick _dynamicJoystick; 
        [SerializeField] private ItemGrab _itemGrab;

        public float Speed;
        public float SpeedCap; 
        public float SlowFactor; 
        
        private Vector3 _velocity; 
        private float _targetAngle;

        private Ray _ray;
        private RaycastHit _hit; 
        private GameObject _grabTarget; 
    
        private void Update() {
            if (!GameManager.instance.HasStarted) return;

            Vector3 direction = new Vector3(_dynamicJoystick.Horizontal, 0f, _dynamicJoystick.Vertical).normalized;

            if ((direction.magnitude >= 0.1f && _rigidbody.velocity.magnitude < SpeedCap) && (!_itemGrab.IsGrabbing)) {
                _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                Vector3 moveDir = (Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward).normalized;
                transform.rotation = Quaternion.LookRotation(direction, transform.up);
                _rigidbody.AddForce(moveDir * ((Speed / SlowFactor + 1) * Time.deltaTime), ForceMode.Force);
            }
        }
    }
}