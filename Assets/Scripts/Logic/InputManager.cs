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

        //Variables to control tapping the arrow buttons.
        private const float FIRST_TRIGGER_DELAY = 0.25f;

        //0: left, 1: right, 2: up, 3: down
        private bool[] tappingDirEnabled = new bool[] { false, false, false, false };
        private float[] tappingDirSpeed = new float[] { 0.15f, 0.15f, 0.15f, 0.15f };
        private Direction tappingDirection;
        private float msUntilNextTrigger = FIRST_TRIGGER_DELAY; //The count until the next input trigger while tapping.

        private Direction lastKeyPressed = Direction.none;

        private void Update() {
            PointerDown(tappingDirection);
        }

        public void OnInputA() {
            if (!inhibitInput) {
                gm.logicMgr.InputA();
                lastKeyPressed = Direction.Down;
            }
        }

        public void OnInputB() {
            if (!inhibitInput) {
                gm.logicMgr.InputB();
                lastKeyPressed = Direction.Up;
            }
        }

        public void OnInputLeft() {
            if (!inhibitInput) {
                gm.logicMgr.InputLeft();
                lastKeyPressed = Direction.Left;
            }
        }

        public void OnInputRight() {
            if (!inhibitInput) {
                gm.logicMgr.InputRight();
                lastKeyPressed = Direction.Right;
            }
        }

        /// <summary>
        /// Consumes the key being stored and returns true if it's any of the keys passed as a parameter.
        /// </summary>
        public bool ConsumeLastKey(params Direction[] checkDir) {
            Debug.Log($"Consumed key! LastKey: {lastKeyPressed}");
            foreach(Direction d in checkDir) {
                if (lastKeyPressed == d) {
                    Debug.Log("Returning true!");
                    lastKeyPressed = Direction.none;
                    return true;
                }
            }
            return false;
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

        public Direction GetKeyBeingTapped() => tappingDirection;

        public void PointerDown(Direction dir) {
            if (tappingDirection == Direction.none) return; //Don't do anything if nothing is being tapped.
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
            switch(dir) {
                case "LEFT":
                    tappingDirection = Direction.Left;
                    break;
                case "RIGHT":
                    tappingDirection = Direction.Right;
                    break;
                case "UP":
                    tappingDirection = Direction.Up;
                    break;
                case "DOWN":
                    tappingDirection = Direction.Down;
                    break;
            }
        }

        public void TapUp(string dir) {
            //If the key being untapped is not the key currently being registered as tapped, ignore it.
            if (dir == "LEFT" && tappingDirection != Direction.Left) return;
            if (dir == "RIGHT" && tappingDirection != Direction.Right) return;
            if (dir == "UP" && tappingDirection != Direction.Up) return;
            if (dir == "DOWN" && tappingDirection != Direction.Down) return;
            tappingDirection = Direction.none;
            msUntilNextTrigger = FIRST_TRIGGER_DELAY;
        }
    }
}

//TODO: Fix that, while tapping to go left or right multiple times, when the user stops tapping, an extra input is received (the regular input).
//One way would be to not have buttons do any action when pressed, but instead do it on TapUp() only if they haven't done any action in PointerDown() yet.