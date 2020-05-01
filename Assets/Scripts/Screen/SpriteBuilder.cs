using Kaisa.Digivice.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class SpriteBuilder : ScreenElement {
        public Image spriteImage;

        //TODO: This won't work currently as sprites are always black instead of white.
        public override T InvertColors<T>(bool val) {
            if (val) {
                spriteImage.color = Constants.BACKGROUND_COLOR;
                background.color = Constants.ACTIVE_COLOR;
            }
            else {
                spriteImage.color = Constants.ACTIVE_COLOR;
                background.color = Constants.BACKGROUND_COLOR;
            }
            return this as T;
        }

        public int ComponentWidth {
            get => (int)(spriteImage.rectTransform.sizeDelta.x / Constants.PIXEL_SIZE);
        }
        public int ComponentHeight {
            get => (int)(spriteImage.rectTransform.sizeDelta.y / Constants.PIXEL_SIZE);
        }
        public Vector2Int ComponentPosition {
            get {
                Vector2 pos = spriteImage.rectTransform.anchoredPosition;
                //The position in digivice pixels is equal to the position, +/- the dimensions of the builder if flipped, divided by the pixel size.
                int flipOffsetX = (IsComponentHorizontallyFlip ? Width : 0) * Constants.PIXEL_SIZE;
                int flipOffsetY = (IsComponentVerticallyFlip ? Height : 0) * Constants.PIXEL_SIZE;
                int posX = (int)((pos.x - flipOffsetX) / Constants.PIXEL_SIZE);
                int posY = (int)((-pos.y + flipOffsetY) / Constants.PIXEL_SIZE);
                return new Vector2Int(posX, posY);
            }
        }
        public Quaternion ComponentRotation {
            get => spriteImage.gameObject.transform.localRotation;
        }

        public bool IsComponentHorizontallyFlip => ComponentRotation.y == 1;
        public bool IsComponentVerticallyFlip => ComponentRotation.x == 1;

        /// <summary>
        /// Sets the size (in digivice pixels) of both the rectangle and the component.
        /// </summary>
        public override T SetSize<T>(int width, int height) {
            base.SetSize<T>(width, height);
            SetComponentSize(width, height);
            return this as T;
        }

        /// <summary>
        /// Sets the position of the component sprite inside the Sprite Builder.
        /// </summary>
        public override T SetComponentPosition<T>(int x, int y) {
            int flipOffsetX = IsComponentHorizontallyFlip ? Width : 0;
            int flipOffsetY = IsComponentVerticallyFlip ? Height : 0;
            spriteImage.gameObject.PlaceInPosition(x + flipOffsetX, y + flipOffsetY);
            return this as T;
        }
        public SpriteBuilder SetComponentX(int x) {
            int flipOffset = IsComponentHorizontallyFlip ? Width : 0;
            SetComponentPosition(x + flipOffset, ComponentPosition.y);
            return this;
        }
        public SpriteBuilder SetComponentY(int y) {
            int flipOffset = IsComponentVerticallyFlip ? Height : 0;
            SetComponentPosition(ComponentPosition.x, y + flipOffset);
            return this;
        }
        /// <summary>
        /// Sets the size of the component sprite, without resizing the Sprite Builder.
        /// </summary>
        public SpriteBuilder SetComponentSize(int width, int height) {
            width *= Constants.PIXEL_SIZE;
            height *= Constants.PIXEL_SIZE;
            spriteImage.rectTransform.sizeDelta = new Vector2(width, height);
            return this;
        }

        /// <summary>
        /// Centers the component inside the Sprite Builder.
        /// </summary>
        public SpriteBuilder CenterComponent() { //TODO: Fix a bug in which this bugs if used after FlipHorizontal/Vertical.
            int x = Mathf.RoundToInt((Width - ComponentWidth) / 2f);
            int y = Mathf.RoundToInt((Height - ComponentHeight) / 2f);
            //Debug.Log($"centerx: {x}, y {y}");
            SetComponentPosition(x, y);
            return this;
        }

        /// <summary>
        /// Snaps the component to a side of the Sprite Builder.
        /// </summary>
        /// <param name="side">The side to which the component will be snapped.</param>
        /// <param name="center">Whether the other axis will be centered.</param>
        public SpriteBuilder SnapComponentToSide(Direction side, bool center = false) {
            if (center == true) CenterComponent();
            switch(side) {
                case Direction.Left:
                    SetComponentX(0);
                    break;
                case Direction.Right:
                    SetComponentX(Width - ComponentWidth);
                    break;
                case Direction.Up:
                    SetComponentY(0);
                    break;
                case Direction.Down:
                    SetComponentY(Height - ComponentHeight);
                    break;
            }
            return this;
        }

        public SpriteBuilder SetSprite(Sprite sprite) {
            spriteImage.sprite = sprite;
            return this;
        }
        public Sprite GetSprite() =>spriteImage.sprite;

        /// <summary>
        /// Flips the component of the Sprite Builder horizontally. This does not flip the Sprite Builder itself.
        /// </summary>
        public SpriteBuilder FlipHorizontal(bool flip) {
            Vector2Int oldPosition = ComponentPosition;
            if (flip != IsComponentHorizontallyFlip) {
                if (flip) {
                    spriteImage.gameObject.transform.localRotation = Quaternion.Euler(ComponentRotation.x, 180, 0);
                }
                else {
                    spriteImage.gameObject.transform.localRotation = Quaternion.Euler(ComponentRotation.x, 0, 0);
                }
                SetComponentPosition(oldPosition);
            }

            return this;
        }

        /// <summary>
        /// Flips the component of the Sprite Builder vertically. This does not flip the Sprite Builder itself.
        /// </summary>
        public SpriteBuilder FlipVertical(bool flip) {
            Vector2Int oldPosition = ComponentPosition;
            if(flip != IsComponentVerticallyFlip) {
                if (flip) {
                    spriteImage.gameObject.transform.localRotation = Quaternion.Euler(180, ComponentRotation.y, 0);
                }
                else {
                    spriteImage.gameObject.transform.localRotation = Quaternion.Euler(0, ComponentRotation.y, 0);
                }
            }
            SetComponentPosition(oldPosition);

            return this;
        }
    }
}