using System;
using Scrapy.Util;
using UnityEngine;

namespace Scrapy.Player
{
    public class WheelAudio : MonoBehaviour
    {
        [SerializeField] private WheelPlayerComponent wheel;
        [SerializeField] private AudioSource engineAudioSource;
        [SerializeField] private float engineMinVolume;
        [SerializeField] private AudioSource breaksAudioSource;
        [SerializeField] private AnimationCurve soundCurve;
        [SerializeField] private float topVolumeSpeed;
        [SerializeField] private float breaksVolumeDecay = 16;
        [SerializeField] private float minBreaksSpeed = 5;

        private Player _player;

        private void Start()
        {
            _player = GetComponentInParent<Player>();
            engineAudioSource.volume = 0;
        }

        private void Update()
        {
            if (!_player)
            {
                return;
            }
            var speed = Mathf.Abs(wheel.WheelJoint2D.jointSpeed);
            var clamped = Mathf.Clamp((Mathf.Abs(wheel.WheelInput) > 0 ? 1 : 0) * speed / topVolumeSpeed, engineMinVolume, 1);
            var targetVolume = clamped / _player.Wheels.Count;
            engineAudioSource.volume = ExpDecay.Decay(engineAudioSource.volume, targetVolume,
                breaksVolumeDecay, Time.deltaTime);

            bool isBreaking = wheel.IsBreaking && wheel.WheelJoint2D.jointLinearSpeed > minBreaksSpeed;
            breaksAudioSource.volume = ExpDecay.Decay(breaksAudioSource.volume, isBreaking ? 1 : 0,
                breaksVolumeDecay, Time.deltaTime);
        }
    }
}