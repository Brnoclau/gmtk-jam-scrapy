using System.Collections.Generic;
using UnityEngine;

namespace Scrapy
{
    public class SfxPlayer : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> clips;
        
        public void Play()
        {
            if (clips.Count == 0) return;
            var clip = clips[Random.Range(0, clips.Count)];
            SfxManager.Instance.Play(clip);
        }
    }
}