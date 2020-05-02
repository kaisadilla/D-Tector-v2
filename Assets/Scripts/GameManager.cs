using Kaisa.Digivice.App;
using System.Collections;
using UnityEngine;

namespace Kaisa.Digivice {
    /// <summary>
    /// This class acts as the central link between different managers and classes. Any object with access to the GameManager, has access to all the other managers.
    /// Additionally, this class provides some variables and methods commonly used accross different classes. This class doesn't generally set or edit
    /// any value, but instead tells other managers to do so.
    /// </summary>
    public class GameManager : MonoBehaviour {
        //Managers
        public AudioManager audioMgr;
        public DebugManager debug;
        public InputManager inputMgr;
        public LogicManager logicMgr;
        public ScreenManager screenMgr;
        public SpriteDatabase spriteDB;
        public DatabaseManager DatabaseMgr { get; private set; }
        public DistanceManager DistanceMgr { get; private set; }

        //Other objects
        [SerializeField] private ShakeDetector shakeDetector;
        private PlayerCharacter playerChar;
        private SavedGame loadedGame;

        [SerializeField] private GameObject mainScreen;

        [Header("Apps")]
        public GameObject pAppMap;
        public GameObject pAppStatus;
        public GameObject pAppDatabase;
        public GameObject pAppDigits;
        public GameObject pAppCamp;
        public GameObject pAppConnect;

        [Header("Games")]
        public GameObject pAppBattle;
        public GameObject pAppSpeedRunner;
        public GameObject pAppMaze;

        [Header("Screen elements")]
        public GameObject pContainer;
        public GameObject pSolidSprite;
        public GameObject pScreenSprite;
        public GameObject pRectangle;
        public GameObject pTextBox;

        public void Awake() {
            VisualDebug.SetDebugManager(debug);
            //Set up the essential configuration and ready managers.
            /*if (Application.isMobilePlatform) {
                AudioConfiguration config = AudioSettings.GetConfiguration();
                config.dspBufferSize = 256;
                //QualitySettings.vSyncCount = 0;
                //Application.targetFrameRate = 120;
            }*/

            //Load information about the player's game.
            LoadGame();
        }

