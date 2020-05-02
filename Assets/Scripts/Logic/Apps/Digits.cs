using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.App {
    public class Digits : DigiviceApp {
        new protected static string appName = "digits";

        private int inputStatus = 0; //0: inputting, 1: ok?, 2: error, 3: success

        //UI
        private RectangleBuilder[] underscores = new RectangleBuilder[5];
        private TextBoxBuilder selectedInputDisplay;
        private TextBoxBuilder currentInputDisplay;

        //Code info
        private byte selectedInput = 0x41;
        private Stack<byte> currentInput = new Stack<byte>();
        private bool InputIsEmpty => (currentInput.Count == 0);
        private bool InputIsFull => (currentInput.Count == 5);
        private string SelectedInputString => ((char)selectedInput).ToString();
        private string CurrentInputString => Encoding.ASCII.GetString(currentInput.Reverse().ToArray());

        #region Input
        public override void InputA() {
            if (!InputIsFull) {
                audioMgr.PlayButtonA();
                currentInput.Push(selectedInput);
                if (InputIsFull) inputStatus = 1; //If this byte made 5 characters.
            }
            else if (inputStatus == 1) {
                audioMgr.PlayButtonA();
                CheckCode();
            }
            else if (inputStatus == 2) {
                audioMgr.PlayButtonA();
                currentInput.Pop();
                inputStatus = 0;
            }
        }
        public override void InputB() {
            if (InputIsEmpty) {
                audioMgr.PlayButtonB();
                CloseApp();
            }
            else if (!InputIsEmpty) {
                audioMgr.PlayButtonB();
                currentInput.Pop();
                inputStatus = 0;
            }
        }
        public override void InputLeft() {
            if (!InputIsFull) {
                audioMgr.PlayButtonA();
                NavigateInput(Direction.Left);
            }
            else if (InputIsFull && inputStatus == 2) {
                audioMgr.PlayButtonA();
                currentInput.Pop();
                inputStatus = 0;
            }
        }
        public override void InputRight() {
            if (!InputIsFull) {
                audioMgr.PlayButtonA();
                NavigateInput(Direction.Right);
            }
            else if (InputIsFull && inputStatus == 2) {
                audioMgr.PlayButtonA();
                currentInput.Pop();
                inputStatus = 0;
            }
        }
        #endregion

        protected override void StartApp() {
            for (int i = 0; i < 5; i++) {
                underscores[i] = gm.BuildRectangle($"Underscore{i}", screenDisplay.transform, 5, 1, 2 + (6 * i), 25);
            }
            selectedInputDisplay = gm.BuildTextBox("Input", screenDisplay.transform, "A", DFont.Big, 6, 8, 14, 8);
            currentInputDisplay = gm.BuildTextBox("CurrentCode", screenDisplay.transform, "", DFont.Big, 30, 8, 2, 17);
            UpdateScreen();
            gm.SetTappingEnabled(Direction.Left, true, 0.1f);
            gm.SetTappingEnabled(Direction.Right, true, 0.1f);
        }
        protected override void CloseApp(Screen goToMenu = Screen.MainMenu) {
            gm.SetTappingEnabled(Direction.Left, false);
            gm.SetTappingEnabled(Direction.Right, false);
            base.CloseApp(goToMenu);
        }
        private void Update() {
            UpdateScreen();
        }
        private void UpdateScreen() {
            currentInputDisplay.Text = CurrentInputString;

            for (int i = 0; i < underscores.Length; i++) {
                if (i == currentInput.Count()) {
                    if (underscores[i].GetFlickPeriod() == 0) {
                        underscores[i].SetFlickPeriod(0.4f, false);
                    }
                }
                else {
                    underscores[i].SetFlickPeriod(0f);
                }
            }

            if (!InputIsFull) {
                selectedInputDisplay.SetActive(true);
                selectedInputDisplay.Text = SelectedInputString;

                screenDisplay.sprite = gm.spriteDB.arrows;
            }
            else {
                selectedInputDisplay.SetActive(false);

                if (inputStatus == 1) {
                    screenDisplay.sprite = gm.spriteDB.digits_ok;
                }
                else if (inputStatus == 2) {
                    screenDisplay.sprite = gm.spriteDB.digits_error;
                }
            }

        }

        private void NavigateInput(Direction dir) {
            if(!InputIsFull) {
                if (dir == Direction.Left) {
                    if (selectedInput == 0x41) selectedInput = 0x39;
                    else if (selectedInput == 0x30) selectedInput = 0x5A;
                    else selectedInput--;
                }
                else {
                    if (selectedInput == 0x5A) selectedInput = 0x30;
                    else if (selectedInput == 0x39) selectedInput = 0x41;
                    else selectedInput++;
                }
            }
        }

        private void CheckCode() {
            if (gm.DatabaseMgr.TryGetDigimonFromCode(CurrentInputString, out string digimon)) {
                gm.logicMgr.SetDigimonUnlocked(digimon, true);
                gm.logicMgr.SetDigimonCodeUnlocked(digimon, true);
                CloseApp();

                gm.EnqueueAnimation(gm.screenMgr.ASummonDigimon(digimon));
                gm.EnqueueAnimation(gm.screenMgr.AUnlockDigimon(digimon));
                gm.EnqueueAnimation(gm.screenMgr.ACharHappy());
            }
            else {
                inputStatus = 2;
            }
        }
    }
}