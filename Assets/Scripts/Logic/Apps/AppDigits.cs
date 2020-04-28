using Kaisa.Digivice;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class AppDigits : MonoBehaviour, IDigiviceApp {
        [Header("UI Elements")]
        [SerializeField]
        private Image screenDisplay;

        private GameManager gm;
        private AudioManager audioMgr;
        //UI
        private RectangleBuilder[] underscores = new RectangleBuilder[5];
        private TextBoxBuilder selectedInputDisplay;
        private TextBoxBuilder currentInputDisplay;
        private int inputStatus = 0; //0: inputting, 1: ok?, 2: error, 3: success
        //Code info
        private byte selectedInput = 0x41;
        private Stack<byte> currentInput = new Stack<byte>();
        private bool InputIsEmpty => (currentInput.Count == 0);
        private bool InputIsFull => (currentInput.Count == 5);
        private string SelectedInputString => ((char)selectedInput).ToString();
        private string CurrentInputString => Encoding.ASCII.GetString(currentInput.Reverse().ToArray());

        //App Loader
        public static AppDigits LoadApp(GameManager gm) {
            GameObject appGO = Instantiate(gm.pAppDigits, gm.mainScreen.transform);
            AppDigits appMap = appGO.GetComponent<AppDigits>();
            appMap.Initialize(gm);
            return appMap;
        }
        //IDigiviceApp Methods:
        public void Dispose() => Destroy(gameObject);
        public void Initialize(GameManager gm) {
            this.gm = gm;
            audioMgr = gm.audioMgr;
            StartApp();
        }
        public void InputA() {
            if(!InputIsFull) {
                audioMgr.PlayButtonA();
                currentInput.Push(selectedInput);
                if (InputIsFull) inputStatus = 1; //If this byte made 5 characters.
            }
            else if (inputStatus == 1) {
                audioMgr.PlayButtonA();
                CheckCode();
            }
        }
        public void InputB() {
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
        public void InputLeft() {
            if(!InputIsFull) {
                audioMgr.PlayButtonA();
                NavigateInput(Direction.Left);
            }
        }
        public void InputRight() {
            if (!InputIsFull) {
                audioMgr.PlayButtonA();
                NavigateInput(Direction.Right);
            }
        }

        private void StartApp() {
            for (int i = 0; i < 5; i++) {
                underscores[i] = gm.CreateRectangle($"Underscore{i}", screenDisplay.transform, 5, 1, 2 + (6 * i), 25);
            }
            selectedInputDisplay = gm.CreateTextBox("Input", screenDisplay.transform, "A", DFont.Big, 6, 8, 14, 8);
            currentInputDisplay = gm.CreateTextBox("CurrentCode", screenDisplay.transform, "", DFont.Big, 30, 8, 2, 17);
            UpdateScreen();
        }
        private void CloseApp() {
            gm.logicMgr.FinalizeApp(false);
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
            //TODO: Continue here.
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
    }
}