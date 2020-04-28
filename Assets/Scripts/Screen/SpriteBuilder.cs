using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kaisa.Digivice.Extensions;

namespace Kaisa.Digivice {
    public class SpriteBuilder : ScreenElement {
        public Image spriteImage;

        //TODO: This won't work currently as sprites are always black instead of white.
        public override void InvertColors(bool val) {
            if (val) {
                spriteImage.color = Constants.backgroundColor;
                background.color = Constants.activeColor;
            }
            else {
                spriteImage.color = Constants.activeColor;
                background.color = Constants.backgroundColor;
            }
        }

        public void SetSprite(Sprite sprite) {
            spriteImage.sprite = sprite;
        }
    }
}