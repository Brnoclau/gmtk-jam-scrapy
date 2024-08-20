using System;
using UnityEngine;

namespace Scrapy
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField] private float nextLineInputDelay = 0.2f;
        
        public static DialogManager Instance { get; private set; }

        public event Action<bool> DialogActiveChanged;
        public event Action<DialogLine, DialogParticipant> DialogLineChanged;
        
        public bool Active => CurrentDialog != null;
        public DialogConfig CurrentDialog { get; private set; }
        public DialogLine CurrentLine { get; private set; }
        public DialogParticipant CurrentParticipant { get; private set; }

        private int _currentLineIndex = -1;
        private float _lastNextLineTime = -100;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There should only be one DialogManager in the scene");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Update()
        {
            if (!Active) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextLine();
            }
        }

        public void StartDialog(DialogConfig dialogConfig)
        {
            if (GameManager.Instance.State != GameState.Playing)
            {
                Debug.LogError("Tried to start dialog when not in Playing state");
                return;
            }
            
            if (CurrentDialog != null)
            {
                Debug.LogError("Tried to start dialog when already in dialog");
                return;
            }

            if (dialogConfig.dialogLines.Count == 0)
            {
                Debug.LogError("Tried to start dialog with no lines");
                return;
            }

            CurrentDialog = dialogConfig;
            GameManager.Instance.State = GameState.Dialog;
            DialogActiveChanged?.Invoke(true);
            SetLineIndex(0);
        }

        public void NextLine(bool force = false)
        {
            if (!force && _lastNextLineTime + nextLineInputDelay > Time.unscaledTime)
            {
                return;
            }

            _lastNextLineTime = Time.unscaledTime;
            
            if (_currentLineIndex + 1 < CurrentDialog.dialogLines.Count)
            {
                SetLineIndex(_currentLineIndex + 1);
            }
            else
            {
                FinishDialog();
            }
        }

        public void FinishDialog()
        {
            if (CurrentDialog != null && !string.IsNullOrWhiteSpace(CurrentDialog.checkpointOnFinish))
            {
                QuestManager.Instance.CompleteCheckpoint(CurrentDialog.checkpointOnFinish);
            }
            CurrentDialog = null;
            CurrentLine = null;
            CurrentParticipant = null;
            GameManager.Instance.State = GameState.Playing;
            DialogActiveChanged?.Invoke(false);
        }

        private void SetLineIndex(int index)
        {
            _currentLineIndex = index;
            CurrentLine = CurrentDialog.dialogLines[index];
            CurrentParticipant = CurrentLine.participant >= 0 && CurrentLine.participant < CurrentDialog.participants.Count
                    ? CurrentDialog.participants[CurrentLine.participant]
                    : null;
            DialogLineChanged?.Invoke(CurrentLine, CurrentParticipant);
        }
    }
}