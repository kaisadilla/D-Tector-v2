using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kaisa.Digivice.Extensions;

namespace Kaisa.Digivice {
    public class SpriteBuilder : ScreenElement {
        public Image spriteImage;

        public void SetSprite(Sprite sprite) {
            spriteImage.sprite = sprite;
        }
    }
}