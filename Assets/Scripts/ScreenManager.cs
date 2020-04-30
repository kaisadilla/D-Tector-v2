using Kaisa.Digivice.Extensions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class ScreenManager : MonoBehaviour {
        private SpriteDatabase spriteDB;
        private GameManager gm;
        private AudioManager audioMgr;
        private LogicManager logicMgr;

        private Transform animParent;

        public void AssignManagers(GameManager gm) {
            this.gm = gm;
            audioMgr = gm.audioMgr;
            this.logicMgr = gm.logicMgr;
            this.spriteDB = gm.spriteDB;
        }
        [Header("UI Elements")]
        public Image screenDisplay;

        public Transform ScreenParent => screenDisplay.transform;

        /// <summary>
        /// Player any number of animations, in a sequence. Those animations are full screen and hide any other element in the screen.
        /// </summary>
        /// <param name="animations">The animations to be played.</param>
        public void PlayAnimation(params IEnumerator[] animations) => StartCoroutine(PlayAnimationCoroutine(animations));
        private IEnumerator PlayAnimationCoroutine(params IEnumerator[] animations) {
            gm.LockInput();
            foreach(IEnumerator a in animations) {
                animParent = gm.BuildBackground(ScreenParent);
                yield return a;
                Destroy(animParent.gameObject);
            }
            gm.UnlockInput();
        }

        private void Update() {
            UpdateDisplay();
        }

        private void UpdateDisplay() {
            //Don't do anything on screens that never use this display.
            if (logicMgr.currentScreen == Screen.App) return;

            int index;
            Sprite sprite;
            switch(logicMgr.currentScreen) {
                case Screen.Character:
                    SetScreenSprite(gm.playerChar.CurrentSprite);
                    break;
                case Screen.MainMenu:
                    index = (int)logicMgr.currentMainMenu;
                    sprite = spriteDB.mainMenu[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesMenu:
                    index = (int)logicMgr.currentGameMenu;
                    sprite = spriteDB.game_sections[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesRewardMenu:
                    index = (int)logicMgr.currentGameRewardMenu;
                    sprite = spriteDB.games_reward[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesTravelMenu:
                    index = (int)logicMgr.currentGameTravelMenu;
                    sprite = spriteDB.games_travel[index];
                    SetScreenSprite(sprite);
                    break;
                default:
                    SetScreenSprite(null);
                    break;
            }
        }

        private void SetScreenSprite(Sprite sprite) {
            screenDisplay.sprite = sprite;
        }

        #region Animations
        public IEnumerator ASummonDigimon(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sDigimonCr = spriteDB.GetDigimonSprite(digimon, SpriteAction.Crush);
            Sprite sBlackScreen = spriteDB.blackScreen;
            Sprite sPowerBlack = spriteDB.givePowerBlack;
            Sprite sPowerWhite = spriteDB.givePowerWhite;

            audioMgr.PlaySound(audioMgr.summonDigimon);

            SpriteBuilder sbDigimon = gm.BuildSprite(digimon, animParent, 24, 24, 4, 4);
            SpriteBuilder sbBlackScreen = gm.BuildSprite("BlackScreen", animParent, sprite: sBlackScreen);

            for (int i = 0; i < 4; i++) {
                sbBlackScreen.SetActive(false);
                yield return new WaitForSeconds(0.15f);
                sbBlackScreen.SetActive(true);
                yield return new WaitForSeconds(0.15f);
            }

            for(int i = 0; i < 4; i++) {
                sbBlackScreen.SetSprite(sPowerBlack);
                yield return new WaitForSeconds(0.15f);
                sbBlackScreen.SetSprite(sBlackScreen);
                yield return new WaitForSeconds(0.15f);
            }

            sbBlackScreen.SetSprite(sPowerBlack);
            yield return new WaitForSeconds(0.15f);
            sbBlackScreen.SetSprite(sPowerWhite);
            yield return new WaitForSeconds(0.15f);
            sbBlackScreen.SetSprite(sPowerBlack);
            sbDigimon.SetSprite(sDigimon);
            yield return new WaitForSeconds(0.15f);

            for(int i = 0; i < 2; i++) {
                sbBlackScreen.SetTransparent(true);
                sbBlackScreen.SetSprite(sPowerWhite);
                yield return new WaitForSeconds(0.15f);
                sbBlackScreen.SetTransparent(false);
                sbBlackScreen.SetSprite(sPowerBlack);
                yield return new WaitForSeconds(0.15f);
            }

            sbBlackScreen.SetTransparent(true);
            sbBlackScreen.SetSprite(sPowerWhite);
            yield return new WaitForSeconds(0.20f);

            sbBlackScreen.SetActive(false);
            yield return new WaitForSeconds(0.20f);
            sbBlackScreen.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            sbBlackScreen.SetActive(false);
            yield return new WaitForSeconds(0.90f);
            sbBlackScreen.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            sbBlackScreen.SetActive(false);
            yield return new WaitForSeconds(1.25f);

            sbDigimon.SetSprite(sDigimonCr);
            yield return new WaitForSeconds(0.75f);
            //yield return new WaitUntil(() => !audioMgr.IsSoundPlaying);
        }

        public IEnumerator AUnlockDigimon(string digimon) {
            Sprite sDigimon = spriteDB.GetDigimonSprite(digimon);
            Sprite sCurtain = spriteDB.acquireDigimon;
            Sprite sDTector = spriteDB.dTector;
            Sprite sPowerWhite = spriteDB.giveMassivePowerWhite;

            audioMgr.PlaySound(audioMgr.unlockDigimon);

            SpriteBuilder sbDigimon = gm.BuildSprite(digimon, animParent, 24, 24, 4, 4, sprite: sDigimon);
            SpriteBuilder sbCurtain = gm.BuildSprite("BlackScreen", animParent, sprite: sCurtain, transparent: true);
            sbCurtain.PlaceOutside(Direction.Down);

            yield return new WaitForSeconds(0.15f);

            for(int i = 0; i < 32; i++) {
                sbCurtain.MoveSprite(Direction.Up);
                yield return new WaitForSeconds(1.5f / 32);

            }
            for(int i = 0; i < 32; i++) {
                sbCurtain.MoveSprite(Direction.Up);
                sbDigimon.MoveSprite(Direction.Up);
                yield return new WaitForSeconds(1.5f / 32);
            }

            yield return new WaitForSeconds(0.75f);
            sbDigimon.Dispose();
            sbCurtain.Dispose();

            gm.BuildSprite("DTector", animParent, sprite: sDTector);
            SpriteBuilder sbPower = gm.BuildSprite("Power", animParent, sprite: sPowerWhite, transparent: true);
            sbPower.SetActive(false);

            yield return new WaitForSeconds(0.30f);

            for(int i = 0; i < 5; i++) {
                sbPower.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                sbPower.SetActive(false);
                yield return new WaitForSeconds(0.15f);
            }

            yield return new WaitForSeconds(0.45f);
        }
        #endregion

        public IEnumerator ACharHappy() {
            yield return ACharHappyShort();
            yield return ACharHappyShort();
        }

        public IEnumerator ACharHappyShort() {
            Sprite charIdle = gm.PlayerCharSprites[0];
            Sprite charHappy = gm.PlayerCharSprites[6];

            audioMgr.PlaySound(audioMgr.charHappy);

            SpriteBuilder sbChar = gm.BuildSprite("Char", animParent, sprite: charIdle);

            for (int i = 0; i < 2; i++) {
                sbChar.SetSprite(charIdle);
                yield return new WaitForSeconds(0.5f);
                sbChar.SetSprite(charHappy);
                yield return new WaitForSeconds(0.5f);
            }
        }
        //TODO: Make the screen exit after the player presses A.
        public IEnumerator AAwardDistance(int score, int distanceBefore, int distanceAfter) {
            Sprite sScore = spriteDB.games_score;
            Sprite sDistance = spriteDB.games_distance;

            audioMgr.PlayButtonA();
            gm.BuildSprite("Score", animParent, 32, 5, 0, 0, sScore);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayButtonA();
            gm.BuildTextBox("ScoreText", animParent, score.ToString(), DFont.Regular, 31, 5, 0, 8, TextAnchor.UpperRight);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayButtonA();
            gm.BuildSprite("Distance", animParent, 32, 5, 0, 17, sDistance);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayButtonA();
            TextBoxBuilder tbDistance = gm.BuildTextBox("DistanceText", animParent, distanceBefore.ToString(), DFont.Regular, 31, 5, 0, 25, TextAnchor.UpperRight);
            yield return new WaitForSeconds(1f);

            audioMgr.PlayCharHappy();
            tbDistance.Text = distanceAfter.ToString();
            yield return new WaitForSeconds(3f);
        }

        public IEnumerator ATravelMap(Direction dir, int currentMap, int currentSector, int newSector) {
            Sprite mapCurrentSprite = spriteDB.GetMapSectorSprites(currentMap)[currentSector];
            Sprite mapNewSprite = spriteDB.GetMapSectorSprites(currentMap)[newSector];

            SpriteBuilder sMapCurrent = gm.BuildSprite("AnimCurrentSector", animParent, posX: 0, posY: 0, sprite: mapCurrentSprite);
            SpriteBuilder sMapNew = gm.BuildSprite("AnimNewSector", animParent, sprite: mapNewSprite);
            sMapNew.PlaceOutside(dir.OppositeDirection());

            float animDuration = 1.5f;
            for (int i = 0; i < 32; i++) {
                sMapCurrent.MoveSprite(dir);
                sMapNew.MoveSprite(dir);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            sMapCurrent.Dispose();
            sMapNew.Dispose();
        }

        public IEnumerator ASwapDDock(int ddock, string newDigimon) {
            float animDuration = 1.5f;
            Sprite newDigimonSprite = spriteDB.GetDigimonSprite(newDigimon);
            Sprite newDigimonSpriteCr = spriteDB.GetDigimonSprite(newDigimon, SpriteAction.Crush);

            audioMgr.PlaySound(audioMgr.changeDock);

            SpriteBuilder bBlackBars = gm.BuildSprite("BlackBars", animParent, sprite: gm.spriteDB.blackBars);
            bBlackBars.PlaceOutside(Direction.Down);
            SpriteBuilder bDDock = gm.BuildSprite("DDock", animParent, sprite: gm.spriteDB.status_ddock[ddock]);
            SpriteBuilder bDDockSprite = gm.BuildDDockSprite(ddock, bDDock.transform);

            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 32; i++) {
                bBlackBars.MoveSprite(Direction.Up);
                bDDock.MoveSprite(Direction.Up);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            bDDockSprite.SetSprite(newDigimonSprite);
            yield return new WaitForSeconds(0.75f);

            for (int i = 0; i < 32; i++) {
                bBlackBars.MoveSprite(Direction.Down);
                bDDock.MoveSprite(Direction.Down);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(audioMgr.PlaySoundAfterDelay(audioMgr.charHappy, 0.175f));
            for (int i = 0; i < 5; i++) {
                bDDockSprite.SetSprite(null);
                yield return new WaitForSeconds(0.175f);
                bDDockSprite.SetSprite(newDigimonSpriteCr);
                yield return new WaitForSeconds(0.175f);
            }

            yield return new WaitForSeconds(0.5f);

            bBlackBars.Dispose();
            bDDock.Dispose();
        }
    }
}