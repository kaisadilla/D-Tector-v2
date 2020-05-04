using System.Diagnostics.Tracing;
using UnityEngine;

namespace Kaisa.Digivice {
    public class InputManager : MonoBehaviour {
        private GameManager gm;
        private DebugManager debug;

        public bool inhibitInput = false;
        public void AssignManagers(GameManager gm) {
            this.gm = gm;
            this.debug = gm.debug;
        }

        public void OnInputA() {
            if (!inhibitInput) {
                gm.logicMgr.InputA();
            }
        }

        public void OnInputB() {
            if (!inhibitInput) {
                gm.logicMgr.InputB();
            }
        }

        public void OnInputLeft() {
            if (!inhibitInput) {
                gm.logicMgr.InputLeft();
            }
        }

        public void OnInputRight() {
            if (!inhibitInput) {
                gm.logicMgr.InputRight();
            }
        }

        public void OnInputADown() {
            if (!inhibitInput) {
                gm.logicMgr.InputADown();
            }
        }
        public void OnInputBDown() {
            if (!inhibitInput) {
                gm.logicMgr.InputBDown();
            }
        }
        public void OnInputLeftDown() {
            if (!inhibitInput) {
                gm.logicMgr.InputLeftDown();
            }
        }
        public void OnInputRightDown() {
            if (!inhibitInput) {
                gm.logicMgr.InputRightDown();
            }
        }
        public void OnInputAUp() {
            if (!inhibitInput) {
                gm.logicMgr.InputAUp();
            }
        }
        public void OnInputBUp() {
            if (!inhibitInput) {
                gm.logicMgr.InputBUp();
            }
        }
        public void OnInputLeftUp() {
            if (!inhibitInput) {
                gm.logicMgr.InputLeftUp();
            }
        }
        public void OnInputRightUp() {
            if (!inhibitInput) {
                gm.logicMgr.InputRightUp();
            }
        }
    }
}

//TODO: Fix that, while tapping to go left or right multiple times, when the user stops tapping, an extra input is received (the regular input).
//One way would be to not have buttons do any action when pressed, but instead do it on TapUp() only if they haven't done any action in PointerDown() yet.