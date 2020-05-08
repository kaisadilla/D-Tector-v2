using UnityEngine;

namespace Kaisa.Digivice {
    public static class Preferences {
        public static float Volume { get; set; } = 1f;
        public static int Localization { get; set; } = 0; //0: D-Tector, 1: Japan, 2: U.S.
        public static Color ActiveColor { get; set; } = Color.black;
        public static Color BackgroundColor { get; set; } = new Color32(129, 147, 118, 255);

        public static void ApplyPreferences() {
            Volume = SavedGame.ConfigVolume;
            Localization = SavedGame.ConfigLocalization;
            ActiveColor = SavedGame.ConfigActiveColor;
            BackgroundColor = SavedGame.ConfigBackgroundColor;
        }
    }
}