using System;
using System.Collections.Generic;
using System.Linq;
using Scrapy.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Scrapy.UI
{
    public class ComponentsUI : MonoBehaviour
    {
        [SerializeField] private ComponentsGroupUI componentsGroupPrefab;
        [SerializeField] private RectTransform componentGroupsContainer;

        private readonly List<ComponentsGroupUI> _componentsGroupUIs = new();

        private void Awake()
        {
            GameManager.Instance.RespawnedPlayer += OnRespawnedPlayer;
        }

        private void OnRespawnedPlayer(Player.Player player)
        {
            foreach (var componentsGroupUI in _componentsGroupUIs)
            {
                Destroy(componentsGroupUI.gameObject);
            }

            _componentsGroupUIs.Clear();

            var attachments = player.AttachedComponents.Where(x => x.component is ActionPlayerComponent)
                .Select(x => x.component as ActionPlayerComponent);
            attachments.GroupBy(x => x.Hotkey.keyCode).ToList().ForEach(x =>
            {
                var componentsGroupUI = Instantiate(componentsGroupPrefab, componentGroupsContainer);
                componentsGroupUI.Init(x.ToList(), x.First().Hotkey);
                _componentsGroupUIs.Add(componentsGroupUI);
            });
            
            // GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}