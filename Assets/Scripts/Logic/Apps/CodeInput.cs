using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kaisa.Digivice.App {
    public class CodeInput : DigiviceApp {
        private bool submitError = false; //if true, a wrong code will submit the default digimon.
        private int inputStatus = 0; //0: inputting, 1: ok?, 2: error, 3: success
        public string ReturnedDigimon { get; private set; }

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
        public override void InputLeftDown() {
            StartNavigation(Direction.Left);
        }
        public override void InputRightDown() {
            StartNavigation(Direction.Right);
        }
        public override void InputLeftUp() {
            StopNavigation();
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
        public override void InputRightUp() {
            StopNavigation();
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
        protected override IEnumerator AutoNavigateDir(Direction dir) {
            yield return new WaitForSeconds(0.25f);
            while(true) {
                Debug.Log("NAVIG");
                yield return new WaitForSeconds(0.1f);
                audioMgr.PlayButtonA();
                NavigateInput(dir);
            }
        }
        #endregion

        protected override void StartApp() {
            if (appArgs.Length >= 1 && appArgs[0] == "true") submitError = true;
            for (int i = 0; i < 5; i++) {
                underscores[i] = ScreenElement.BuildRectangle($"Underscore{i}", screenDisplay.transform).SetSize(5, 1).SetPosition(2 + (6 * i), 25);
            }
            selectedInputDisplay = ScreenElement.BuildTextBox("Input", screenDisplay.transform, DFont.Big).SetSize(6, 8).SetPosition(14, 8);
            currentInputDisplay = ScreenElement.BuildTextBox("CurrentCode", screenDisplay.transform, DFont.Big).SetSize(30, 8).SetPosition(2, 17);
            UpdateScreen();
        }
        private void Update() {
            UpdateScreen();
        }
        private void UpdateScreen() {
            currentInputDisplay.Text = CurrentInputString;

            for (int i = 0; i < underscores.Length; i++) {
                if (i == currentInput.Count()) {
                    if (underscores[i].FlickPeriod == 0) {
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
                ReturnedDigimon = digimon;
                CloseApp();
            }
            else {
                if(submitError) {
                    ReturnedDigimon = Constants.DEFAULT_DIGIMON;
                    CloseApp();
                }
                inputStatus = 2;
            }
        }
    }
}