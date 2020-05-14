using UnityEngine;

namespace Kaisa.Digivice {
    public static class Constants {
        public const string GAME_VERSION = "0.20.0513a";

        public const int PIXEL_SIZE = 24;
        public const int SCREEN_WIDTH = 32;
        public const int SCREEN_HEIGHT = 32;
        //The speed at which attacks always travel.
        public const float ATTACK_TRAVEL_SPEED = 0.05f; //0.06f
        public const float CRUSH_TRAVEL_SPEED = 0.035f; //0.04f

        public const int MAX_SPIRIT_POWER = 99;
        public const string DEFAULT_DIGIMON = "numemon";
        public const string DEFAULT_SPIRIT_DIGIMON = "flamemon";

        //THIS IS ONLY TEMPORARY.
        public static Sprite EMPTY_SPRITE { get; private set; }
        public static void SetEmptySprite(Sprite sprite) => EMPTY_SPRITE = sprite;
    }
}