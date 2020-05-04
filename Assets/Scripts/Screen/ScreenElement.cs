using Kaisa.Digivice.Extensions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public abstract class ScreenElement : MonoBehaviour {
        #region behavior
        public Image background;
        /// <summary>
        /// Returns the Width, in pixels, of the Element.
        /// </summary>
        public int Width {
            get => (int)(gameObject.GetComponent<RectTransform>().sizeDelta.x / Constants.PIXEL_SIZE);
        }
        /// <summary>
        /// Returns the Height, in pixels, of the Element.
        /// </summary>
        public int Height {
            get => (int)(gameObject.GetComponent<RectTransform>().sizeDelta.y / Constants.PIXEL_SIZE);
        }
        /// <summary>
        /// Returns the x and y position, in pixels, of the Element.
        /// </summary>
        public Vector2Int Position {
            get {
                Vector2 pos = gameObject.GetComponent<RectTransform>().anchoredPosition;
                return new Vector2Int((int)(pos.x / Constants.PIXEL_SIZE), (int)(-pos.y / Constants.PIXEL_SIZE));
            }
        }
        /// <summary>
        /// Returns true if the Element is active.
        /// </summary>
        public bool Active => gameObject.activeSelf;

        //Methods:

        /// <summary>
        /// Sets the name of the Element in the Unity Editor.
        /// </summary>
        public void SetName(string name) {
            gameObject.name = name;
        }
        /// <summary>
        /// Destroys this Element.
        /// </summary>
        public virtual void Dispose() => Destroy(gameObject);
        /// <summary>
        /// Sets whether this Element is visible or not.
        /// </summary>
        /// 
        protected void BaseSetActive(bool active) {
            gameObject.SetActive(active);
        }
        /// <summary>
        /// Sets whether the Element's background is transparent or not.
        /// </summary>
        /// <param name="val">The value of transparency.</param>
        protected void BaseSetTransparent(bool val) {
            background.enabled = !val;
        }
        /// <summary>
        /// Sets whether the colors of the Element have been inverted.
        /// </summary>
        /// <param name="val"></param>
        protected abstract void BaseInvertColors(bool val);
        /// <summary>
        /// Sets the size in pixels of this Element.
        /// </summary>
        protected virtual void BaseSetSize(int width, int height) {
            width *= Constants.PIXEL_SIZE;
            height *= Constants.PIXEL_SIZE;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
        /// <summary>
        /// Sets the position in pixels of this Element.
        /// </summary>
        protected void BaseSetPosition(int x, int y) {
            gameObject.PlaceInPosition(x, y);
        }
        /// <summary>
        /// Sets the position in pixels of this Element.
        /// </summary>
        protected void BaseSetPosition(Vector2Int pos) {
            BaseSetPosition(pos.x, pos.y);
        }
        /// <summary>
        /// Sets the position in pixels of this Element in the x axis.
        /// </summary>
        protected void BaseSetX(int x) {
            BaseSetPosition(x, Position.y);
        }
        /// <summary>
        /// Sets the position in pixels of this Element in the y axis.
        /// </summary>
        protected void BaseSetY(int y) {
            BaseSetPosition(Position.x, y);
        }
        /// <summary>
        /// Places the Element at the center of the Screen.
        /// </summary>
        protected void BaseCenter() {
            int x = Mathf.RoundToInt((Constants.SCREEN_WIDTH - Width) / 2f);
            int y = Mathf.RoundToInt((Constants.SCREEN_HEIGHT - Width) / 2f);
            BaseSetPosition(x, y);
        }
        /// <summary>
        /// Places the Element outside of the Screen.
        /// </summary>
        /// <param name="direction">The side of the screen at which the Element will be placed.</param>
        protected void BasePlaceOutside(Direction direction) {
            int coordinateUp = -Height;
            int coordinateDown = Constants.SCREEN_HEIGHT;
            int coordinateLeft = -Width;
            int coordinateRight = Constants.SCREEN_WIDTH;

            switch (direction) {
                case Direction.Up:
                    BaseSetY(coordinateUp);
                    break;
                case Direction.Down:
                    BaseSetY(coordinateDown);
                    break;
                case Direction.Left:
                    BaseSetX(coordinateLeft);
                    break;
                case Direction.Right:
                    BaseSetX(coordinateRight);
                    break;
            }
        }
        /// <summary>
        /// Moves the Element a number of pixels in a direction.
        /// </summary>
        /// <param name="direction">The direction of the movement.</param>
        /// <param name="amount">The amount of Pixels that will be moved.</param>
        protected void BaseMove(Direction direction, int amount = 1) {
            gameObject.MoveSprite(direction, amount);
        }
        /*
        private T SetRotation<T>(int x, int y, int z) where T : ScreenElement {
            gameObject.transform.localRotation = Quaternion.Euler(x, y, z);
            return this as T;
        }
        public void SetRotation(int x, int y, int z) => SetRotation<ScreenElement>(x, y, z);
        */
        #endregion

        #region creator methods
        public static GameObject pContainer;
        public static GameObject pSolidSprite;
        public static GameObject pRectangle;
        public static GameObject pTextBox;

        public static void Initialize(GameObject container, GameObject solidSprite, GameObject rectangle, GameObject textBox) {
            pContainer = container;
            pSolidSprite = solidSprite;
            pRectangle = rectangle;
            pTextBox = textBox;
        }

        public static SpriteBuilder BuildSprite(string name, Transform parent) {
            GameObject go = Instantiate(pSolidSprite, parent);
            SpriteBuilder goClass = go.GetComponent<SpriteBuilder>();
            goClass.SetName(name);
            return goClass;
        }
        public static RectangleBuilder BuildRectangle(string name, Transform parent) {
            GameObject go = Instantiate(pRectangle, parent);
            RectangleBuilder goClass = go.GetComponent<RectangleBuilder>();
            goClass.SetName(name);
            return goClass;
        }
        public static TextBoxBuilder BuildTextBox(string name, Transform parent, DFont font) {
            GameObject go = Instantiate(pTextBox, parent);
            TextBoxBuilder goClass = go.GetComponent<TextBoxBuilder>();
            goClass.SetName(name);
            goClass.SetFont(font);
            return goClass;
        }
        public static ContainerBuilder BuildContainer(string name, Transform parent, bool transparent = true) {
            GameObject go = Instantiate(pContainer, parent);
            ContainerBuilder goClass = go.GetComponent<ContainerBuilder>();
            goClass.SetName(name);
            goClass.SetTransparent(transparent);
            return goClass;
        }
        public static Transform BuildBackground(Transform parent) {
            RectangleBuilder goClass = BuildRectangle("Parent", parent).SetSize(32, 32).SetColor(false);
            return goClass.transform;
        }
        public static ContainerBuilder BuildStatSign(string message, Transform parent) {
            ContainerBuilder cbSign = BuildContainer("Sign", parent, false).SetBackgroundBlack(true).SetSize(32, 17).SetPosition(0, 15);
            TextBoxBuilder sbMessage = BuildTextBox("Sign", cbSign.transform, DFont.Small)
                .SetText(message)
                .SetSize(28, 5)
                .SetPosition(2, 2)
                .InvertColors(true);
            TextBoxBuilder sbValue = BuildTextBox("Sign", cbSign.transform, DFont.Small)
                .SetSize(28, 5)
                .SetPosition(2, 10)
                .SetAlignment(TextAnchor.UpperRight)
                .InvertColors(true);
            return cbSign;
        }
        #endregion
    }
}