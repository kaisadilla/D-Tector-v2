using Kaisa.Digivice.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class SpriteBuilder : ScreenElement {
        public Image spriteImage;

        //Properties:
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
        public Sprite Sprite => spriteImage.sprite;

        //Overrides:
        //TODO: This won't work currently as sprites are always black instead of white.
        protected override void BaseInvertColors(bool val) {
            if (val) {
                spriteImage.color = Constants.BACKGROUND_COLOR;
                background.color = Constants.ACTIVE_COLOR;
            }
            else {
                spriteImage.color = Constants.ACTIVE_COLOR;
                background.color = Constants.BACKGROUND_COLOR;
            }
        }
        protected override void BaseSetSize(int width, int height) {
            base.BaseSetSize(width, height);
            SetComponentSize(width, height);
        }
        //Chained base methods:
        public SpriteBuilder Center() {
            BaseCenter();
            return this;
        }
        public SpriteBuilder InvertColors(bool val) {
            BaseInvertColors(val);
            return this;
        }
        public SpriteBuilder Move(Direction direction, int amount = 1) {
            BaseMove(direction, amount);
            return this;
        }
        public SpriteBuilder PlaceOutside(Direction direction) {
            BasePlaceOutside(direction);
            return this;
        }
        public SpriteBuilder SetActive(bool active) {
            BaseSetActive(active);
            return this;
        }
        public SpriteBuilder SetPosition(int x, int y) {
            BaseSetPosition(x, y);
            return this;
        }
        public SpriteBuilder SetPosition(Vector2Int pos) {
            BaseSetPosition(pos);
            return this;
        }
        public SpriteBuilder SetSize(int width, int length) {
            BaseSetSize(width, length);
            return this;
        }
        public SpriteBuilder SetTransparent(bool val) {
            BaseSetTransparent(val);
            return this;
        }
        public SpriteBuilder SetX(int x) {
            BaseSetX(x);
            return this;
        }
        public SpriteBuilder SetY(int y) {
            BaseSetY(y);
            return this;
        }

        //Extra methods:
        /// <summary>
        /// Centers the component inside the Sprite Builder.
        /// </summary>
        public SpriteBuilder CenterComponent() { //TODO: Fix a bug in which this bugs if used after FlipHorizontal/Vertical.
            int x = Mathf.RoundToInt((Width - ComponentWidth) / 2f);
            int y = Mathf.RoundToInt((Height - ComponentHeight) / 2f);
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
        public SpriteBuilder SetComponentSize(int width, int height) {
            width *= Constants.PIXEL_SIZE;
            height *= Constants.PIXEL_SIZE;
            spriteImage.rectTransform.sizeDelta = new Vector2(width, height);
            return this;
        }
        public SpriteBuilder SetComponentPosition(int x, int y) {
            int flipOffsetX = IsComponentHorizontallyFlip ? Width : 0;
            int flipOffsetY = IsComponentVerticallyFlip ? Height : 0;
            spriteImage.gameObject.PlaceInPosition(x + flipOffsetX, y + flipOffsetY);
            return this;
        }
        public SpriteBuilder SetComponentPosition(Vector2Int pos) => SetComponentPosition(pos.x, pos.y);
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

    }
}