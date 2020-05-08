using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice.App {
    public class Finder : DigiviceApp {
        public override void InputB() {
            if (result == 0) {
                audioMgr.PlayButtonB();
                CloseApp(Screen.GamesMenu);
            }
        }
        public override void InputADown() {
            //Remove error screen.
            if (result == 2) {
                audioMgr.PlayButtonA();
                result = 0;
            }
            else if (result != 3) {
                audioMgr.PlayButtonA();
                tries = 0;
                StartLoadingBar();
            }
        }
        public override void InputAUp() {
            if (result != 3) {
                result = 0;
                StopLoadingBar();
            }
        }
        protected override void StartApp() {
            InvokeRepeating("DisplayPressASprite", 0f, 0.75f);
        }

        private bool state = false;
        private void DisplayPressASprite() {
            if(result == 0) {
                if (state) SetScreen(gm.spriteDB.pressAButton[1]);
                else SetScreen(gm.spriteDB.pressAButton[0]);
                state = !state;
            }
        }

        SpriteBuilder sbHourglass;
        RectangleBuilder rbBlackScreen;
        SpriteBuilder sbLoading;
        Coroutine loadingCoroutine;

        private int tries = 0;
        private int result = 0; //0: nothing, 1: loading, 2: failure, 3: succeed.
        private void StartLoadingBar() {
            result = 1;
            rbBlackScreen = ScreenElement.BuildRectangle("BlackScreen0", Parent).SetSize(32,32);
            sbLoading = ScreenElement.BuildSprite("Loading", Parent).SetSprite(gm.spriteDB.loading).PlaceOutside(Direction.Up);
            loadingCoroutine = StartCoroutine(AnimateLoadingBar());
        }
        private void StopLoadingBar() {
            if(loadingCoroutine != null) {
                StopCoroutine(loadingCoroutine);
                if (rbBlackScreen != null) rbBlackScreen.Dispose();
                if (sbLoading != null) sbLoading.Dispose();
                if (sbHourglass != null) sbHourglass.Dispose();
            }
        }

        private IEnumerator AnimateLoadingBar() {
            sbHourglass = ScreenElement.BuildSprite("Hourglass", Parent).SetSprite(gm.spriteDB.hourglass);
            yield return new WaitForSeconds(0.5f);
            sbHourglass.Dispose();

            while (result == 1) {
                if (tries == 5) {
                    result = 2;
                    break;
                }
                int thisRoundRNG = Random.Range(0, 10);
                VisualDebug.WriteLine($"RNG: {thisRoundRNG}");
                if (thisRoundRNG == 0) {
                    result = 3;
                    break;
                }

                sbLoading.PlaceOutside(Direction.Up);
                for (int i = 0; i < 64; i++) {
                    sbLoading.Move(Direction.Down);
                    yield return new WaitForSeconds(1.75f / 64);
                }
                tries++;
            }
            if (result == 2) {
                SetScreen(gm.spriteDB.error);
                rbBlackScreen.Dispose();
                sbLoading.Dispose();
            }
            else if (result == 3) {
                StartCoroutine(AnimateSuccessBar());
            }
        }

        private IEnumerator AnimateSuccessBar() {
            SpriteBuilder sbSuccessBar = ScreenElement.BuildSprite("LoadingSuccessful", Parent)
                .SetSize(32, 82)
                .SetSprite(gm.spriteDB.loadingComplete)
                .PlaceOutside(Direction.Up);
            for(int i = 0; i < 82 + 32; i++) {
                sbSuccessBar.Move(Direction.Down);
                yield return new WaitForSeconds(1.75f / 64);
            }
            CloseApp();
            if(controller is LogicManager logicMgr) {
                logicMgr.CallRandomBattle();
            }
        }
    }
}