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

        public new int Width {
            get => (int)(textField.rectTransform.sizeDelta.x / Constants.PIXEL_SIZE);
        }

        public override T InvertColors<T>(bool val) {
            if(val) {
                textField.color = Constants.BACKGROUND_COLOR;
                background.color = Constants.ACTIVE_COLOR;
            }
            else {
                textField.color = Constants.ACTIVE_COLOR;
                background.color = Constants.BACKGROUND_COLOR;
            }
            return this as T;
        }

        //Specific methods:
        public string Text {
            get => textField.text;
            set => textField.text = value;
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

        public override T SetComponentPosition<T>(int x, int y) {
            textField.gameObject.PlaceInPosition(x, y);
            return this as T;
        }
        public TextBoxBuilder SetComponentOffset(Vector2 offsetMin, Vector2 offsetMax) {
            textField.rectTransform.offsetMin = new Vector2(offsetMin.x * Constants.PIXEL_SIZE, offsetMin.y * Constants.PIXEL_SIZE);
            textField.rectTransform.offsetMax = new Vector2(offsetMax.x * Constants.PIXEL_SIZE, offsetMax.y * Constants.PIXEL_SIZE);
            return this;
        }
    }

    public enum DFont {
        Regular,
        Big,
        Small
    }
}