using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UI.Workshop
{
    [RequireComponent(typeof(Canvas))]
    public class WorkshopUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private ScrollRect componentsScrollView;
        [SerializeField] private RectTransform componentsContainer;
        [SerializeField] private ComponentUI componentPrefab;

        private Canvas _canvas;
        private WorkshopController _workshopController;
        private readonly List<ComponentUI> _componentUIs = new();
        
        private ComponentUI _selectedUI;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _workshopController = GameManager.Instance.WorkshopController;
            _workshopController.ActiveChanged += OnActiveChanged;
            _workshopController.SelectedComponentChanged += OnNewComponentSelected;
            _workshopController.PlacedComponent += OnPlacedComponent;
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnPlacedComponent(AvailableComponent component)
        {
            var ui = _componentUIs.FirstOrDefault(x => x.Config == component.componentConfig);
            if (ui == null)
            {
                Debug.LogError("Placed component doesn't have a UI in the workshop for some reason!");
            }
            UpdateComponentUIState(ui, component);
        }

        private void OnActiveChanged(bool value)
        {
            _canvas.enabled = value;
            if (value)
            {
                UpdateAvailableComponents();
            }
        }

        public void UpdateAvailableComponents()
        {
            var availableComponents = GameManager.Instance.WorkshopController.AvailableComponents;
            if (_componentUIs.Count < availableComponents.Count)
            {
                SpawnNewComponentUI();
            }
            else if (_componentUIs.Count > availableComponents.Count)
            {
                DeleteComponentUIs(_componentUIs.Count - availableComponents.Count);
            }

            for (int i = 0; i < availableComponents.Count; i++)
            {
                var component = availableComponents[i];
                var ui = _componentUIs[i];
                UpdateComponentUIState(ui, component);
                ui.button.onClick.AddListener(() => _workshopController.TrySetSelectedComponent(component.componentConfig));
            }
        }

        void UpdateComponentUIState(ComponentUI ui, AvailableComponent component)
        {
            ui.InitFromConfig(component.componentConfig);
            ui.SetCount(component.count, component.maxCount);
            ui.State = component.count > 0
                ? ComponentUI.ComponentUIState.Normal
                : ComponentUI.ComponentUIState.Disabled;
        }

        void OnNewComponentSelected(AvailableComponent availableComponent)
        {
            if (_selectedUI != null)
            {
                _selectedUI.State = ComponentUI.ComponentUIState.Normal;
            }

            if (availableComponent == null)
            {
                return;
            }
            var ui = _componentUIs.FirstOrDefault(x => x.Config == availableComponent.componentConfig);
            if (ui != null)
            {
                _selectedUI = ui;
                _selectedUI.State = ComponentUI.ComponentUIState.Selected;
            }
        }

        void SpawnNewComponentUI()
        {
            var newUI = Instantiate(componentPrefab, componentsContainer);
            _componentUIs.Add(newUI);
        }

        void DeleteComponentUIs(int count)
        {
            var amountToRemove = Math.Min(count, _componentUIs.Count);
            for (int i = 0; i < amountToRemove; i++)
            {
                var ui = _componentUIs.Last();
                Destroy(ui.gameObject);
            }

            _componentUIs.RemoveRange(_componentUIs.Count - amountToRemove, amountToRemove);
        }

        private void OnPlayButtonClicked()
        {
            GameManager.Instance.State = GameState.Playing;
        }
    }
}