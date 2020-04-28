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
            get => (int)(textField.rectTransform.sizeDelta.x / Constants.PixelSize);
        }

        public override void InvertColors(bool val) {
            if(val) {
                textField.color = Constants.backgroundColor;
                background.color = Constants.activeColor;
            }
            else {
                textField.color = Constants.activeColor;
                background.color = Constants.backgroundColor;
            }
        }

        //Specific methods:
        public string Text {
            get => textField.text;
            set => textField.text = value;
        }
        public void SetFont(DFont font) {
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
        }
        public void SetAlignment(TextAnchor alignment) {
            textField.alignment = alignment;
        }

        public void SetFitSizeToContent(bool val) {
            fitter.enabled = val;
            background.rectTransform.sizeDelta = new Vector2(Width, background.rectTransform.sizeDelta.y);
        }
    }

    public enum DFont {
        Regular,
        Big,
        Small
    }
}