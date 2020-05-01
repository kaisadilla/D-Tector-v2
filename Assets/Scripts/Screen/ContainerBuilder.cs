using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class ContainerBuilder : ScreenElement {
        [SerializeField]
        private RectMask2D mask;
        public override void InvertColors(bool val) => throw new System.NotImplementedException();
        public override void SetComponentPosition(int x, int y) => throw new System.NotImplementedException();
        public void SetChildPosition(int index, int x, int y) {
            gameObject.transform.GetChild(index).gameObject.PlaceInPosition(x, y);
        }
        public void SetChildActive(int index, bool active) {
            gameObject.transform.GetChild(index).gameObject.SetActive(active);
        }
        public void SetMaskActive(bool active) => mask.enabled = active;
    }
}