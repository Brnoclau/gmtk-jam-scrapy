using Scrapy.Util;
using UnityEngine;

namespace Scrapy.Player
{
    public class ThrusterAudio : MonoBehaviour
    {
        [SerializeField] private ThrusterPlayerComponent thruster;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float breaksVolumeDecay = 4;

        private void Start()
        {
            audioSource.volume = 0;
        }

        private void Update()
        {
            var targetVolume = thruster.IsActive ? 1 : 0;
            audioSource.volume = ExpDecay.Decay(audioSource.volume, targetVolume,
                breaksVolumeDecay, Time.deltaTime);
        }
    }
}