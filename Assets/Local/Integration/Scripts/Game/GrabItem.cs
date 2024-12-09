using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Local.Integration.Scripts.Game
{
    public class GrabItem : MonoBehaviour
    {
        [Header("Essential Settings")] public GameObject[] HandAim;
        public Button GrabButton;

        [Header("Stacked Objects (Not Implemented)")]
        public List<GameObject> Stack; 

        [Header("Animation")] public Animator HandAnimator;
        public float InterpolationTime;
        public bool IsGrabbing { get; private set; }
        [SerializeField] private float _despawnTime;

        private GameObject _hand; 
        private GameObject _hitObj; 
        private Item _hitItem;

        private void Start()
        {
            HandAnimator = GetComponentInParent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            GrabButton.gameObject.SetActive(true);
            _hitObj = other.gameObject;
            _hitItem = _hitObj.GetComponent<Item>();
            _hitItem.GetComponent<Renderer>().sharedMaterial.SetVector("_OutlineColor", Vector4.one);
        }

        private void OnTriggerExit(Collider other)
        {
            GrabButton.gameObject.SetActive(false);
            _hitObj = null;
            _hitItem.GetComponent<Renderer>().material
                .SetVector("_OutlineColor", new Vector4(0, 0, 0, 1));
        }

        public void Grab()
        {
            if (_hitObj.CompareTag("Item"))
            {
                IsGrabbing = true;
                HandAnimator.SetBool("IsGrabbing", true);

                float dist0 = Vector3.Distance(_hitObj.transform.position, HandAim[0].transform.position);
                float dist1 = Vector3.Distance(_hitObj.transform.position, HandAim[1].transform.position);

                _hand = (dist0 < dist1) ? HandAim[0] : HandAim[1];
                Debug.Log(_hand.name);

                StartCoroutine(GrabAnimate(InterpolationTime));
            }

            StackItem(_hitItem, Stack); 
            Destroy(_hitObj, _despawnTime); 
            IsGrabbing = false;
        }

        IEnumerator GrabAnimate(float dt)
        {
            for (dt = 0f; dt < InterpolationTime; dt += Time.deltaTime) yield return null; 
            HandAnimator.SetBool("IsGrabbing", false);
            IsGrabbing = false;
        }

        private void StackItem(Item i, List<GameObject> stack)
        {
            stack.Add(i.StackedCounterpart);
        }
    }
}