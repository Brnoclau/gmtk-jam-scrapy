using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class ThrusterChargeUI : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private Image _chargeBar;

        // Update is called once per frame
        void Update()
        {
            _chargeBar.fillAmount = Mathf.Lerp(_chargeBar.fillAmount, _player.ThrusterCharge / _player.MaxThrusterCharge, 0.1f);
        }
    }
}