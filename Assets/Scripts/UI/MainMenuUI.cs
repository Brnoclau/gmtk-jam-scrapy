using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scrapy.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitToDesktopButton;
        [SerializeField] private string gameSceneName;
        
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private AudioClip sfxSoundOnSliderChange;

        [SerializeField] private CreditsUI creditsUI;

        private void Awake()
        {
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);   
            musicSlider.onValueChanged.AddListener(SetMusicVolume);   
            newGameButton.onClick.AddListener(StartNewGame);
            continueButton.onClick.AddListener(ContinueGame);
            creditsButton.onClick.AddListener(ShowCredits);
            exitToDesktopButton.onClick.AddListener(Application.Quit);
        }

        private void Start()
        {
            sfxSlider.SetValueWithoutNotify(OptionsManager.Instance.SfxVolume);
            musicSlider.SetValueWithoutNotify(OptionsManager.Instance.MusicVolume);
            continueButton.gameObject.SetActive(SaveManager.Instance.SaveFileExists());

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                exitToDesktopButton.gameObject.SetActive(false);
            }
        }

        private void StartNewGame()
        {
            SaveManager.Instance.ResetSave();
            LoadGameScene();
        }

        private void ContinueGame()
        {
            LoadGameScene();
        }

        private void LoadGameScene()
        {
            SceneLoadingManager.Instance.LoadScene(gameSceneName);
        }

        private void SetSfxVolume(float value)
        {
            OptionsManager.Instance.SfxVolume = value;
            SfxManager.Instance.Play(sfxSoundOnSliderChange);
        }

        private void SetMusicVolume(float value)
        {
            OptionsManager.Instance.MusicVolume = value;
        }

        private void ShowCredits()
        {
            creditsUI.ShowCredits();
        }
    }
}