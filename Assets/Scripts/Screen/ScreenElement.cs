using Kaisa.Digivice.Extensions;
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
        public T SetActive<T>(bool active) where T : ScreenElement {
            gameObject.SetActive(active);
            return this as T;
        }
        public void SetActive(bool active) => SetActive<ScreenElement>(active);

        public bool GetActive() => gameObject.activeSelf;
        public T SetName<T>(string name) where T : ScreenElement {
            gameObject.name = name;
            return this as T;
        }
        public void SetName(string name) => SetName<ScreenElement>(name);

        /// <summary>
        /// Sets whether the element's background is transparent or not.
        /// </summary>
        /// <param name="val">The value of transparency.</param>
        public T SetTransparent<T>(bool val) where T : ScreenElement {
            background.enabled = !val;
            return this as T;
        }
        public void SetTransparent(bool val) => SetTransparent<ScreenElement>(val);
        /// <summary>
        /// Sets the size (in digivice pixels) of the rectangle.
        /// </summary>
        public virtual T SetSize<T>(int width, int height) where T : ScreenElement {
            width *= Constants.PIXEL_SIZE;
            height *= Constants.PIXEL_SIZE;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            return this as T;
        }
        public void SetSize(int width, int height) => SetSize<ScreenElement>(width, height);
        public T SetPosition<T>(int x, int y) where T : ScreenElement {
            gameObject.PlaceInPosition(x, y);
            return this as T;
        }
        public void SetPosition(int x, int y) => SetPosition<ScreenElement>(x, y);
        public T SetPosition<T>(Vector2Int pos) where T : ScreenElement {
            SetPosition<T>(pos.x, pos.y);
            return this as T;
        }
        public void SetPosition(Vector2Int pos) => SetPosition<ScreenElement>(pos);

        public T SetX<T>(int x) where T : ScreenElement {
            SetPosition(x, Position.y);
            return this as T;
        }
        public void SetX(int x) => SetX<ScreenElement>(x);
        public T SetY<T>(int y) where T : ScreenElement {
            SetPosition(Position.x, y);
            return this as T;
        }
        public void SetY(int y) => SetY<ScreenElement>(y);

        public abstract T SetComponentPosition<T>(int x, int y) where T : ScreenElement;
        public void SetComponentPosition(int x, int y) => SetComponentPosition<ScreenElement>(x, y);
        public T SetComponentPosition<T>(Vector2Int pos) where T : ScreenElement {
            SetComponentPosition<T>(pos.x, pos.y);
            return this as T;
        }
        public void SetComponentPosition(Vector2Int pos) => SetComponentPosition<ScreenElement>(pos);

        public T Center<T>() where T : ScreenElement {
            int x = Mathf.RoundToInt((Constants.SCREEN_WIDTH - Width) / 2f);
            int y = Mathf.RoundToInt((Constants.SCREEN_HEIGHT - Width) / 2f);
            SetPosition(x, y);
            return this as T;
        }
        /// <summary>
        /// Centers the builder in the middle of the screen.
        /// </summary>
        public void Center() => Center<ScreenElement>();
        public T PlaceOutside<T>(Direction direction) where T : ScreenElement {
            int coordinateUp = -Height;
            int coordinateDown = Constants.SCREEN_HEIGHT;
            int coordinateLeft = -Width;
            int coordinateRight = Constants.SCREEN_WIDTH;

            switch (direction) {
                case Direction.Up:
                    SetY(coordinateUp);
                    break;
                case Direction.Down:
                    SetY(coordinateDown);
                    break;
                case Direction.Left:
                    SetX(coordinateLeft);
                    break;
                case Direction.Right:
                    SetX(coordinateRight);
                    break;
            }
            return this as T;
        }
        public void PlaceOutside(Direction direction) => PlaceOutside<ScreenElement>(direction);
        public T MoveSprite<T>(Direction direction, int amount = 1) where T : ScreenElement {
            gameObject.MoveSprite(direction, amount);
            return this as T;
        }
        public void MoveSprite(Direction direction, int amount = 1) => MoveSprite<ScreenElement>(direction, amount);

        private T SetRotation<T>(int x, int y, int z) where T : ScreenElement {
            gameObject.transform.localRotation = Quaternion.Euler(x, y, z);
            return this as T;
        }
        public void SetRotation(int x, int y, int z) => SetRotation<ScreenElement>(x, y, z);

        public abstract T InvertColors<T>(bool val) where T : ScreenElement;
        public void InvertColors(bool val) => InvertColors<ScreenElement>(val);
    }
}