using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.App {
    public class SpeedRunner : DigiviceApp {
        /* This number is equal to 0.75 / 38 / 3 and it's the biggest number that should exactly match all the speeds / 38 */
        //private const float THIS_DELTA_TIME = 0.00657894736842105f;
        private const float THIS_DELTA_TIME = 0.005f;

        //Constants
        private const int ROW_COUNT = 70;
        private const byte HAS_FIRST  = 0b001; //If the row has an asteroid on the first position.
        private const byte HAS_SECOND = 0b010;
        private const byte HAS_THIRD  = 0b100;

        private byte currentScreen = 0; //1: for end game.
        //Level data
        private byte[] rowData = new byte[ROW_COUNT];
        //private float[] speeds = new float[] { 2.85f, 1.85f, 1.1f, 0.8f }; //(THIS IS THE ORIGINAL D-TECTOR SPEED)
        //private float[] speeds = new float[] { 1.5f, 1f, 0.66f, 0.5f }; //Suicide.
        private float[] speeds = new float[] { 2f, 1.5f, 1.0f, 0.75f }; //The times it takes for a row to travel 38 pixels.
        //Game stats
        private bool gameStarted = false;
        private byte rocketPosition = 1; //0, 1, 2
        private int rowsBeaten = 0; //The amount of rows the player has already beaten.
        private byte crashes = 0;
        //UI elements
        private SpriteBuilder rocket;
        private ContainerBuilder[] visualRows = new ContainerBuilder[2];
        private ContainerBuilder finishRow;
        private SpriteBuilder[] speedMarks = new SpriteBuilder[4];
        //Update controls
        private float timeUntilMovement = 0f;
        private int currentRow = -1;
        private int currentSpeed = 0;
        private int[] rowY = new int[] {-6, -6}; //Represents the y coordinate of each of the two rows.
        private int finishY = -32;
        private bool lockControls = false;
        //Player controls
        private Direction lastDirectionTapped = Direction.none;

        #region Input
        public override void InputA() {
            if (currentScreen == 1) {
                audioMgr.PlayButtonA();
                SubmitScoreAndClose();
            }
        }
        public override void InputB() {
            if (currentScreen == 0) {
                audioMgr.PlayButtonB();
                CloseApp(Screen.GamesTravelMenu);
            }
            else if (currentScreen == 1) {
                audioMgr.PlayButtonA();
                SubmitScoreAndClose();
            }
        }
        public override void InputLeftDown() {
            lastDirectionTapped = Direction.Left;
        }
        public override void InputRightDown() {
            lastDirectionTapped = Direction.Right;
        }
        public override void InputLeftUp() {
            if (lastDirectionTapped == Direction.Left) lastDirectionTapped = Direction.none;
        }
        public override void InputRightUp() {
            if (lastDirectionTapped == Direction.Right) lastDirectionTapped = Direction.none;
        }
        #endregion
        protected override void StartApp() {
            GenerateLevel();
            Sprite sRocket = gm.spriteDB.speedRunner_rocket;
            Sprite sSpeedMark = gm.spriteDB.speedRunner_rocketSpeedMark;
            Sprite sAsteroid = gm.spriteDB.speedRunner_rocketAsteroid;
            Sprite sExplosion = gm.spriteDB.speedRunner_rocketExplosion;
            Sprite sFinish = gm.spriteDB.speedRunner_rocketFinish;

            ScreenElement.BuildRectangle("Line", Parent).SetSize(1, 32).SetPosition(6, 0);
            rocket = ScreenElement.BuildSprite("Rocket", Parent).SetSize(8, 8).SetPosition(15, 32).SetSprite(sRocket);
            StartCoroutine(IASpawnRocket());

            for(int i = 0; i < visualRows.Length; i++) {
                visualRows[i] = ScreenElement.BuildContainer($"Row{i}", Parent).SetSize(24, 6).SetPosition(7, -6);
                for (int j = 0; j < 3; j++) {
                    ScreenElement.BuildSprite($"Asteroid{j}", visualRows[i].transform).SetSize(7, 6).SetPosition(j * 8, 0).SetSprite(sAsteroid);
                }
            }
            finishRow = ScreenElement.BuildContainer($"FinishRow", Parent).SetSize(25, 32).SetPosition(7, -32);
            for (int i = 0; i < 2; i++) {
                ScreenElement.BuildSprite($"FinishLine{i}", finishRow.transform).SetSize(6, 32).SetPosition(1 + (i * 17), 0).SetSprite(sFinish);
            }
            for (int i = 0; i < speedMarks.Length; i++) {
                speedMarks[i] = ScreenElement.BuildSprite($"SpeedMark{i}", Parent).SetSize(3, 5).SetPosition(2, 26 - (i * 6)).SetSprite(sSpeedMark);
                speedMarks[i].SetActive(false);
            }
            InvokeRepeating("CustomUpdate", 0f, THIS_DELTA_TIME);
        }

        protected override void CloseApp(Screen goToMenu = Screen.MainMenu) {
            StopAllCoroutines();
            base.CloseApp(goToMenu);
        }

        private void CustomUpdate() {
            if (!gameStarted) return;

            if (!lockControls) {
                //Set the position of the rocket wherever the player is tapping.
                if (lastDirectionTapped == Direction.Left) rocketPosition = 0;
                else if (lastDirectionTapped == Direction.Right) rocketPosition = 2;
                else rocketPosition = 1;
                //Set the visual position of the rocket.
                #if UNITY_EDITOR
                if(Input.GetKey(KeyCode.LeftArrow)) rocketPosition = 0;
                if (Input.GetKey(KeyCode.RightArrow)) rocketPosition = 2;
                #endif
            }

            rocket.SetPosition(8 * (rocketPosition + 1) - 1, rocket.Position.y);
            //Update the speed after x rows have been beaten THIS ROUND.
            if (currentSpeed != 3) {
                int rowsBeatenThisRound = rowsBeaten - (ROW_COUNT - rowData.Length);
                if (rowsBeatenThisRound == 0) {
                    speedMarks[0].SetActive(true);
                }
                else if (rowsBeatenThisRound == 1) {
                    speedMarks[1].SetActive(true);
                    currentSpeed = 1;
                }
                else if (rowsBeatenThisRound == 3) {
                    speedMarks[2].SetActive(true);
                    currentSpeed = 2;
                }
                else if (rowsBeatenThisRound == 6) {
                    speedMarks[3].SetActive(true);
                    currentSpeed = 3;
                }
            }
            //If no row has been generated yet, or if the latest row has reached y = 16.
            if (currentRow == -1 || (currentRow < rowData.Length && rowY[GetVisualRowIndex(currentRow)] == 16)) {
                currentRow++;
                if (currentRow < rowData.Length) {
                    rowY[GetVisualRowIndex(currentRow)] = -6;
                    EnableAsteroids(currentRow);
                }
            }

            //The part of the update that only triggers on a fixed time. 
            timeUntilMovement += THIS_DELTA_TIME;

            if (timeUntilMovement > (speeds[currentSpeed] / 38)) {
                timeUntilMovement = 0;
                rowY[0]++;
                //If we aren't in row 0 (because else this would apply to row -1).
                if (currentRow > 0) {
                    rowY[1]++;
                    if (currentRow < rowData.Length) {
                        if (rowY[GetVisualRowIndex(currentRow - 1)] == 31) {
                            audioMgr.PlaySound(audioMgr.speedRunner_Asteroid);
                            rowsBeaten++;
                        }
                    }
                }
                //If the current row is the finish line, and the finish line has not reached y = 0 yet.
                if(currentRow == rowData.Length && finishY < 0) {
                    finishY++;
                    finishRow.SetPosition(7, finishY);
                    //Win the game
                    if (finishY == 0) {
                        audioMgr.PlaySound(audioMgr.speedRunner_Finish);
                        rowsBeaten++;
                        lockControls = true;
                        StartCoroutine(IAElevateRocket());
                    }
                }
                //If the current row is not the first one nor the finish row.
                if(currentRow > 0 && currentRow < rowData.Length) {
                    int lowerRowY = rowY[GetVisualRowIndex(currentRow - 1)];
                    //If the lower row is in a y position where it could crash with the rocket.
                    if (lowerRowY >= 20 && lowerRowY <= 29) {
                        if(IsAsteroidAtPos(currentRow - 1, rocketPosition)) {
                            audioMgr.PlaySound(audioMgr.speedRunner_Crash);
                            rocket.SetSprite(gm.spriteDB.speedRunner_rocketExplosion);

                            gameStarted = false;
                            rowY[0] = -6;
                            rowY[1] = -6;

                            _CrashRocket();
                        }
                    }
                }
                //If the finish row could collide with the rocket. In this situation, no more opportunities are given to the player.
                else if (currentRow == rowData.Length) {
                    if(finishY >= -6 && finishY < -1) {
                        if (rocketPosition != 1) {
                            Debug.Log("Crashed with goal!");
                            audioMgr.PlaySound(audioMgr.speedRunner_Crash);
                            rocket.SetSprite(gm.spriteDB.speedRunner_rocketExplosion);

                            gameStarted = false;
                            rowY[0] = -6;
                            rowY[1] = -6;

                            StartCoroutine(IALoseGame());
                        }
                    }
                }
                visualRows[0].SetPosition(7, rowY[0]);
                visualRows[1].SetPosition(7, rowY[1]);
            }

            void _CrashRocket() {
                if (crashes < 2 && currentRow < rowData.Length) {
                    StartCoroutine(IASpawnRocket(0.5f));
                    rowsBeaten++;
                    crashes++;
                    rowData = rowData.SubArray(currentRow);
                    currentRow = -1;
                    currentSpeed = 0;

                    foreach (SpriteBuilder sb in speedMarks) {
                        sb.SetActive(false);
                    }
                }
                else {
                    StartCoroutine(IALoseGame());
                }
            }

        }

        /// <summary>
        /// Returns the index of the UI row associated with the real, logic-side row.
        /// </summary>
        private int GetVisualRowIndex(int logicRow) => logicRow % 2;
        private void EnableAsteroids(int logicRow) {
            ContainerBuilder visualRow = visualRows[GetVisualRowIndex(logicRow)];

            for (int i = 0; i < 3; i++) {
                //Set enabled to be equal to whether there's an asteroid.
                visualRow.SetChildActive(i, IsAsteroidAtPos(logicRow, i));
            }
        }

        private bool IsAsteroidAtPos(int logicRow, int posIndex) {
            switch (posIndex) {
                case 0: return (rowData[logicRow] & HAS_FIRST) == HAS_FIRST;
                case 1: return (rowData[logicRow] & HAS_SECOND) == HAS_SECOND;
                case 2: return (rowData[logicRow] & HAS_THIRD) == HAS_THIRD;
                default: return false;
            }
        }


        private void GenerateLevel() { 
            for(int i = 0; i < rowData.Length; i++) {
                /* Add two asteroids to each row. A third of the times, the second asteroid will be
                 * the same as the first and that line will only contain one. */
                do {
                    rowData[i] = 0;
                    for (int attempt = 0; attempt < 2; attempt++) {
                        switch (Random.Range(0, 3)) {
                            case 0:
                                rowData[i] |= HAS_FIRST;
                                break;
                            case 1:
                                rowData[i] |= HAS_SECOND;
                                break;
                            case 2:
                                rowData[i] |= HAS_THIRD;
                                break;
                        }
                    }
                }
                while (i > 0 && rowData[i] == rowData[i - 1]); //Do not generate the same row twice in a row.
            }
        }

        private int CalculateScore() {
            int score = (rowsBeaten * 6) - (80 * crashes);
            return (score > 0) ? score : 0;
        }

        private void SubmitScoreAndClose() {
            gm.SubmitGameScore(CalculateScore());
            CloseApp(Screen.GamesTravelMenu);
        }

        #region Animations
        private IEnumerator IASpawnRocket(float delay = 0f) {
            yield return new WaitForSeconds(delay);
            audioMgr.PlaySound(audioMgr.speedRunner_Start);

            rocket.SetPosition(15, 32);
            rocket.SetSprite(gm.spriteDB.speedRunner_rocket);

            for (int i = 0; i < 8; i++) {
                yield return new WaitForSeconds(1f / 8);
                rocket.Move(Direction.Up);
            }
            TextBoxBuilder tbStart = ScreenElement.BuildTextBox("Start", Parent, DFont.Small).SetText("START").SetSize(24, 5).SetPosition(9, 8);
            yield return new WaitForSeconds(0.5f);
            tbStart.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            tbStart.SetActive(true);
            yield return new WaitForSeconds(1f);
            tbStart.SetActive(false);
            gameStarted = true;
        }
        private IEnumerator IALoseGame() {
            gm.LockInput();

            foreach (SpriteBuilder sb in speedMarks) {
                sb.SetActive(false);
            }
            finishRow.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            TextBoxBuilder tbStart = ScreenElement.BuildTextBox("Game Over", Parent, DFont.Small).SetText("GAME\nOVER").SetSize(24, 11).SetPosition(9, 8);
            yield return new WaitForSeconds(0.5f);
            currentScreen = 1;
            gm.UnlockInput();
        }
        private IEnumerator IAElevateRocket() {
            gm.LockInput();

            for(int i = 0; i < 32; i++) {
                rocket.Move(Direction.Up);
                yield return new WaitForSeconds(1.5f / 32);
            }
            foreach(SpriteBuilder sb in speedMarks) {
                sb.SetActive(false);
            }
            finishRow.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            TextBoxBuilder tbGoal = ScreenElement.BuildTextBox("Goal", Parent, DFont.Small).SetText("GOAL!").SetSize(24, 5).SetPosition(9, 8);
            yield return new WaitForSeconds(0.5f);
            currentScreen = 1;
            gm.UnlockInput();
            while (true) {
                tbGoal.SetActive(!tbGoal.Active);
                yield return new WaitForSeconds(0.5f);
            }
        }
        #endregion
    }
}