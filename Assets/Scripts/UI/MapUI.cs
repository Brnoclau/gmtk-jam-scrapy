using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scrapy.UI
{
    public class MapUI : MonoBehaviour
    {
        [SerializeField] private Image playerImage;

        private RectTransform _rectTransform;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (GameManager.Instance.Player == null) return;
            var playerPos = GameManager.Instance.Player.transform.position;
            var mapBottomLeft = GameManager.Instance.BottomLeft.position;
            var mapTopRight = GameManager.Instance.TopRight.position;
            var mapSize = mapTopRight - mapBottomLeft;
            var playerPosNormalized = new Vector2(
                Mathf.Clamp01((playerPos.x - mapBottomLeft.x) / mapSize.x),
                Mathf.Clamp01((playerPos.y - mapBottomLeft.y) / mapSize.y)
            );
            var playerPosOnMap = new Vector2(
                playerPosNormalized.x * _rectTransform.sizeDelta.x,
                playerPosNormalized.y * _rectTransform.sizeDelta.y
            );
            playerImage.rectTransform.anchoredPosition = playerPosOnMap;
        }
    }
}