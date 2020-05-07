using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class VisualGameSlot : MonoBehaviour {
        public int Slot { get; private set; }
        [SerializeField] private Text gameName;
        [SerializeField] private Text character;
        [SerializeField] private Text level;
        [SerializeField] private Toggle toggle;

        public void SetData(BriefSavedGame sg, ToggleGroup toggleGroup) {
            Slot = sg.slot;
            gameName.text = sg.name;
            character.text = sg.character;
            level.text = $"Lv. {sg.level}";
            toggle.group = toggleGroup;
        }
    }
}