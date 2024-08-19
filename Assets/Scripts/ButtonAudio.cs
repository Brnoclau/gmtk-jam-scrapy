using System;
using Scrapy.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Scrapy
{
    [RequireComponent(typeof(Button))]
    public class ButtonAudio : MonoBehaviour
    {
        private Button _button;
        private void Awake()
        {
            _button = GetComponent<Button>();
            ApplyListener();
        }

        public void ApplyListener()
        {
            _button.onClick.AddListener(PlayAudio);
        }

        private void PlayAudio()
        {
            SfxManager.Instance.Play(GlobalConfig.Instance.audio.buttonClick);
        }
    }
}