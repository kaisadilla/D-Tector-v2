using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public abstract class ScreenElement : MonoBehaviour {
        public Image background;
        public int Width {
            get => (int)(gameObject.GetComponent<RectTransform>().sizeDelta.x / Constants.PixelSize);
        }
        public int Height {
            get => (int)(gameObject.GetComponent<RectTransform>().sizeDelta.y / Constants.PixelSize);
        }
        public Vector2Int Position {
            get {
                Vector2 pos = gameObject.GetComponent<RectTransform>().anchoredPosition;
                return new Vector2Int((int)pos.x, (int)pos.y);
            }
        }
        public virtual void Dispose() => Destroy(gameObject);
        public void SetActive(bool active) => gameObject.SetActive(active);
        public void SetName(string name) => gameObject.name = name;

        /// <summary>
        /// Sets whether the element's background is transparent or not.
        /// </summary>
        /// <param name="val">The value of transparency.</param>
        public void SetTransparent(bool val) {
            background.enabled = val;
        }
        /// <summary>
        /// Sets the size (in digivice pixels) of the rectangle.
        /// </summary>
        public void SetSize(int width, int height) {
            width *= Constants.PixelSize;
            height *= Constants.PixelSize;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
        public void SetPosition(int x, int y) {
            gameObject.PlaceInPosition(x, y);
        }
        public void SetPosition(Vector2Int pos) => SetPosition(pos.x, pos.y);
        public void PlaceOutside(Direction direction) {
            int coordinateUp = -Height;
            int coordinateDown = Constants.ScreenHeight;
            int coordinateLeft = -Width;
            int coordinateRight = Constants.ScreenWidth;

            switch (direction) {
                case Direction.Up:
                    gameObject.PlaceInPosition(0, coordinateUp);
                    break;
                case Direction.Down:
                    gameObject.PlaceInPosition(0, coordinateDown);
                    break;
                case Direction.Left:
                    gameObject.PlaceInPosition(coordinateLeft, 0);
                    break;
                case Direction.Right:
                    gameObject.PlaceInPosition(coordinateRight, 0);
                    break;
            }
        }
        public void MoveSprite(Direction direction, int amount = 1) => gameObject.MoveSprite(direction, amount);

        public void FlipHorizontal(bool flip) {
            if (flip) SetRotation(0, 180, 0);
            else SetRotation(0, 0, 0);
        }

        public void FlipVertical(bool flip) {
            if (flip) SetRotation(180, 0, 0);
            else SetRotation(0, 0, 0);
        }

        private void SetRotation(int x, int y, int z) {
            gameObject.transform.localRotation = Quaternion.Euler(x, y, z);
        }

        public abstract void InvertColors(bool val);
    }
}