        public void Start() {
            SetupManagers();
            DatabaseMgr = new DatabaseManager(this);
            EnqueueAnimation(screenMgr.ALevelUp(12));
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

        private void SetupManagers() {
            inputMgr.AssignManagers(this);
            logicMgr.Initialize(this, loadedGame);
            screenMgr.AssignManagers(this);

            debug.Initialize(this, loadedGame);

            shakeDetector.AssignManagers(this);
            DistanceMgr = new DistanceManager(this, loadedGame);
        }

        private void LoadGame() {
            loadedGame = SavedGame.LoadSavedGame(0);
            GameChar gameChar = loadedGame.PlayerChar;
            playerChar = new PlayerCharacter(gameChar);
            InvokeRepeating("UpdateCharSprite", 0.5f, 0.5f);
        }
        //This should be done with a Task in PlayerCharacter, but I avoided installing the necessary plugins to make async Tasks work in this project.
        private void UpdateCharSprite() => playerChar.UpdateSprite();
        /// <summary>
        /// Creates the necessary keys in the SavedGame to run the game for the first time.
        /// </summary>
        private void CreateNewGame() {
            //TODO: This function is not being worked on yet.
            //loadedGame.PlayerChar = GameChar.Koji;
            //LoadedGame.CurrentMap = 0;
            //LoadedGame.CurrentArea = 0;
            //LoadedGame.CurrentDistance = <--distance-->;
            //LoadedGame.SetRandomSeed(0, <--random-->);
            //LoadedGame.SetRandomSeed(1, <--random-->);
            //LoadedGame.SetRandomSeed(2, <--random-->);
            //LoadedGame.SetDDockDigimon(0, "<empty>");
            //LoadedGame.SetDDockDigimon(1, "<empty>");
            //LoadedGame.SetDDockDigimon(2, "<empty>");
            //LoadedGame.SetDDockDigimon(3, "<empty>");
            //LoadedGame.SetAreaCompleted(0, 1, true);
            //LoadedGame.SetAreaCompleted(0, 2, true);
            //LoadedGame.SetAreaCompleted(0, 4, true);
            //LoadedGame.SetAreaCompleted(0, 5, true);
            //LoadedGame.SetAreaCompleted(0, 11, true);
        }

        /// <summary>
        /// Reduces distance by 1, if possible, and increases the step count by one.
        /// </summary>
        public void TakeAStep() {
            //TODO: Trigger both methods' events if needed.
            DistanceMgr.TakeSteps(1);
            DistanceMgr.ReduceDistance(1, out _);
        }
        /// <summary>
        /// Returns the Transform of the main screen, usually to submit it as a parent to other gameobjects.
        /// </summary>
        public Transform RootParent => screenMgr.screenDisplay.transform;
        /// <summary>
        /// Returns the array of sprites corresponding with the current character.
        /// </summary>
        public Sprite[] PlayerCharSprites => spriteDB.GetCharacterSprites(loadedGame.PlayerChar);

        #region Input interaction
        public void LockInput() => inputMgr.inhibitInput = true;
        public void UnlockInput() => inputMgr.inhibitInput = false;

        public bool GetTappingEnabled(Direction dir) => inputMgr.GetTappingEnabled(dir);
        public void SetTappingEnabled(Direction dir, bool enabled, float speed = 0.15f) => inputMgr.SetTappingEnabled(dir, enabled, speed);
        #endregion
        #region PlayerChar interaction
        public CharState GetPlayerCharState() => playerChar.currentState;
        public void SetPlayerCharState(CharState state) => playerChar.currentState = state;
        public int CurrentPlayerCharSprite => playerChar.CurrentSprite;
        #endregion

        /// <summary>
        /// Applies the score to the current distance and triggers the animation for beating a game.
        /// </summary>
        public void SubmitGameScore(int score) {
            int oldDistance = DistanceMgr.CurrentDistance;
            DistanceMgr.ReduceDistance(score, out _);
            int newDistance = DistanceMgr.CurrentDistance;
            screenMgr.EnqueueAnimation(screenMgr.AAwardDistance(score, oldDistance, newDistance));
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
            SpriteBuilder sbDDockName = BuildSprite("$DDock{ddock}", parent, sprite: spriteDB.status_ddock[ddock]);
            Sprite dockDigimon = spriteDB.GetDigimonSprite(logicMgr.GetDDockDigimon(ddock));
            if (dockDigimon == null) dockDigimon = spriteDB.status_ddockEmpty;
            return BuildSprite($"DigimonDDock{ddock}", sbDDockName.transform, 24, 24, 4, 8, dockDigimon);
        }

        public ContainerBuilder BuildStatSign(string message, Transform parent) {
            ContainerBuilder cbSign = BuildContainer("Sign", parent, 32, 17, 0, 15, false).SetBackgroundBlack(true);
            TextBoxBuilder sbMessage = BuildTextBox("Sign", cbSign.transform, message, DFont.Small, 28, 5, 2, 2);
            sbMessage.InvertColors(true);
            TextBoxBuilder sbValue = BuildTextBox("Sign", cbSign.transform, "", DFont.Small, 28, 5, 2, 10, TextAnchor.UpperRight);
            sbValue.InvertColors(true);
            return cbSign;
        }
        #endregion

        /// <summary>
        /// Returns one of the three seeds of this game at random.
        /// </summary>
        public int GetRandomSavedSeed() {
            return loadedGame.GetRandomSeed(Random.Range(0, 3));
        }

        public string[] GetAllDDockDigimons() {
            string[] ddockDigimon = new string[4];
            for(int i = 0; i < ddockDigimon.Length; i++) {
                ddockDigimon[i] = logicMgr.GetDDockDigimon(i);
            }
            return ddockDigimon;
        }
        /// <summary>
        /// Returns true if the player has unlocked, at least, one digimon in that stage.
        /// </summary>
        public bool OwnsDigimonInStage(Stage stage) {
            foreach (Digimon d in DatabaseMgr.Digimons) {
                if (d.stage == stage && logicMgr.GetDigimonUnlocked(d.name)) {
                    return true;
                }
            }
            return false;
        }

        public bool OwnsSpiritDigimonOfElement(Element element) {
            foreach (Digimon d in DatabaseMgr.Digimons) {
                if (d.stage == Stage.Spirit && d.spiritType != SpiritType.Fusion && d.element == element && logicMgr.GetDigimonUnlocked(d.name)) {
                    return true;
                }
            }
            return false;
        }
        public bool OwnsFusionSpiritDigimon() {
            foreach (Digimon d in DatabaseMgr.Digimons) {
                if (d.stage == Stage.Spirit && d.spiritType == SpiritType.Fusion && logicMgr.GetDigimonUnlocked(d.name)) {
                    return true;
                }
            }
            return false;
        }

        public bool IsInDock(string digimon) {
            foreach(string s in GetAllDDockDigimons()) {
                if (s == digimon) return true;
            }
            return false;
        }

        /// <summary>
        /// Enables LeaverBuster and updates the values attached to it.
        /// </summary>
        /// <param name="expLoss">The experience the player would lose if they were busted.</param>
        /// <param name="digimonLoss">The digimon the player would lose if they were busted.</param>
        public void UpdateLeaverBuster(int expLoss, string digimonLoss) {
            loadedGame.IsLeaverBusterActive = true;
            loadedGame.LeaverBusterExpLoss = expLoss;
            loadedGame.LeaverBusterDigimonLoss = digimonLoss;
        }
        /// <summary>
        /// Disables LeaverBuster.
        /// </summary>
        public void DisableLeaverBuster() {
            loadedGame.IsLeaverBusterActive = false;
            loadedGame.LeaverBusterExpLoss = 0;
            loadedGame.LeaverBusterDigimonLoss = "";
        }

        #region Animations
        public void EnqueueAnimation(IEnumerator animation) => screenMgr.EnqueueAnimation(animation);
        #endregion
    }
}