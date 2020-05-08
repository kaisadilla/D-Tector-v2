﻿using UnityEngine;

namespace Kaisa.Digivice {
    public static class Constants {
        public const int PIXEL_SIZE = 24;
        public const int SCREEN_WIDTH = 32;
        public const int SCREEN_HEIGHT = 32;
        //The speed at which attacks always travel.
        public const float ATTACK_TRAVEL_SPEED = 0.05f; //0.06f
        public const float CRUSH_TRAVEL_SPEED = 0.035f; //0.04f

        public const int MAX_SPIRIT_POWER = 99;
        public const string DEFAULT_DIGIMON = "numemon";
        public const string DEFAULT_SPIRIT_DIGIMON = "flamemon";

        public static readonly Vector2Int[][] AREA_POSITIONS = new Vector2Int[][] {
            new Vector2Int[] {
                new Vector2Int(18, 21),
                new Vector2Int(6, 27),
                new Vector2Int(26, 27),
                new Vector2Int(14, 7),
                new Vector2Int(28, 9),
                new Vector2Int(26, 19),
                new Vector2Int(3, 13),
                new Vector2Int(9, 3),
                new Vector2Int(21, 10),
                new Vector2Int(25, 27),
                new Vector2Int(6, 24),
                new Vector2Int(13, 18)
            }
        };
        //THIS IS ONLY TEMPORARY.
        public static Sprite EMPTY_SPRITE { get; private set; }
        public static void SetEmptySprite(Sprite sprite) => EMPTY_SPRITE = sprite;
    }
}