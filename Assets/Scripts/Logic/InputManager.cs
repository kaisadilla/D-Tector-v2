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

        //Variables to control tapping the arrow buttons.
        private const float FIRST_TRIGGER_DELAY = 0.25f;

        private bool tapping = false;
        //0: left, 1: right, 2: up, 3: down
        private bool[] tappingDirEnabled = new bool[] { false, false, false, false };
        private float[] tappingDirSpeed = new float[] { 0.15f, 0.15f, 0.15f, 0.15f };
        private Direction tapDir;
        private float msUntilNextTrigger = FIRST_TRIGGER_DELAY; //The count until the next input trigger while tapping.

        private void Update() {
            if(tapping) {
                PointerDown(tapDir);
            }
        }

        public void OnInputA() {
            if(!inhibitInput) gm.logicMgr.InputA();
        }

        public void OnInputB() {
            if (!inhibitInput) gm.logicMgr.InputB();
        }

        public void OnInputLeft() {
            if (!inhibitInput) gm.logicMgr.InputLeft();
        }

        public void OnInputRight() {
            if (!inhibitInput) gm.logicMgr.InputRight();
        }

        public bool GetTappingEnabled(Direction dir) {
            return tappingDirEnabled[(int)dir];
        }
        public float GetTappingSpeed(Direction dir) {
            return tappingDirSpeed[(int)dir];
        }
        public void SetTappingEnabled(Direction dir, bool enabled, float speed = 0.15f) {
            tappingDirEnabled[(int)dir] = enabled;
            tappingDirSpeed[(int)dir] = speed;
        }

        public void PointerDown(Direction dir) {
            if (!GetTappingEnabled(dir)) return; //Don't do anything if tapping is not enabled.

            msUntilNextTrigger -= Time.deltaTime;

            if(msUntilNextTrigger <= 0) {
                msUntilNextTrigger = GetTappingSpeed(dir);
                switch (dir) {
                    case Direction.Left:
                        OnInputLeft();
                        break;
                    case Direction.Right:
                        OnInputRight();
                        break;
                    case Direction.Up:
                        OnInputB();
                        break;
                    case Direction.Down:
                        OnInputA();
                        break;
                }
            }
        }
        //Note that A is "DOWN" and B is "UP".
        public void TapDown(string dir) {
            tapping = true;
            switch(dir) {
                case "LEFT":
                    tapDir = Direction.Left;
                    break;
                case "RIGHT":
                    tapDir = Direction.Right;
                    break;
                case "UP":
                    tapDir = Direction.Up;
                    break;
                case "DOWN":
                    tapDir = Direction.Down;
                    break;
            }
        }

        public void TapUp() {
            tapping = false;
            msUntilNextTrigger = FIRST_TRIGGER_DELAY;
        }
    }
}

//TODO: Fix that, while tapping to go left or right multiple times, when the user stops tapping, an extra input is received (the regular input).
//One way would be to not have buttons do any action when pressed, but instead do it on TapUp() only if they haven't done any action in PointerDown() yet.