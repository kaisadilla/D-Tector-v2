using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public static class Constants {
        public const int PixelSize = 24;
        public const int ScreenWidth = 32;
        public const int ScreenHeight = 32;

        public static readonly Vector2Int[][] areaPositions = new Vector2Int[][] {
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
    }
}