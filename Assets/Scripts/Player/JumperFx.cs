using System;
using UnityEngine;

namespace Scrapy.Player
{
    public class JumperFx : MonoBehaviour
    {
        
        [SerializeField] private JumperPlayerComponent jumper;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float pitchVariation = 0.05f;

        private void Awake()
        {
            jumper.Used += JumperOnUsed;
        }

        private void JumperOnUsed()
        {
            audioSource.pitch = 1 + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            audioSource.Play();
        }
    }
}