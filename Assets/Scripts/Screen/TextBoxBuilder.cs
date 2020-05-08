using Kaisa.Digivice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kaisa.Digivice.Extensions;

namespace Kaisa.Digivice {
    public class TextBoxBuilder : ScreenElement {
        [SerializeField] private Text textField;
        [SerializeField] private ContentSizeFitter fitter;
        [SerializeField] private Font fRegular;
        [SerializeField] private Font fBig;
        [SerializeField] private Font fSmall;

        private void Awake() {
            background.color = Preferences.BackgroundColor;
            textField.color = Preferences.ActiveColor;
        }

        public new int Width {
            get => (int)(textField.rectTransform.sizeDelta.x / Constants.PIXEL_SIZE);
        }

        //Overrides:
        //TODO: This won't work currently as sprites are always black instead of white.
        protected override void BaseInvertColors(bool val) {
            if (val) {
                textField.color = Preferences.BackgroundColor;
                background.color = Preferences.ActiveColor;
            }
            else {
                textField.color = Preferences.ActiveColor;
                background.color = Preferences.BackgroundColor;
            }
        }
        protected override void BaseSetSize(int width, int height) {
            base.BaseSetSize(width, height);
            SetComponentSize(width, height);
        }
        //Chained base methods:
        public TextBoxBuilder Center() {
            BaseCenter();
            return this;
        }
        public TextBoxBuilder InvertColors(bool val) {
            BaseInvertColors(val);
            return this;
        }
        public TextBoxBuilder Move(Direction direction, int amount = 1) {
            BaseMove(direction, amount);
            return this;
        }
        public TextBoxBuilder PlaceOutside(Direction direction) {
            BasePlaceOutside(direction);
            return this;
        }
        public TextBoxBuilder SetActive(bool active) {
            BaseSetActive(active);
            return this;
        }
        public TextBoxBuilder SetPosition(int x, int y) {
            BaseSetPosition(x, y);
            return this;
        }
        public TextBoxBuilder SetPosition(Vector2Int pos) {
            BaseSetPosition(pos);
            return this;
        }
        public TextBoxBuilder SetSize(int width, int length) {
            BaseSetSize(width, length);
            return this;
        }
        public TextBoxBuilder SetTransparent(bool val) {
            BaseSetTransparent(val);
            return this;
        }
        public TextBoxBuilder SetX(int x) {
            BaseSetX(x);
            return this;
        }
        public TextBoxBuilder SetY(int y) {
            BaseSetY(y);
            return this;
        }

        //Specific methods:
        public string Text {
            get => textField.text;
            set => textField.text = value;
        }
        public TextBoxBuilder SetText(string text) {
            Text = text;
            return this;
        }
        public TextBoxBuilder SetFont(DFont font) {
            switch(font) {
                case DFont.Regular:
                    textField.font = fRegular;
                    break;
                case DFont.Big:
                    textField.font = fBig;
                    break;
                case DFont.Small:
                    textField.font = fSmall;
                    break;
            }
            return this;
        }
        public TextBoxBuilder SetAlignment(TextAnchor alignment) {
            textField.alignment = alignment;
            return this;
        }

        public TextBoxBuilder SetFitSizeToContent(bool val) {
            fitter.enabled = val;
            background.rectTransform.sizeDelta = new Vector2(Width, background.rectTransform.sizeDelta.y);
            return this;
        }
        public TextBoxBuilder SetComponentPosition(int x, int y) {
            textField.gameObject.PlaceInPosition(x, y);
            return this;
        }
        /// <summary>
        /// Sets the size of the component sprite, without resizing the Sprite Builder.
        /// </summary>
        public TextBoxBuilder SetComponentSize(int width, int height) {
            width *= Constants.PIXEL_SIZE;
            height *= Constants.PIXEL_SIZE;
            textField.rectTransform.sizeDelta = new Vector2(width, height);
            return this;
        }
    }

    public enum DFont {
        Regular,
        Big,
        Small
    }
}