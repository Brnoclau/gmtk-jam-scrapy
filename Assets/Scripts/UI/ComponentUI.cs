using Scrapy.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Scrapy.UI
{
    public class ComponentUI : MonoBehaviour
    {
        // [SerializeField] private Button button;
        [SerializeField] private Image componentImage;
        [SerializeField] private Image fadeImage;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color unavailableColor;

        private ActionPlayerComponent _component; 

        public void Init(ActionPlayerComponent component)
        {
            _component = component;
            componentImage.sprite = component.Config.uiImage;
        }

        private void Update()
        {
            if (_component == null) return;
            if (_component.OnCooldown)
            {
                fadeImage.fillAmount = _component.CooldownProgress;
            }
            else if (_component.Config.usesCharge)
            {
                fadeImage.fillAmount = 1 - _component.Charge / _component.MaxCharge;
            }
            else
            {
                fadeImage.fillAmount = 0;
            }

            if (_component.IsActive)
            {
                componentImage.color = Color.Lerp(normalColor, activeColor, Mathf.PingPong(Time.time, 1));
                // button.interactable = true;
            }
            else if (_component.OnCooldown || _component.Config.usesCharge && _component.Charge <= 0)
            {
                componentImage.color = unavailableColor;
                // button.interactable = false;
            }
            else
            {
                componentImage.color = normalColor;
                // button.interactable = true;
            }
        }
    }
}