using Kaisa.Digivice.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class ScreenBuilder : ScreenElement {
        public Image spriteImage;
        public SpriteRenderer spriteRenderer;

        //TODO: This won't work currently as sprites are always black instead of white.
        public override void InvertColors(bool val) {
            if (val) {
                spriteImage.color = Constants.BACKGROUND_COLOR;
                background.color = Constants.ACTIVE_COLOR;
            }
            else {
                spriteImage.color = Constants.ACTIVE_COLOR;
                background.color = Constants.BACKGROUND_COLOR;
            }
        }

        public override void SetComponentPosition(int x, int y) {
            spriteImage.gameObject.PlaceInPosition(x, y);
        }
        public void SetComponentSize(int width, int height) {
            width *= Constants.PIXEL_SIZE;
            height *= Constants.PIXEL_SIZE;
            spriteImage.rectTransform.sizeDelta = new Vector2(width, height);
        }

        public void SetSprite(Sprite sprite) {
            spriteImage.sprite = sprite;
        }

        public void FlipHorizontal(bool flip) {
            spriteRenderer.flipX = flip;
        }

        public void FlipVertical(bool flip) {
            spriteRenderer.flipY = flip;
        }
    }
}
