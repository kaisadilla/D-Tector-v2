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
            get => (int)(gameObject.GetComponent<RectTransform>().sizeDelta.x / Constants.PIXEL_SIZE);
        }
        public int Height {
            get => (int)(gameObject.GetComponent<RectTransform>().sizeDelta.y / Constants.PIXEL_SIZE);
        }
        public Vector2Int Position {
            get {
                Vector2 pos = gameObject.GetComponent<RectTransform>().anchoredPosition;
                return new Vector2Int((int)(pos.x / Constants.PIXEL_SIZE), (int)(-pos.y / Constants.PIXEL_SIZE));
            }
        }
        public virtual void Dispose() => Destroy(gameObject);
        public void SetActive(bool active) => gameObject.SetActive(active);

        public bool GetActive() => gameObject.activeSelf;
        public void SetName(string name) => gameObject.name = name;

        /// <summary>
        /// Sets whether the element's background is transparent or not.
        /// </summary>
        /// <param name="val">The value of transparency.</param>
        public void SetTransparent(bool val) {
            background.enabled = !val;
        }
        /// <summary>
        /// Sets the size (in digivice pixels) of the rectangle.
        /// </summary>
        public virtual void SetSize(int width, int height) {
            width *= Constants.PIXEL_SIZE;
            height *= Constants.PIXEL_SIZE;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
        public void SetPosition(int x, int y) {
            gameObject.PlaceInPosition(x, y);
        }
        public void SetPosition(Vector2Int pos) => SetPosition(pos.x, pos.y);
        //public abstract void SetComponentOffset(Vector2 offsetMin, Vector2 offsetMax);
        public abstract void SetComponentPosition(int x, int y);
        public void SetComponentPosition(Vector2Int pos) => SetComponentPosition(pos.x, pos.y);
        public void PlaceOutside(Direction direction) {
            int coordinateUp = -Height;
            int coordinateDown = Constants.SCREEN_HEIGHT;
            int coordinateLeft = -Width;
            int coordinateRight = Constants.SCREEN_WIDTH;

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
        public ScreenElement MoveSprite(Direction direction, int amount = 1) {
            gameObject.MoveSprite(direction, amount);
            return this;
        }

        private void SetRotation(int x, int y, int z) {
            gameObject.transform.localRotation = Quaternion.Euler(x, y, z);
        }

        public abstract void InvertColors(bool val);
    }
}