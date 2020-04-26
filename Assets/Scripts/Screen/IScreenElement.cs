using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Kaisa.Digivice {
    public interface IScreenElement {
        void Dispose();
        void SetName(string name);
        /// <summary>
        /// Sets the size (in digivice pixels) of the rectangle.
        /// </summary>
        void SetSize(int width, int height);
        void SetPosition(int x, int y);
        void SetPosition(Vector2Int pos);
        void SetActive(bool active);
    }
}