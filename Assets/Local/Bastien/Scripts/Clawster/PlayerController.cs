using Joystick_Pack.Scripts.Joysticks;
using UnityEngine;

namespace Local.Bastien.Scripts.Clawster
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rbc; //RigidBody entity to move Clawster 
        [SerializeField] private ItemGrab _itemGrab; //Grab animation control
        [SerializeField] private DynamicJoystick _dynamicJoystick; // Référence au joystick dynamique

        public float Speed; // Vitesse de déplacement
        public float SpeedCap; // Maximum velocity
        public float SlowFactor; // Réduction de vitesse dans l'eau

        private Vector2 _movement; // Mouvement X et Z
        private float _targetAngle; // Angle de rotation

        private void Update()
        {
            // Récupère les valeurs du joystick
            _movement = new Vector2(_dynamicJoystick.Horizontal, _dynamicJoystick.Vertical);

            // Convertit les valeurs du joystick en une direction normalisée
            Vector3 direction = new Vector3(_movement.x, 0f, _movement.y).normalized;

            // Si une direction est donnée et qu'on ne dépasse pas la vitesse limite
            if ((direction.magnitude >= 0.1f && _rbc.velocity.magnitude < SpeedCap) && (!_itemGrab.IsGrabbing))
            {
                // Calcule l'angle et applique une rotation
                _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.LookRotation(direction, transform.up);

                // Ajoute une force au Rigidbody pour déplacer le joueur
                _rbc.AddForce(direction * (Speed * Time.deltaTime), ForceMode.Acceleration);
            }
        }

        void OnGUI()
        {
            Debug.DrawRay(transform.position, transform.forward * 1000, Color.yellow);
        }
    }
}