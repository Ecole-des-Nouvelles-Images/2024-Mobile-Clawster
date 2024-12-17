using System.Collections;
using Local.Integration.Scripts.MainMenu;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Local.Integration.Scripts.Game
{
    public class LegRaycast : MonoBehaviour {
        [Header("GameObjects")]
        public Transform IKTarget;                
    
        [Header("Main Settings")]
        public float MaxDistance;                 
        public float InterpolationTime;            
        public float LegRaise;             
        
        [Header("Fine tuning")] 
        [SerializeField] private Vector3 _overshootFac;
        [SerializeField] private AnimationCurve _interpolationCurve;

        [Header("Graphics")]
        [SerializeField] private ParticleSystem _stepParticles;

        [Header("Sound")] 
        [SerializeField] private AudioClip _step;
    
        private Vector3 _IKPos;                    
        private Vector3 _currentHitPos;  
        private RaycastHit _hit;  
    
        void Update() {
            if (Physics.Raycast(transform.position, Vector3.down, out _hit, Mathf.Infinity)) {
                _currentHitPos = _hit.point;
            }
        
            if (Vector3.Distance(IKTarget.position, _currentHitPos) > MaxDistance) {
                StartCoroutine(Walk(InterpolationTime, _IKPos, _currentHitPos)); 
            } else {
                IKTarget.position = _IKPos;                  
            }
        }

        IEnumerator WalkInterpolate(float dt, Vector3 start, Vector3 end) {
            Vector3 tempPos;
            float pi = Mathf.PI;
            float dtNorm;



            for (dt = 0f; dt < InterpolationTime; dt += Time.deltaTime) {
                dtNorm = dt / InterpolationTime;
                tempPos = Vector3.Lerp(start, end, _interpolationCurve.Evaluate(dtNorm));
                tempPos.y += Mathf.Sin(pi * dtNorm) * LegRaise;                       
                IKTarget.position = tempPos;       
                yield return null;
            }
            _IKPos = end;
            if (_step != null) {
                SoundFXManager.instance.PlaySoundFXClip(_step, this.transform, .5f);
            } 
            
            if (_stepParticles != null)
            {
                _stepParticles.Play();
            }
        }

        IEnumerator Walk(float dt, Vector3 start, Vector3 end) { 
           
            yield return StartCoroutine(WalkInterpolate(dt, start, end));                
        }
    }
}
