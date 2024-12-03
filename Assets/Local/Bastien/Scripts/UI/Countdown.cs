using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public TMP_Text CountdownText;
    public int CountdownTime;
    public Vector3 InitialScale;
    public Vector3 InitialRotation;

    private string GoText;

    public void StartCountdown() {
        if (CountdownTime <= 0) return;
        
        StartCoroutine(CountdownCoroutine(CountdownTime));
    }

    IEnumerator CountdownCoroutine(int time) {
        
        CountdownText.transform.localScale = InitialScale;
        
        if (time <= 1) {
            CountdownText.text = GoText;
            CountdownText.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutExpo);
            yield return new WaitForSeconds(1f);
            CountdownText.gameObject.SetActive(false);
            yield break;
        }
        
        time--; CountdownText.text = time.ToString();
        CountdownText.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(1f);
        
        StartCoroutine(CountdownCoroutine(time));
    }

    // Update is called once per frame
    void Start() {
        GoText = CountdownText.text;
        
        StartCountdown();
        
    }
}
