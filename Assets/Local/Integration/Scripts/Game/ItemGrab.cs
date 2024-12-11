using UnityEngine;
using UnityEngine.UI;

namespace Local.Integration.Scripts.Game
{
    public class ItemGrab : MonoBehaviour
    {
        public GameObject[] HandAim;
        public Collider GrabCollider;
        public Animator HandAnimator;  // Animator pour les mains (ou le crab)

        private GameObject _hitObj;
        private Item _hitItem;

        public void Grab()
        {
            if (_hitObj != null && _hitObj.CompareTag("Item"))
            {
                HandAnimator.SetTrigger("Grab");
                _hitObj.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                _hitObj = other.gameObject;
                _hitItem = _hitObj.GetComponent<Item>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                _hitObj = null;
                _hitItem = null;
            }
        }
    }
}