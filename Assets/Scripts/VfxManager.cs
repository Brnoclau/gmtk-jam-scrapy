using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Scrapy
{
    public class VfxManager : MonoBehaviour
    {
        public static VfxManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Removing extra SfxManager");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Play(ParticleSystem particleSystem, Vector3 position)
        {
            if (particleSystem == null) return;
            Debug.Log("Playing particle system: " + particleSystem.name);
            var particleSystemInstance = Instantiate(particleSystem, position, Quaternion.identity);
            particleSystemInstance.Play();
        }
    }
}