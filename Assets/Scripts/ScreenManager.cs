using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class ScreenManager : MonoBehaviour {
        private SpriteDatabase spriteDB;
        private GameManager gm;
        private AudioManager audioMgr;
        private LogicManager logicMgr;

        public Transform RootParent => screenDisplay.transform;
        public Transform animParent;

        private Queue<IEnumerator> animationQueue = new Queue<IEnumerator>();

        private SpriteBuilder defeatedLayer;
        private SpriteBuilder eventLayer;
        private SpriteBuilder eyesLayer;
        public bool PlayingAnimations { get; private set; }

        public void Initialize(GameManager gm) {
            this.gm = gm;
            audioMgr = gm.audioMgr;
            logicMgr = gm.logicMgr;
            spriteDB = gm.spriteDB;
            UpdateColors();
        }
        [Header("UI Elements")]
        public Image screenBackground;
        public Image screenDisplay;

        public void UpdateColors() {
            screenBackground.color = Preferences.BackgroundColor;
            screenDisplay.color = Preferences.ActiveColor;
        }

        public Transform ScreenParent => screenDisplay.transform;

        /// <summary>
        /// Adds a new animation the queue.
        /// </summary>
        public void EnqueueAnimation(IEnumerator animation) {
            animationQueue.Enqueue(animation);
            if (!PlayingAnimations) StartCoroutine(ConsumeQueue());
        }

        private void Start() {
            //InvokeRepeating("UpdateDisplay", 0f, 0.05f);
            defeatedLayer = ScreenElement.BuildSprite("Defeated", RootParent.transform).SetSize(6, 7)
                .SetPosition(1, 1).SetTransparent(true).SetActive(false);
            eventLayer = ScreenElement.BuildSprite("Event", RootParent.transform).SetTransparent(true).SetActive(false);
            eyesLayer = ScreenElement.BuildSprite("Eyes", RootParent.transform).SetTransparent(true).SetActive(false);

            StartCoroutine(PAFlashDefeatedEffect());
            StartCoroutine(PAFlashEventEffect());
            StartCoroutine(PAFlashEyesEffect());

            StartCoroutine(ConsumeQueue());
        }
        private IEnumerator PAFlashDefeatedEffect() {
            defeatedLayer.transform.SetAsFirstSibling();
            while (true) {
                defeatedLayer.SetSprite(spriteDB.defeatedSymbol);
                yield return new WaitForSeconds(0.5f);
                defeatedLayer.SetSprite(spriteDB.emptySprite);
                yield return new WaitForSeconds(0.5f);
            }
        }
        private IEnumerator PAFlashEventEffect() {
            eventLayer.transform.SetAsFirstSibling();
            while (true) {
                eventLayer.SetSprite(spriteDB.triggerEvent);
                yield return new WaitForSeconds(0.2f);
                eventLayer.SetSprite(spriteDB.emptySprite);
                yield return new WaitForSeconds(0.2f);
            }
        }
        private IEnumerator PAFlashEyesEffect() {
            eyesLayer.transform.SetAsFirstSibling();
            while (true) {
                eyesLayer.SetSprite(spriteDB.eyes[0]);
                yield return new WaitForSeconds(Random.Range(0.25f, 1f));
                eyesLayer.SetSprite(spriteDB.eyes[1]);
                yield return new WaitForSeconds(Random.Range(0.25f, 1f));
            }
        }

        private IEnumerator ConsumeQueue() {
            PlayingAnimations = true;
            while (animationQueue.Count > 0) {
                gm.LockInput();
                animParent = ScreenElement.BuildContainer("Anim Parent", ScreenParent, false).SetSize(32, 32).transform;
                yield return animationQueue.Dequeue();
                Destroy(animParent.gameObject);
                gm.UnlockInput();
            }
            PlayingAnimations = false;
            gm.CheckPendingEvents();
        }
        private void ClearAnimParent() {
            foreach (Transform child in animParent) Destroy(child.gameObject);
        }

        private void Update() {
            UpdateDisplay();
        }

        private void UpdateDisplay() {
            foreach (Transform child in RootParent) {
                if (child.gameObject.tag == "disposable") {
                    Destroy(child.gameObject);
                }
            }

            int showLayer = 0; //0: none, 1: defeated, 2: event, 3: eyes.
            if (logicMgr.currentScreen == Screen.Character) {
                if (gm.IsCharacterDefeated) showLayer = 1;
                else if (gm.IsEventActive) showLayer = 2;
                else if (gm.showEyes) showLayer = 3;
            }
            defeatedLayer.SetActive(showLayer == 1);
            eventLayer.SetActive(showLayer == 2);
            eyesLayer.SetActive(showLayer == 3);

            int index;
            Sprite sprite;
            switch (logicMgr.currentScreen) {
                case Screen.CharSelection:
                    SpriteBuilder sb = ScreenElement.BuildSprite("Arrows", screenDisplay.transform).SetSprite(spriteDB.arrows).SetTransparent(true);
                    sb.gameObject.tag = "Disposable";
                    sb.transform.SetAsFirstSibling();
                    SetScreenSprite(spriteDB.GetCharacterSprites((GameChar)logicMgr.charSelectionIndex)[0]);
                    break;
                case Screen.Character:
                    SetScreenSprite(gm.PlayerCharSprites[gm.CurrentPlayerCharSprite]);
                    break;
                case Screen.MainMenu:
                    index = (int)logicMgr.currentMainMenu;
                    sprite = spriteDB.mainMenu[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesMenu:
                    index = logicMgr.gamesMenuIndex;
                    sprite = spriteDB.game_sections[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesRewardMenu:
                    index = logicMgr.gamesRewardMenuIndex;
                    sprite = spriteDB.games_reward[index];
                    SetScreenSprite(sprite);
                    break;
                case Screen.GamesTravelMenu:
                    index = logicMgr.gamesTravelMenuIndex;
                    sprite = spriteDB.games_travel[index];
                    SetScreenSprite(sprite);
                    break;
                default:
                    SetScreenSprite(spriteDB.emptySprite);
                    break;
            }
        }

        private void SetScreenSprite(Sprite sprite) {
            screenDisplay.sprite = sprite;
        }
    }
}