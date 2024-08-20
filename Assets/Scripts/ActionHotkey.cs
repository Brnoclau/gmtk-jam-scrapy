using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Scrapy
{
    [Serializable]
    public class ActionHotkey
    {
        public KeyCode keyCode;
        public string name;
    }
}