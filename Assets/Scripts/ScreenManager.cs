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
            StartCoroutine(ConsumeQueue());
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
        /*private IEnumerator ConsumeQueue() {
            while (true) {
                if(animationQueue.Count > 0) {
                    gm.LockInput();
                    for(int i = 0; i < animationQueue.Count; i++) {
                        animParent = gm.BuildContainer("Anim Parent", ScreenParent, 32, 32, transparent: false).transform;
                        yield return animationQueue.Dequeue();
                        Destroy(animParent.gameObject);
                    }
                    gm.UnlockInput();
                }
                yield return new WaitForEndOfFrame();
            }
        }*/

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