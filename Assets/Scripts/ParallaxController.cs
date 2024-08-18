using System;
using Scrapy.Util;
using UnityEngine;

namespace Scrapy
{
    public class ParallaxController : MonoBehaviour
    {
        [SerializeField] private ParallaxEntry[] _parallaxEntries;
        [SerializeField] private Transform _cameraTransform;
        
        private void Update()
        {
            foreach (var entry in _parallaxEntries)
            {
                entry.spriteRenderer.transform.position = new Vector3(_cameraTransform.position.x * entry.parallaxFactor.x,
                    _cameraTransform.position.y * entry.parallaxFactor.y) + entry.offset.XY0();
            }
        }
    }


    [Serializable]
    public struct ParallaxEntry
    {
        public SpriteRenderer spriteRenderer;
        public Vector2 parallaxFactor;
        public Vector2 offset;
    }
}