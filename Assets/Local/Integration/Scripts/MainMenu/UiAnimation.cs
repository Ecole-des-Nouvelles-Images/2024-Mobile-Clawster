using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Local.Integration.Scripts.MainMenu
{
    public class UIAnimation : BaseBehavior
    {
        [SerializeField] private CanvasGroup _title;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private GameObject _settingsPanel; 
        [SerializeField] private GameObject _volumePanel;
        [SerializeField] private GameObject _creditsPanel;
        [SerializeField] private AudioClip _buttonSoundA;
        [SerializeField] private AudioClip _buttonSoundB;

        
        public float animationDuration = 0.5f;
        private Vector3 hiddenScale = Vector3.zero;
        private Vector3 visibleScale = Vector3.one;
        
        private float INITIAL_DELAY = 1f;
        private const float DELAY_BETWEEN_BUTTONS = 0.3f;
        
        private List<Button> _buttons = new List<Button>();
        private List<Sequence> _animationSequence = new List<Sequence>();

        private void Awake()
        {
            _settingsPanel.transform.localScale = hiddenScale;
            _volumePanel.transform.localScale = hiddenScale;
            _creditsPanel.transform.localScale = hiddenScale;
            _buttons.Add(_settingsButton);
            AnimateTitle();
            AnimateButtons();
        }

        private void AnimateTitle()
        {
            _title.alpha = 0f;
            _title.DOFade(1f, 1.8f).SetEase(Ease.InQuint);
        }

        private void AnimationButton(int index, float delay)
        {
            if (_animationSequence.Count >= index)
            {
                _animationSequence.Add(DOTween.Sequence());
            }
            else
            {
                if (_animationSequence[index].IsPlaying())
                {
                    _animationSequence[index].Kill(true);
                }
            }

            var seq = _animationSequence[index];
            var button = _buttons[index];
            seq.Append(button.transform.DOScale(1, 0.1f));
            seq.Append(button.transform.DOPunchScale(Vector3.one * 0.6f, 0.8f, 6, 0.3f).SetEase(Ease.OutCirc));
            seq.PrependInterval(delay);
        }
        
        private void AnimateButtons()
        {
            for (int i = 0; i < 1; i++)
            {
                _buttons[i].transform.localScale = Vector3.zero;
                AnimationButton(i, INITIAL_DELAY + DELAY_BETWEEN_BUTTONS * i);
            }
        }
        
        public void OpenSettingsPanel()
        {
            _settingsPanel.transform.localScale = hiddenScale;
            _settingsPanel.transform.DOScale(visibleScale, animationDuration).SetEase(Ease.OutBack);
            SoundFXManager.instance.PlaySoundFXClip(_buttonSoundA, transform, 1f);
        }
        
        public void CloseSettingsPanel()
        {
            _settingsPanel.transform.DOScale(hiddenScale, animationDuration).SetEase(Ease.InBack);
            _settingsPanel.transform.localScale = hiddenScale;
            SoundFXManager.instance.PlaySoundFXClip(_buttonSoundB, transform, 1f);

        }
        
        public void OpenVolumePanel()
        {
            _volumePanel.transform.localScale = hiddenScale;
            _volumePanel.transform.DOScale(visibleScale, animationDuration).SetEase(Ease.OutBack);
            SoundFXManager.instance.PlaySoundFXClip(_buttonSoundA, transform, 1f);
        }
        
        public void CloseVolumePanel()
        {
            _volumePanel.transform.DOScale(hiddenScale, animationDuration).SetEase(Ease.InBack);
            _volumePanel.transform.localScale = hiddenScale;
            SoundFXManager.instance.PlaySoundFXClip(_buttonSoundB, transform, 1f);
        }
        
        public void OpenCreditsPanel()
        {
            _creditsPanel.transform.localScale = hiddenScale;
            _creditsPanel.transform.DOScale(visibleScale, animationDuration).SetEase(Ease.OutBack);
            SoundFXManager.instance.PlaySoundFXClip(_buttonSoundA, transform, 1f);
        }
        
        public void CloseCreditsPanel()
        {
            _creditsPanel.transform.DOScale(hiddenScale, animationDuration).SetEase(Ease.InBack);
            _creditsPanel.transform.localScale = hiddenScale;
            SoundFXManager.instance.PlaySoundFXClip(_buttonSoundB, transform, 1f);
        }
        
    }
}
