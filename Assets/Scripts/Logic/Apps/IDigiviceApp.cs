using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public interface IDigiviceApp {
        void Initialize(GameManager gm);
        void Dispose();

        void InputA();
        void InputB();
        void InputLeft();
        void InputRight();
    }
}