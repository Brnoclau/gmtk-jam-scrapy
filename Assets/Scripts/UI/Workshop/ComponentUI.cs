using Scrapy.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scrapy.UI.Workshop
{
    [RequireComponent(typeof(Animator))]
    public class ComponentUI : MonoBehaviour
    {
        [SerializeField] private Image componentImage;
        [SerializeField] private TMP_Text componentName;
        [SerializeField] private TMP_Text count;
        [SerializeField] public Button button;
        [SerializeField] private Animator animator;

        
        public PlayerComponentConfig Config { get;private set; }

        public enum ComponentUIState
        {
            Normal = 0,
            Selected = 1,
            Disabled = 2
        }

        public ComponentUIState State
        {
            get => _state;
            set => SetState(value);
        }

        private ComponentUIState _state;
        
        private static readonly int StateAnimatorParam = Animator.StringToHash("state");

        public void InitFromConfig(PlayerComponentConfig config)
        {
            Config = config;
            componentImage.sprite = config.uiImage;
            componentName.text = config.uiName;
        }

        public void SetCount(int current, int max)
        {
            count.text = $"{current}/{max}";
        }

        private void SetState(ComponentUIState state)
        {
            _state = state;
            animator.SetInteger(StateAnimatorParam, (int)state);
            button.interactable = state != ComponentUIState.Disabled;
        }
    }
}