using Kaisa.Digivice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kaisa.Digivice.Extensions;

namespace Kaisa.Digivice {
    public class TextBoxBuilder : MonoBehaviour, IScreenElement {
        [SerializeField] private Text textField;
        [SerializeField] private Font fRegular;
        [SerializeField] private Font fBig;

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
            }
        }
        public void SetAlignment(TextAnchor alignment) {
            textField.alignment = alignment;
        }
    }

    public enum DFont {
        Regular,
        Big
    }
}