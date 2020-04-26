using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kaisa.Digivice.Extensions;

namespace Kaisa.Digivice {
    public class SpriteBuilder : MonoBehaviour, IScreenElement {
        public Image screen;

        //IScreenElement methods:
        public void Dispose() => Destroy(gameObject);
        public void SetActive(bool active) => gameObject.SetActive(active);
        public void SetName(string name) => gameObject.name = name;
        public void SetPosition(int x, int y) {
            gameObject.PlaceInPosition(x, y);
        }
        public void SetPosition(Vector2Int pos) => SetPosition(pos.x, pos.y);
        public void SetSize(int width, int height) {
            width *= Constants.PixelSize;
            height *= Constants.PixelSize;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }

        //Specific methods:
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

        public void SetSprite(Sprite sprite) {
            screen.sprite = sprite;
        }
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

        public void FlipHorizontal(bool flip) {
            if(flip) SetRotation(0, 180, 0);
            else SetRotation(0, 0, 0);
        }

        public void FlipVertical(bool flip) {
            if (flip) SetRotation(180, 0, 0);
            else SetRotation(0, 0, 0);
        }

        private void SetRotation(int x, int y, int z) {
            screen.gameObject.transform.localRotation = Quaternion.Euler(x, y, z);
        }
    }
}