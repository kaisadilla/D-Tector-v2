using System;
using System.Collections;
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
        [SerializeField] private ShakeDetector shakeDetector;

        public GameObject mainScreen;

        [Header("Apps")]
        public GameObject pAppMap;
        public GameObject pAppStatus;
        public GameObject pAppDatabase;
        public GameObject pAppDigits;
        public GameObject pAppCamp;
        public GameObject pAppConnect;

        [Header("Games")]
        public GameObject pAppSpeedRunner;
        public GameObject pAppMaze;

        [Header("Screen elements")]
        public GameObject pContainer;
        public GameObject pSolidSprite;
        public GameObject pScreenSprite;
        public GameObject pRectangle;
        public GameObject pTextBox;

        public PlayerCharacter playerChar;
        public DatabaseManager DatabaseMgr { get; private set; }
        public DistanceManager DistanceMgr { get; private set; }
        public SavedGame LoadedGame { get; private set; }

        public void Awake() {
            SetupManagers();
            DatabaseMgr = new DatabaseManager(this);
            DistanceMgr = new DistanceManager(this);
            /*if (Application.isMobilePlatform) { 
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 120;
            }*/

        }

        public void Start() {
            LoadGame();
        }

        /// <summary>
        /// Returns the corresponding app prefab based on the type of this app.
        /// </summary>
        /// <param name="app">The app class calling this.</param>
        /// <returns></returns>
        public GameObject GetAppPrefab(string appName) {
            switch(appName) {
                case "map":         return pAppMap;
                case "status":      return pAppStatus;
                case "database":    return pAppDatabase;
                case "digits":      return pAppDigits;
                case "maze":        return pAppSpeedRunner;
                case "speedrunner": return pAppMaze;
                default: return null;
            }
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
            shakeDetector.AssignManagers(this);
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

        public void TakeAStep() {
            DistanceMgr.TakeSteps(1); //Check for this.
            DistanceMgr.ReduceDistance(1, out _);
        }

        private void UpdateCharSprite() => playerChar.UpdateSprite(); //This should be done with a Task in PlayerCharacter, but I avoided installing the necessary plugins to make async Tasks work in this project.

        public Sprite[] PlayerCharSprites => spriteDB.GetCharacterSprites(LoadedGame.PlayerChar);
        public void LockInput() => inputMgr.inhibitInput = true;
        public void UnlockInput() => inputMgr.inhibitInput = false;

        public bool GetTappingEnabled(Direction dir) => inputMgr.GetTappingEnabled(dir);
        public void SetTappingEnabled(Direction dir, bool enabled, float speed = 0.15f) => inputMgr.SetTappingEnabled(dir, enabled, speed);


        public Transform RootParent => screenMgr.screenDisplay.transform;

        public void SubmitGameScore(int score) {
            int oldDistance = DistanceMgr.CurrentDistance;
            DistanceMgr.ReduceDistance(score, out _);
            int newDistance = DistanceMgr.CurrentDistance;
            screenMgr.PlayAnimation(screenMgr.AAwardDistance(score, oldDistance, newDistance));
        }

        #region Create Screen Elements
        public ScreenElement BuildScreenElement(GameObject element, Transform parent, string name = "") {
            GameObject go = Instantiate(element, parent);
            ScreenElement goClass = go.GetComponent<ScreenElement>();
            if (name != "") goClass.SetName(name);
            return goClass;
        }
        public SpriteBuilder BuildSprite(string name, Transform parent, int width = 32, int height = 32, int posX = 0, int posY = 0, Sprite sprite = null, bool transparent = false) {
            GameObject go = Instantiate(pSolidSprite, parent);
            SpriteBuilder goClass = go.GetComponent<SpriteBuilder>();
            goClass.SetName(name);
            if (sprite != null) goClass.SetSprite(sprite);
            goClass.SetSize(width, height);
            goClass.SetPosition(posX, posY);
            goClass.SetTransparent(transparent);
            return goClass;
        }
        public RectangleBuilder BuildRectangle(string name, Transform parent, int width = 1, int height = 1, int posX = 0, int posY = 0, float flickPeriod = 0f, bool activeColor = true) {
            GameObject go = Instantiate(pRectangle, parent);
            RectangleBuilder goClass = go.GetComponent<RectangleBuilder>();
            goClass.SetName(name);
            goClass.SetSize(width, height);
            goClass.SetPosition(posX, posY);
            goClass.SetFlickPeriod(flickPeriod);
            goClass.SetColor(activeColor);
            return goClass;
        }
        public TextBoxBuilder BuildTextBox(string name, Transform parent, string content, DFont font, int width = 32, int height = 5, int posX = 0, int posY = 0, TextAnchor alignment = TextAnchor.UpperLeft) {
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
        public Transform BuildBackground(Transform parent) {
            RectangleBuilder goClass = BuildRectangle("Parent", parent, 32, 32, activeColor: false);
            return goClass.transform;
        }
        public ContainerBuilder BuildContainer(string name, Transform parent, int width = 1, int height = 1, int posX = 0, int posY = 0, bool transparent = true) {
            GameObject go = Instantiate(pContainer, parent);
            ContainerBuilder goClass = go.GetComponent<ContainerBuilder>();
            goClass.SetName(name);
            goClass.SetSize(width, height);
            goClass.SetPosition(posX, posY);
            goClass.SetTransparent(transparent);
            return goClass;
        }

        public SpriteBuilder BuildDDockSprite(int ddock, Transform parent) {
            Sprite dockDigimon = spriteDB.GetDigimonSprite(DatabaseMgr.GetDDockDigimon(ddock));
            if (dockDigimon == null) dockDigimon = spriteDB.status_ddockEmpty;
            return BuildSprite($"DigimonDDock{ddock}", parent, 24, 24, 4, 8, dockDigimon);
        }
        #endregion

        #region Animations
        public void PlayAnimation(params IEnumerator[] animations) {
            screenMgr.PlayAnimation(animations);
        }
        #endregion
    }
}