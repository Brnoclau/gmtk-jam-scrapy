using Scrapy.UI;
using TMPro;
using UnityEngine;

namespace Scrapy
{
    public class DialogUI : FadeCanvas
    {
        [SerializeField] private TMP_Text participantText;
        [SerializeField] private TMP_Text lineText;

        protected override void Awake()
        {
            base.Awake();
            DialogManager.Instance.DialogActiveChanged += OnDialogActiveChanged;
            DialogManager.Instance.DialogLineChanged += OnDialogLineChanged;
        }

        private void OnDialogActiveChanged(bool value)
        {
            SetOpen(value);
        }

        private void OnDialogLineChanged(DialogLine line, DialogParticipant participant)
        {
            lineText.text = line.text;
            participantText.gameObject.SetActive(participant != null);
            if (participant != null)
            {
                participantText.text = participant.name;
            }
        }
    }
}