using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapy
{
    
    [CreateAssetMenu(menuName = "Create DialogConfig", fileName = "DialogConfig", order = 0)]
    public class DialogConfig : ScriptableObject
    {
        public List<DialogParticipant> participants;
        public List<DialogLine> dialogLines;
        public string checkpointOnFinish;
    }

    [Serializable]
    public class DialogLine
    {
        public int participant;
        public string text;
    }

    [Serializable]
    public class DialogParticipant
    {
        public string name;
    }
}