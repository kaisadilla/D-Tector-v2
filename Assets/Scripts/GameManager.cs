using Kaisa.CircularTypes;
using Kaisa.Digivice;
using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public class GameManager : MonoBehaviour {
        //[Tooltip("The screen builder")]
        public AudioManager audioMgr;
        public DebugManager debug;
        public InputManager inputMgr;
        public LogicManager logicMgr;
        public ScreenManager screenMgr;
        public SpriteDatabase spriteDB;

        public GameObject mainScreen;

        [Header("Apps")]
        public GameObject pAppMap;
        public GameObject pAppStatus;
        public GameObject pAppDatabase;
        public GameObject pAppDigits;
        public GameObject pAppCamp;
        public GameObject pAppConnect;

        [Header("Screen elements")]
        public GameObject pSolidSprite;
        public GameObject pRectangle;
        public GameObject pTextBox;

        public PlayerCharacter playerChar;
        public Database Database { get; private set; }
        public DistanceManager DistanceMgr { get; private set; }
        public SavedGame LoadedGame { get; private set; }

        public void Awake() {
            SetupManagers();
            Database = new Database(this);
            DistanceMgr = new DistanceManager(this);
        }

        public void Start() {
            LoadGame();
        }

        public void DEBUGInitialize() {
            LoadedGame.CurrentMap = 0;
            LoadedGame.CurrentArea = 0;
            LoadedGame.CurrentDistance = 3362;
            LoadedGame.PlayerChar = GameChar.Koji;
        }

        private void SetupManagers() {
            inputMgr.AssignManagers(this);
            logicMgr.AssignManagers(this);
            screenMgr.AssignManagers(this);
            debug.AssignManagers(this);
        }

        private void LoadGame() {
            LoadedGame = SavedGame.LoadSavedGame(0);
            GameChar gameChar = LoadedGame.PlayerChar;
            playerChar = new PlayerCharacter(gameChar, spriteDB.GetCharacterSprites(gameChar));
            InvokeRepeating("UpdateCharSprite", 0.5f, 0.5f);
            //LoadedGame.CurrentMap = 0;
            //LoadedGame.CurrentArea = 0;
            //LoadedGame.CurrentDistance = 3362;
            //LoadedGame.SetAreaCompleted(0, 1, true);
            //LoadedGame.SetAreaCompleted(0, 2, true);
            //LoadedGame.SetAreaCompleted(0, 4, true);
            //LoadedGame.SetAreaCompleted(0, 5, true);
            //LoadedGame.SetAreaCompleted(0, 11, true);
        }

        private void UpdateCharSprite() => playerChar.UpdateSprite(); //This should be done with a Task in PlayerCharacter, but I avoided installing the necessary plugins to make async Tasks work in this project.


        public void LockInput() => inputMgr.inhibitInput = true;
        public void UnlockInput() => inputMgr.inhibitInput = false;

        public Transform MainScreenTransform => screenMgr.screenDisplay.transform;

        #region Create Screen Elements
        public ScreenElement CreateScreenElement(GameObject element, Transform parent, string name = "") {
            GameObject go = Instantiate(element, parent);
            ScreenElement goClass = go.GetComponent<ScreenElement>();
            if (name != "") goClass.SetName(name);
            return goClass;
        }
        public SpriteBuilder CreateSprite(string name, Transform parent, int width = 32, int height = 32, int posX = 0, int posY = 0, Sprite sprite = null) {
            GameObject go = Instantiate(pSolidSprite, parent);
            SpriteBuilder goClass = go.GetComponent<SpriteBuilder>();
            goClass.SetName(name);
            if (sprite != null) goClass.SetSprite(sprite);
            goClass.SetSize(width, height);
            goClass.SetPosition(posX, posY);
            return goClass;
        }
        public RectangleBuilder CreateRectangle(string name, Transform parent, int width = 1, int height = 1, int posX = 0, int posY = 0, float flickPeriod = 0f) {
            GameObject go = Instantiate(pRectangle, parent);
            RectangleBuilder goClass = go.GetComponent<RectangleBuilder>();
            goClass.SetName(name);
            goClass.SetSize(width, height);
            goClass.SetPosition(posX, posY);
            goClass.SetFlickPeriod(flickPeriod);
            return goClass;
        }
        public TextBoxBuilder CreateTextBox(string name, Transform parent, string content, DFont font, int width = 32, int height = 5, int posX = 0, int posY = 0, TextAnchor alignment = TextAnchor.UpperLeft) {
            GameObject go = Instantiate(pTextBox, parent);
            TextBoxBuilder goClass = go.GetComponent<TextBoxBuilder>();
            goClass.SetName(name);
            goClass.SetSize(width, height);
            goClass.SetPosition(posX, posY);
            goClass.Text = content;
            goClass.SetFont(font);
            goClass.SetAlignment(alignment);
            return goClass;
        }

        public SpriteBuilder CreateSpriteForDDock(int ddock, Transform parent) {
            Sprite dockDigimon = spriteDB.GetDigimonSprite(Database.GetDDockDigimon(ddock));
            if (dockDigimon == null) dockDigimon = spriteDB.status_ddockEmpty;
            return CreateSprite($"DigimonDDock{ddock}", parent, 24, 24, 4, 8, dockDigimon);
        }
        #endregion

        #region Animations
        public void PlayAnimation(IEnumerator coroutine) {
            StartCoroutine(coroutine);
        }
        #endregion
    }
}