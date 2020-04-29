using Kaisa.Digivice.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

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

        public override void SetComponentPosition(int x, int y) {
            spriteImage.gameObject.PlaceInPosition(x, y);

        }
        public override void SetComponentOffset(Vector2 offsetMin, Vector2 offsetMax) {
            throw new NotImplementedException();
            /*width *= Constants.PixelSize;
            height *= Constants.PixelSize;
            spriteImage.rectTransform.sizeDelta = new Vector2(width, height);*/
        }

        public void SetSprite(Sprite sprite) {
            spriteImage.sprite = sprite;
        }
    }
}