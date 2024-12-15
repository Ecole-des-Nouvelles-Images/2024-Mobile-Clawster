using System;
using System.Collections;
using UnityEngine;

namespace Local.Integration.Scripts.MainMenu
{
    public class BaseBehavior : MonoBehaviour
    {
        protected Coroutine Invoke(Action action, float time)
        {
            return StartCoroutine(InvokeAfterTime(action, time));
        }

        private IEnumerator InvokeAfterTime(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            
            action?.Invoke();
        }
        
    }
}