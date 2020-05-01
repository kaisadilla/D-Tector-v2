using Kaisa.Digivice.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class SpriteBuilder : ScreenElement {
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

        public int ComponentWidth {
            get => (int)(spriteImage.gameObject.GetComponent<RectTransform>().sizeDelta.x / Constants.PIXEL_SIZE);
        }
        public int ComponentHeight {
            get => (int)(spriteImage.gameObject.GetComponent<RectTransform>().sizeDelta.y / Constants.PIXEL_SIZE);
        }
        public Vector2Int ComponentPosition {
            get {
                Vector2 pos = spriteImage.gameObject.GetComponent<RectTransform>().anchoredPosition;
                return new Vector2Int((int)(pos.x / Constants.PIXEL_SIZE), (int)(-pos.y / Constants.PIXEL_SIZE));
            }
        }

        /// <summary>
        /// Sets the size (in digivice pixels) of both the rectangle and the component.
        /// </summary>
        public override void SetSize(int width, int height) {
            base.SetSize(width, height);
            SetComponentSize(width, height);
        }

        /// <summary>
        /// Sets the position of the component sprite inside the Sprite Builder.
        /// </summary>
        public override void SetComponentPosition(int x, int y) {
            spriteImage.gameObject.PlaceInPosition(x, y);
        }
        /// <summary>
        /// Sets the size of the component sprite, without resizing the Sprite Builder.
        /// </summary>
        public void SetComponentSize(int width, int height) {
            width *= Constants.PIXEL_SIZE;
            height *= Constants.PIXEL_SIZE;
            spriteImage.rectTransform.sizeDelta = new Vector2(width, height);
        }

        /// <summary>
        /// Centers the component inside the Sprite Builder.
        /// </summary>
        public void CenterComponent() {
            int x = Mathf.RoundToInt((Width - ComponentWidth) / 2f);
            int y = Mathf.RoundToInt((Height - ComponentHeight) / 2f);
            SetComponentPosition(x, y);
        }

        /// <summary>
        /// Snaps the component to a side of the Sprite Builder.
        /// </summary>
        /// <param name="side">The side to which the component will be snapped.</param>
        /// <param name="center">Whether the other axis will be centered.</param>
        public void SnapComponentToSide(Direction side, bool center = false) {
            if (center == true) CenterComponent();
            switch(side) {
                case Direction.Left:
                    spriteImage.gameObject.PlaceInPosition(0, ComponentPosition.y);
                    break;
                case Direction.Right:
                    spriteImage.gameObject.PlaceInPosition(Width - ComponentWidth, ComponentPosition.y);
                    break;
                case Direction.Up:
                    spriteImage.gameObject.PlaceInPosition(ComponentPosition.x, 0);
                    break;
                case Direction.Down:
                    spriteImage.gameObject.PlaceInPosition(ComponentPosition.x, Height - ComponentHeight);
                    break;
            }
        }

        public void SetSprite(Sprite sprite) {
            spriteImage.sprite = sprite;
        }

        /// <summary>
        /// Flips the component of the Sprite Builder horizontally. This does not flip the Sprite Builder itself.
        /// </summary>
        public SpriteBuilder FlipHorizontal(bool flip) {
            spriteRenderer.flipX = flip;
            return this;
        }

        /// <summary>
        /// Flips the component of the Sprite Builder vertically. This does not flip the Sprite Builder itself.
        /// </summary>
        public SpriteBuilder FlipVertical(bool flip) {
            spriteRenderer.flipY = flip;
            return this;
        }
    }
}