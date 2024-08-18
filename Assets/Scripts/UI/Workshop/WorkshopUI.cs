using System;
using System.Collections.Generic;
using System.Linq;
using Scrapy.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scrapy.UI.Workshop
{
    [RequireComponent(typeof(Canvas))]
    public class WorkshopUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private ScrollRect componentsScrollView;
        [SerializeField] private RectTransform componentsContainer;
        [SerializeField] private ComponentUI componentPrefab;
        [SerializeField] private TMP_Text placementErrorText;
        [Header("Selected object actions")]
        [SerializeField] private RectTransform selectedObjectActionsUI;
        [SerializeField] private TMP_Text selectedComponentNameText;
        [SerializeField] private Button deleteComponentButton;

        private Canvas _canvas;
        private WorkshopController _workshopController;
        private readonly List<ComponentUI> _componentUIs = new();
        
        private ComponentUI _selectedUI;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _workshopController = GameManager.Instance.WorkshopController;
            _workshopController.ActiveChanged += OnActiveChanged;
            _workshopController.AddingComponentChanged += OnNewComponentAdding;
            _workshopController.AvailableComponentChanged += OnAvailableComponentChanged;
            _workshopController.AvailableComponentsChanged += OnAvailableComponentsChanged;
            _workshopController.SelectedComponentChanged += OnSelectedComponentChanged;
            playButton.onClick.AddListener(OnPlayButtonClicked);
            deleteComponentButton.onClick.AddListener(OnDeleteComponentClicked);
            OnSelectedComponentChanged(_workshopController.SelectedComponent);
            placementErrorText.text = "";
        }

        private void OnAvailableComponentsChanged()
        {
            UpdateAvailableComponents();
        }

        private void Update()
        {
            if (!_workshopController.Active) return;
            if (_workshopController.Mode == WorkshopController.WorkshopMode.Adding)
            {
                placementErrorText.text = _workshopController.PlaceObjectError;
            }
            else
            {
                placementErrorText.text = "";
            }
        }

        private void OnDeleteComponentClicked()
        {
            _workshopController.DeleteSelectedComponent();
        }

        private void OnSelectedComponentChanged(PlayerComponent newSelectedObject)
        {
            selectedObjectActionsUI.gameObject.SetActive(newSelectedObject != null);
            if (newSelectedObject != null)
            {
                selectedComponentNameText.text = newSelectedObject.Config.uiName;
            }
        }

        private void OnAvailableComponentChanged(AvailableComponent component)
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
            while (_componentUIs.Count < availableComponents.Count)
            {
                SpawnNewComponentUI();
            }
            if (_componentUIs.Count > availableComponents.Count)
            {
                DeleteComponentUIs(_componentUIs.Count - availableComponents.Count);
            }

            for (int i = 0; i < availableComponents.Count; i++)
            {
                var component = availableComponents[i];
                var ui = _componentUIs[i];
                UpdateComponentUIState(ui, component);
                ui.button.onClick.RemoveAllListeners();
                ui.button.onClick.AddListener(() => _workshopController.SetAddingComponent(component.componentConfig));
            }
        }

        void UpdateComponentUIState(ComponentUI ui, AvailableComponent component)
        {
            Debug.Log($"Updating component UI: {component.componentConfig.key} {component.count}/{component.maxCount}");
            ui.InitFromConfig(component.componentConfig);
            ui.SetCount(component.count, component.maxCount);
            
            ui.State = component.count > 0
                ? (component == _workshopController.AddingComponent ? 
                    ComponentUI.ComponentUIState.Selected : 
                    ComponentUI.ComponentUIState.Normal)
                : ComponentUI.ComponentUIState.Disabled;
        }

        void OnNewComponentAdding(AvailableComponent _)
        {
            var availableComponents = GameManager.Instance.WorkshopController.AvailableComponents;

            for (int i = 0; i < availableComponents.Count; i++)
            {
                var component = availableComponents[i];
                var ui = _componentUIs[i];
                UpdateComponentUIState(ui, component);
            }
            // if (_selectedUI != null)
            // {
            //     _selectedUI.State = component.count > 0
            //         ? (component == _workshopController.AddingComponent ? 
            //             ComponentUI.ComponentUIState.Selected : 
            //             ComponentUI.ComponentUIState.Normal)
            //         : ComponentUI.ComponentUIState.Disabled;
            // }
            //
            // if (availableComponent == null)
            // {
            //     return;
            // }
            //
            // var ui = _componentUIs.FirstOrDefault(x => x.Config == availableComponent.componentConfig);
            // if (ui != null)
            // {
            //     UpdateComponentUIState(_selectedUI, availableComponent);
            // }
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