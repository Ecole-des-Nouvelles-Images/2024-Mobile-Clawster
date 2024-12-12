using UnityEngine;

namespace Local.Integration.Scripts.Game
{
    public class Water : MonoBehaviour
    {
       [SerializeField] private Transform _waterPlane;
        private bool _isInWater = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Head Player") && !_isInWater)
            {
                _isInWater = true;
                Debug.Log("Player entre dans l'eau !");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Head Player") && _isInWater)
            {
                _isInWater = false;
                Debug.Log("Player sort de l'eau !");
            }
        }
    }
}
