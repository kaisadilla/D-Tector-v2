using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.App {
    public class Status : DigiviceApp {
        private int currentScreen = 0;

        public override void Dispose() {
            CancelInvoke();
            base.Dispose();
        }

        #region Input
        public override void InputA() {
            audioMgr.PlayButtonB();
        }
        public override void InputB() {
            audioMgr.PlayButtonB();
            CloseApp();
        }
        public override void InputLeft() {
            audioMgr.PlayButtonA();
            currentScreen = currentScreen.CircularAdd(-1, 6);
        }
        public override void InputRight() {
            audioMgr.PlayButtonA();
            currentScreen = currentScreen.CircularAdd(1, 6);
        }
        #endregion

        protected override void StartApp() {
            InvokeRepeating("DrawScreen", 0, 0.05f); //The app screen is redrawn 20 times each second.
        }

        private void DrawScreen() {
            foreach(Transform child in screenDisplay.transform) {
                Destroy(child.gameObject);
            }

            switch(currentScreen) {
                case 0:
                    screenDisplay.sprite = gm.spriteDB.status_distance;
                    string distance = gm.DistanceMgr.CurrentDistance.ToString();
                    string steps = gm.DistanceMgr.TotalSteps.ToString();
                    gm.BuildTextBox("TextDistance", screenDisplay.transform, distance, DFont.Regular, 31, 5, 0, 10, TextAnchor.UpperRight);
                    gm.BuildTextBox("TextSteps", screenDisplay.transform, steps, DFont.Regular, 31, 5, 0, 26, TextAnchor.UpperRight);
                    break;
                case 1:
                    screenDisplay.sprite = gm.spriteDB.status_level;
                    string level = gm.LoadedGame.PlayerLevel.ToString();
                    string spirits = gm.LoadedGame.SpiritPower.ToString();
                    gm.BuildTextBox("TextLevel", screenDisplay.transform, level, DFont.Regular, 31, 5, 0, 10, TextAnchor.UpperRight);
                    gm.BuildTextBox("TextSpirits", screenDisplay.transform, spirits, DFont.Regular, 31, 5, 0, 26, TextAnchor.UpperRight);
                    break;
                case 2:
                    screenDisplay.sprite = gm.spriteDB.status_victories;
                    float fVictoryPerc = gm.LoadedGame.WinPercentage;
                    int iVictoryPerc = Mathf.RoundToInt(gm.LoadedGame.WinPercentage * 100);
                    //The victory percentage is never 100% or 0%, unless the player has won or lost every single battle they've played.
                    if (iVictoryPerc == 100 && fVictoryPerc != 1f) iVictoryPerc = 99;
                    if (iVictoryPerc == 0 && fVictoryPerc != 0f) iVictoryPerc = 1;

                    string victoryPerc = iVictoryPerc.ToString();
                    string winCount = gm.LoadedGame.SpiritPower.ToString();
                    gm.BuildTextBox("TextLevel", screenDisplay.transform, victoryPerc, DFont.Regular, 24, 5, 0, 10, TextAnchor.UpperRight);
                    gm.BuildTextBox("TextSpirits", screenDisplay.transform, winCount, DFont.Regular, 31, 5, 0, 26, TextAnchor.UpperRight);
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                    int dockNumber = currentScreen - 3;
                    screenDisplay.sprite = gm.spriteDB.status_ddock[dockNumber];
                    gm.BuildDDockSprite(dockNumber, screenDisplay.transform);
                    break;
            }
        }
    }
}