using Kaisa.Digivice.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class ContainerBuilder : ScreenElement {
        [SerializeField]
        private RectMask2D mask;
        private void Awake() {
            background.color = Preferences.BackgroundColor;
        }
        protected override void BaseInvertColors(bool val) => throw new System.NotImplementedException();

        //Chained base methods:
        public ContainerBuilder Center() {
            BaseCenter();
            return this;
        }
        public ContainerBuilder InvertColors(bool val) {
            BaseInvertColors(val);
            return this;
        }
        public ContainerBuilder Move(Direction direction, int amount = 1) {
            BaseMove(direction, amount);
            return this;
        }
        public ContainerBuilder PlaceOutside(Direction direction) {
            BasePlaceOutside(direction);
            return this;
        }
        public ContainerBuilder SetActive(bool active) {
            BaseSetActive(active);
            return this;
        }
        public ContainerBuilder SetPosition(int x, int y) {
            BaseSetPosition(x, y);
            return this;
        }
        public ContainerBuilder SetPosition(Vector2Int pos) {
            BaseSetPosition(pos);
            return this;
        }
        public ContainerBuilder SetSize(int width, int length) {
            BaseSetSize(width, length);
            return this;
        }
        public ContainerBuilder SetTransparent(bool val) {
            BaseSetTransparent(val);
            return this;
        }
        public ContainerBuilder SetX(int x) {
            BaseSetX(x);
            return this;
        }
        public ContainerBuilder SetY(int y) {
            BaseSetY(y);
            return this;
        }

        //Specific methods:
        public ContainerBuilder SetChildPosition(int index, int x, int y) {
            gameObject.transform.GetChild(index).gameObject.PlaceInPosition(x, y);
            return this;
        }
        public ContainerBuilder SetChildActive(int index, bool active) {
            gameObject.transform.GetChild(index).gameObject.SetActive(active);
            return this;
        }
        public ContainerBuilder SetMaskActive(bool active) {
            mask.enabled = active;
            return this;
        }

        /// <summary>
        /// Returns the Builder of the ScreenElement child at the index specified. Fails if the index does not exist, or if the child found is not a ScreenElement.
        /// </summary>
        /// <param name="index">The index of the child.</param>
        /// <returns></returns>
        public ScreenElement GetChildBuilder(int index) {
            return gameObject.transform.GetChild(index).GetComponent<ScreenElement>();
        }

        public ContainerBuilder SetBackgroundBlack(bool val) {
            if (val) background.color = Preferences.ActiveColor;
            else background.color = Preferences.BackgroundColor;
            return this;
        }
    }
}