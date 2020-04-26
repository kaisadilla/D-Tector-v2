using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kaisa.Digivice;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class ScreenManager : MonoBehaviour {
        private SpriteDatabase spriteDB;
        private GameManager gm;
        private LogicManager logicMgr;
        public void AssignManagers(GameManager gm) {
            this.gm = gm;
            this.logicMgr = gm.logicMgr;
            this.spriteDB = gm.spriteDB;
        }
        [Header("UI Elements")]
        public Image screenDisplay;

        private void Update() {
            UpdateDisplay();
        }

        public void UpdateDisplay() {
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
                case Screen.GameMenu:
                    index = (int)logicMgr.currentGameMenu;
                    sprite = spriteDB.game_sections[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GameRewardMenu:
                    index = (int)logicMgr.currentGameRewardMenu;
                    sprite = spriteDB.games_reward[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GameTravelmenu:
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

    }
}