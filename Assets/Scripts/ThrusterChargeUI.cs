using UnityEngine;
using UnityEngine.UI;

namespace Scrapy
{
    public class ThrusterChargeUI : MonoBehaviour
    {
        [SerializeField] private Image _chargeBar;

        // Update is called once per frame
        void Update()
        {
            var player = GameManager.Instance.Player;
            if (player == null) return;
            // _chargeBar.fillAmount = Mathf.Lerp(_chargeBar.fillAmount, player.ThrusterCharge / player.MaxThrusterCharge, 0.1f);
        }
    }
}