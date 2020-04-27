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
            fitter.gameObject.SetActive(val);
        }
    }

    public enum DFont {
        Regular,
        Big,
        Small
    }
}