using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace Kaisa.Digivice.Apps {
    public class DigiHunter : DigiviceApp {
        private const int STARTING_TIME = 60;

        private bool gameStarted = false;

        private int score;
        private int timeRemaining = STARTING_TIME; //-1: playing end sound, -2 end.
        private TextBoxBuilder tbTime;

        private int playerX, playerY;
        private SpriteBuilder[] sbArrows = new SpriteBuilder[2];
        private int[,] faces = new int[3,3]; //int[y][x] (each array contains an horizontal line of 3 faces)
        private SpriteBuilder[,] sbFaces = new SpriteBuilder[3,3];

        public override void InputA() {
            if (timeRemaining >= 0) {
                AttemptDestroy(playerY, playerX);
            }
            else if (timeRemaining == -2) {
                audioMgr.PlayButtonA();
                SubmitScoreAndClose();
            }
        }
        public override void InputB() {
            if (timeRemaining > 0) {
                audioMgr.PlayButtonB();
                CloseApp();
            }
            else if (timeRemaining == -2) {
                audioMgr.PlayButtonA();
                SubmitScoreAndClose();
            }
        }
        public override void InputLeft() {
            if(timeRemaining >= 0) {
                MoveY();
            }
        }
        public override void InputRight() {
            if (timeRemaining >= 0) {
                MoveX();
            }
        }

        public override void StartApp() {
            gm.EnqueueAnimation(Animations.StartAppDigiHunter(mark => gameStarted = mark));

            ScreenElement.BuildTextBox("Time", screenDisplay.transform, DFont.Small).SetText("TIME").SetSize(18, 5).SetPosition(1, 0);
            tbTime = ScreenElement.BuildTextBox("TimeCount", screenDisplay.transform, DFont.Small).SetText(timeRemaining.ToString()).SetSize(10, 5).SetPosition(22, 0);

            sbArrows[0] = ScreenElement.BuildSprite("Y-Arrow", Parent).SetSize(3, 6).SetPosition(2, 9).SetSprite(gm.spriteDB.digiHunter_arrows[0]);
            sbArrows[1] = ScreenElement.BuildSprite("X-Arrow", Parent).SetSize(6, 3).SetPosition(6, 5).SetSprite(gm.spriteDB.digiHunter_arrows[1]);
            for(int y = 0; y < sbFaces.GetLength(0); y++) {
                for(int x = 0; x < sbFaces.GetLength(1); x++) {
                    sbFaces[y,x] = ScreenElement.BuildSprite("Face", Parent).SetSize(8, 8).SetPosition(5 + (x * 8), 8 + (y * 8)).SetSprite(gm.spriteDB.emptySprite);
                }
            }

            gm.StartCoroutine(CountDown());
            gm.StartCoroutine(GenerateFaces());
        }

        private IEnumerator CountDown() {
            while(!gameStarted) {
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(1f);
            while (timeRemaining > 0) {
                if (this == null) {
                    yield break;
                }
                timeRemaining--;
                tbTime.Text = timeRemaining.ToString();
                yield return new WaitForSeconds(1f);
            }

            timeRemaining = -1;
            audioMgr.PlaySound(audioMgr.speedRunner_Finish);
            yield return new WaitForSeconds(1.5f);
            timeRemaining = -2;

            foreach (var sb in sbFaces) sb.Dispose();

            TextBoxBuilder tb = ScreenElement.BuildTextBox("End", screenDisplay.transform, DFont.Small).SetText("END").SetPosition(6, 17);
        }

        private IEnumerator GenerateFaces() {
            while (!gameStarted) {
                yield return new WaitForSeconds(0.05f);
            }

            float baseMin = 0.75f;
            float baseMax = 1.5f;

            while (timeRemaining > 0) {
                if (this == null) yield break;

                int x = Random.Range(0, 3);
                int y = Random.Range(0, 3);

                float nextMin = baseMin - ((0.75f * baseMin) * ((STARTING_TIME - timeRemaining) / (float)STARTING_TIME));
                float nextMax = baseMax - ((0.75f * baseMax) * ((STARTING_TIME - timeRemaining) / (float)STARTING_TIME));

                if (faces[y, x] == 0) {
                    int face = Random.Range(1, 3); //1: white, 2: black
                    faces[y, x] = face;
                    sbFaces[y, x].SetSprite(gm.spriteDB.digiHunter_faces[face - 1]);
                    StartCoroutine(DestroyFaceAfterDelay(y, x, Random.Range(baseMin * 2f, baseMax * 2f)));
                }

                yield return new WaitForSeconds(Random.Range(nextMin, nextMax));
            }
        }
        
        private IEnumerator DestroyFaceAfterDelay(int y, int x, float delay) {
            float accDelay = 0f;
            while(accDelay < delay) {
                if (faces[y, x] < 1) {
                    yield break;
                }
                accDelay += 0.05f;
                yield return new WaitForSeconds(0.05f);
            }

            faces[y, x] = 0;
            if(sbFaces[y, x] != null) sbFaces[y, x].SetSprite(gm.spriteDB.emptySprite);
        }

        private (int y, int x)? GetRandomNonEmptyFace() {
            List<(int y, int x)> candidates = new List<(int y, int x)>();
            for(int y = 0; y < faces.GetLength(0); y++) {
                for (int x = 0; x < faces.GetLength(1); x++) {
                    if (faces[y, x] != 0) candidates.Add((y, x));
                }
            }

            return candidates.GetRandomElement();
        }

        private void MoveX() {
            playerX = playerX.CircularAdd(1, 2);
            sbArrows[1].SetPosition(6 + (playerX * 8), 5);
        }
        private void MoveY() {
            playerY = playerY.CircularAdd(1, 2);
            sbArrows[0].SetPosition(2, 9 + (playerY * 8));
        }

        private void AttemptDestroy(int y, int x) {
            if (faces[y, x] < 1) {
                audioMgr.PlayButtonA();
            }
            else if (faces[y, x] == 1) {
                audioMgr.PlaySound(audioMgr.speedRunner_Asteroid);
                score++;
                StartCoroutine(DestroyFace(y, x));
            }
            else if (faces[y, x] == 2) {
                audioMgr.PlaySound(audioMgr.speedRunner_Crash);
                score--;
                StartCoroutine(DestroyFace(y, x));
            }
        }

        private IEnumerator DestroyFace(int y, int x) {
            faces[y, x] = -1;
            sbFaces[y, x].SetSprite(gm.spriteDB.digiHunter_explosion);
            yield return new WaitForSeconds(0.35f);
            sbFaces[y, x].SetSprite(gm.spriteDB.emptySprite);
            faces[y, x] = 0;
        }
        private void SubmitScoreAndClose() {
            gm.SubmitGameScore(CalculateScore());
            CloseApp(Screen.GamesTravelMenu);
        }

        private int CalculateScore() {
            if (score > 0) return score * 15;
            else return 0;
        }
    }
}