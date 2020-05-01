using Kaisa.Digivice.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class ContainerBuilder : ScreenElement {
        [SerializeField]
        private RectMask2D mask;
        public override T InvertColors<T>(bool val) => throw new System.NotImplementedException();
        public override T SetComponentPosition<T>(int x, int y) => throw new System.NotImplementedException();
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
            if (val) background.color = Constants.ACTIVE_COLOR;
            else background.color = Constants.BACKGROUND_COLOR;
            return this;
        }
    }
}