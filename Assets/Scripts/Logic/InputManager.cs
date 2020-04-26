using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public class InputManager : MonoBehaviour {
        private GameManager gm;
        private DebugManager debug;
        private LogicManager logicMgr;

        public bool inhibitInput = false;
        public void AssignManagers(GameManager gm) {
            this.gm = gm;
            this.logicMgr = gm.logicMgr;
            this.debug = gm.debug;
        }

        //Variables to control tapping the arrow buttons.
        private const int FIRST_TRIGGER_MS_COUNT = 1000;
        private const int NEXT_TRIGGER_MS_COUNT = 150;

        private bool tapping = false;
        private Direction tapDir;
        private float msUntilNextTrigger = FIRST_TRIGGER_MS_COUNT; //The count until the next input trigger while tapping.

        private void Update() {
            if(tapping) {
                PointerDown(tapDir);
            }
        }

        public void OnInputA() {
            if(!inhibitInput) logicMgr.InputA();
        }

        public void OnInputB() {
            if (!inhibitInput) logicMgr.InputB();
        }

        public void OnInputLeft() {
            if (!inhibitInput) logicMgr.InputLeft();
        }

        public void OnInputRight() {
            if (!inhibitInput) logicMgr.InputRight();
        }

        public void PointerDown(Direction dir) {
            msUntilNextTrigger -= Time.deltaTime * 1000;

            if(msUntilNextTrigger <= 0) {
                msUntilNextTrigger = NEXT_TRIGGER_MS_COUNT;
                switch (dir) {
                    case Direction.Left:
                        OnInputLeft();
                        break;
                    case Direction.Right:
                        OnInputRight();
                        break;
                }
            }
        }

        public void TapDown(string dir) {
            tapping = true;
            if (dir == "LEFT") {
                tapDir = Direction.Left;
            }
            else {
                tapDir = Direction.Right;
            }
        }

        public void TapUp() {
            tapping = false;
            msUntilNextTrigger = FIRST_TRIGGER_MS_COUNT;
        }
    }
}

//TODO: Fix that, while tapping to go left or right multiple times, when the user stops tapping, an extra input is received (the regular input).
//One way would be to not have buttons do any action when pressed, but instead do it on TapUp() only if they haven't done any action in PointerDown() yet.