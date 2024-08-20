using System.Collections.Generic;
using Scrapy.Player;
using TMPro;
using UnityEngine;

namespace Scrapy.UI
{
    public class ComponentsGroupUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text hotkeyText;
        [SerializeField] private ComponentUI componentPrefab;
        [SerializeField] private RectTransform componentsContainer;
        
        private readonly List<ComponentUI> _componentUIs = new();
        
        public void Init(List<ActionPlayerComponent> components, ActionHotkey hotkey)
        {
            hotkeyText.text = hotkey.name;

            foreach (var componentUI in _componentUIs)
            {
                Destroy(componentUI.gameObject);
            }
            
            _componentUIs.Clear();
            
            foreach (var component in components)
            {
                var componentUI = Instantiate(componentPrefab, componentsContainer);
                componentUI.Init(component);
                _componentUIs.Add(componentUI);
            }
        }
    }
}