﻿using Kaisa.Digivice.App;
using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] private PlayerCharacter playerChar;
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
        public GameObject pAppFinder;
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
            if(SavedGame.CurrentlyLoadedSlot == -1) {
                SceneManager.LoadScene("MainMenu");
            }
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
            #if UNITY_EDITOR
            Application.targetFrameRate = 60;
            DisableLeaverBuster();
            audioMgr.SetVolume(0.48f);
            VisualDebug.WriteLine("Leaver Buster disabled by the Unity editor.");
            #endif

            SetupManagers();
            SetupStaticClasses();
            DatabaseMgr = new DatabaseManager();

            if(loadedGame.PlayerChar == GameChar.none) {
                VisualDebug.WriteLine("Saved character assigned to 'none'. A new game will be created.");
                logicMgr.currentScreen = Screen.CharSelection;
                EnqueueAnimation(screenMgr.ALoadCharacterSelection());
            }
            else {
                logicMgr.currentScreen = Screen.Character;
                CheckLeaverBuster();
                CheckPendingEvents();
            }
            //EnqueueAnimation(screenMgr.AStartGameAnimation(GameChar.Zoe, "kazemon", 3, "agumon", 2));
        }

        public void CloseGame() {
            SavedGame.CurrentlyLoadedSlot = -1;
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Creates the necessary keys in the SavedGame to run the game for the first time.
        /// </summary>
        public void CreateNewGame(GameChar chosenGameChar) {
            VisualDebug.WriteLine("Created new game.");

            string randomInitial = DatabaseMgr.GetInitialDigimons().GetRandomElement();
            string playerSpirit = GetPlayerSpirit(chosenGameChar);

            loadedGame.PlayerChar = chosenGameChar;
            loadedGame.SetRandomSeed(0, Random.Range(0, 2147483647));
            loadedGame.SetRandomSeed(1, Random.Range(0, 2147483647));
            loadedGame.SetRandomSeed(2, Random.Range(0, 2147483647));
            loadedGame.CheatsUsed = false;
            loadedGame.StepsToNextEvent = 300;
            DistanceMgr.MoveToArea(0, 0);
            logicMgr.SetDigimonUnlocked(playerSpirit, true);
            logicMgr.SetDigimonUnlocked(randomInitial, true);
            logicMgr.SetDDockDigimon(0, "");
            logicMgr.SetDDockDigimon(1, "");
            logicMgr.SetDDockDigimon(2, "");
            logicMgr.SetDDockDigimon(3, "");
            logicMgr.SpiritPower = 99;
            AssignRandomBosses();
            int spiritEnergy = DatabaseMgr.GetDigimon(playerSpirit).GetBossStats(1).GetEnergyRank();
            int enemyEnergy = DatabaseMgr.GetDigimon(randomInitial).GetRegularStats().GetEnergyRank();

            EnqueueAnimation(screenMgr.AStartGameAnimation(chosenGameChar, playerSpirit, spiritEnergy, randomInitial, enemyEnergy));

            logicMgr.currentScreen = Screen.Character;
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

        private void SetupStaticClasses() {
            ScreenElement.Initialize(pContainer, pSolidSprite, pRectangle, pTextBox);
        }

        private void LoadGame() {
            int currentLoadedGame = SavedGame.CurrentlyLoadedSlot;
            loadedGame = SavedGame.LoadSavedGame(currentLoadedGame);
            GameChar gameChar = loadedGame.PlayerChar;
            playerChar.Initialize(this, gameChar);
        }

        private void CheckLeaverBuster() {
            if (loadedGame.IsLeaverBusterActive) {
                int expLoss = loadedGame.LeaverBusterExpLoss;
                string digimonLoss = loadedGame.LeaverBusterDigimonLoss;
                VisualDebug.WriteLine($"Leaver Buster triggered. Experience lost: {expLoss}. Digimon lost: {digimonLoss}");
                logicMgr.AddPlayerExperience(-expLoss);
                logicMgr.PunishDigimon(digimonLoss, out _, out _);
                DistanceMgr.IncreaseDistance(2000);
                DisableLeaverBuster();
            }
        }
        /// <summary>
        /// Triggers an event if there's any event pending in the distance manager.
        /// </summary>
        public void CheckPendingEvents() {
            if (logicMgr.IsAppLoaded) return; //Don't trigger while an app is loaded. When an app is closed, this is called again.

            int savedEvent = loadedGame.SavedEvent;
            if (savedEvent == 0) return;
            else if (savedEvent == 1) {
                logicMgr.EnqueueRegularEvent();
            }
            else if(savedEvent == 2) {
                logicMgr.EnqueueBossEvent();
            }
        }

        public GameChar CurrentPlayerChar => playerChar.currentChar;

        public void DebugInitialize() {
            //loadedGame.SetRandomSeed(0, Random.Range(0, 2147483647));
            //loadedGame.SetRandomSeed(1, Random.Range(0, 2147483647));
            //loadedGame.SetRandomSeed(2, Random.Range(0, 2147483647));
            //loadedGame.CheatsUsed = false;
            //AssignRandomBosses();
            //loadedGame.StepsToNextEvent = 300;
            loadedGame.SlotExists = true;
            loadedGame.Overwrittable = false;
        }

        /// <summary>
        /// Reduces distance by 1, if possible, and increases the step count by one.
        /// </summary>
        public void TakeAStep() {
            //TODO: Trigger both methods' events if needed.
            DistanceMgr.TakeSteps(1);
            DistanceMgr.ReduceDistance(1);
            if (DistanceMgr.CurrentDistance == 1) loadedGame.SavedEvent = 2;
            CheckPendingEvents();
        }
        /// <summary>
        /// Returns the Transform of the main screen, usually to submit it as a parent to other gameobjects.
        /// </summary>
        public Transform RootParent => screenMgr.RootParent;
        /// <summary>
        /// Returns the array of sprites corresponding with the current character.
        /// </summary>
        public Sprite[] PlayerCharSprites => spriteDB.GetCharacterSprites(loadedGame.PlayerChar);

        #region Input interaction
        public void LockInput() => inputMgr.inhibitInput = true;
        public void UnlockInput() => inputMgr.inhibitInput = false;
        #endregion
        #region PlayerChar interaction
        public CharState GetPlayerCharState() => playerChar.currentState;
        public void SetPlayerCharState(CharState state) => playerChar.currentState = state;
        public int CurrentPlayerCharSprite => playerChar.CurrentSprite;
        public void SetEventActive(bool triggerEvent) => playerChar.SetEventMode(triggerEvent);
        #endregion

        /// <summary>
        /// Applies the score to the current distance and triggers the animation for beating a game.
        /// </summary>
        public void SubmitGameScore(int score) {
            int oldDistance = DistanceMgr.CurrentDistance;
            DistanceMgr.ReduceDistance(score);
            int newDistance = DistanceMgr.CurrentDistance;
            DistanceMgr.TakeSteps(Mathf.RoundToInt(score / 5f));
            screenMgr.EnqueueAnimation(screenMgr.AAwardDistance(score, oldDistance, newDistance));
        }
        public SpriteBuilder GetDDockScreenElement(int ddock, Transform parent) {
            SpriteBuilder sbDDockName = ScreenElement.BuildSprite("$DDock{ddock}", parent).SetSprite(spriteDB.status_ddock[ddock]);
            Sprite dockDigimon = spriteDB.GetDigimonSprite(logicMgr.GetDDockDigimon(ddock));
            if (dockDigimon == null) dockDigimon = spriteDB.status_ddockEmpty;
            return ScreenElement.BuildSprite($"DigimonDDock{ddock}", sbDDockName.transform).SetSize(24, 24).SetPosition(4, 8).SetSprite(dockDigimon);
        }

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
        public List<string> GetAllUnlockedDigimonInStage(Stage stage) {
            List<string> allDigimon = new List<string>();
            foreach (Digimon d in DatabaseMgr.Digimons) {
                if (d.stage == stage && logicMgr.GetDigimonUnlocked(d.name)) {
                    allDigimon.Add(d.name);
                }
            }
            return allDigimon;
        }
        public List<string> GetAllUnlockedSpiritsOfElement(Element element) {
            List<string> allDigimon = new List<string>();
            foreach (Digimon d in DatabaseMgr.Digimons) {
                if (d.stage == Stage.Spirit
                        && d.element == element
                        && d.spiritType != SpiritType.Fusion
                        && logicMgr.GetDigimonUnlocked(d.name))
                {
                    allDigimon.Add(d.name);
                }
            }
            return allDigimon;
        }
        public List<string> GetAllUnlockedFusionDigimon() {
            List<string> allDigimon = new List<string>();
            foreach (Digimon d in DatabaseMgr.Digimons) {
                if (d.stage == Stage.Spirit && d.spiritType == SpiritType.Fusion && logicMgr.GetDigimonUnlocked(d.name)) {
                    allDigimon.Add(d.name);
                }
            }
            return allDigimon;
        }
        /// <summary>
        /// Returns true if the player has both the Human and Animal form of a spirit.
        /// </summary>
        public bool HasBothFormsOfSpirit(Element element) {
            int count = 0;
            foreach (Digimon d in DatabaseMgr.Digimons) {
                if (d.stage == Stage.Spirit
                        && d.element == element
                        && (d.spiritType == SpiritType.Human || d.spiritType == SpiritType.Animal)
                        && logicMgr.GetDigimonUnlocked(d.name))
                {
                    count++;
                }
            }
            return (count == 2);
        }
        /// <summary>
        /// Returns true if the player has all the required spirits for a fusion. fusionName can be 'kaisergreymon', 'magnagarurumon' or 'susanoomon'.
        /// </summary>
        public bool HasAllSpiritsForFusion(string fusionName) {
            int count = 0;
            if (fusionName == "kaisergreymon") {
                foreach (Digimon d in DatabaseMgr.Digimons) {
                    if (d.stage == Stage.Spirit
                            && (d.spiritType == SpiritType.Human || d.spiritType == SpiritType.Animal)
                            && (d.element == Element.Fire || d.element == Element.Wind || d.element == Element.Ice
                            || d.element == Element.Earth || d.element == Element.Wood)
                            && logicMgr.GetDigimonUnlocked(d.name))
                    {
                        count++;
                    }
                }
            }
            else if (fusionName == "magnagarurumon") {
                foreach (Digimon d in DatabaseMgr.Digimons) {
                    if (d.stage == Stage.Spirit
                            && (d.spiritType == SpiritType.Human || d.spiritType == SpiritType.Animal)
                            && (d.element == Element.Light || d.element == Element.Thunder || d.element == Element.Dark
                            || d.element == Element.Metal || d.element == Element.Water)
                            && logicMgr.GetDigimonUnlocked(d.name))
                    {
                        count++;
                    }
                }
            }
            else {
                foreach (Digimon d in DatabaseMgr.Digimons) {
                    if (d.stage == Stage.Spirit
                            && (d.spiritType == SpiritType.Human || d.spiritType == SpiritType.Animal)
                            && logicMgr.GetDigimonUnlocked(d.name))
                    {
                        count++;
                    }
                }
            }
            if (fusionName == "kaisergreymon" || fusionName == "magnagarurumon") return (count == 10);
            else return (count == 20);
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

        public void AssignRandomBosses() {
            string[][][] bosses = DatabaseMgr.Bosses;
            for (int map = 0; map < bosses.Length; map++) {
                List<string> worldBosses = bosses[map][0].ToList();
                //Take the initial Human spirit of the player from the list.
                if (map == 0) {
                    if (!worldBosses.Remove(GetPlayerSpirit(playerChar.currentChar))) {
                        VisualDebug.WriteLine("No suitable Digimon was found to be removed from the first list of bosses. This should never happen.");
                    }
                }
                else if (map == 6) {
                    loadedGame.SetSemibossGroupForMap(6, Random.Range(1, bosses[map].Length));
                }

                worldBosses.Shuffle();

                //If the world has semibosses, choose one set at random.
                if(bosses[map].Length > 1) {
                    string[] semibosses = bosses[map][Random.Range(1, bosses[map].Length)];
                    for (int i = 0; i < semibosses.Length; i++) {
                        int semibossIndexInMainList = worldBosses.FindIndex(val => val.Equals($"<sp-{i}>"));
                        if(semibossIndexInMainList > -1) {
                            worldBosses[semibossIndexInMainList] = semibosses[i];
                        }
                    }
                }

                loadedGame.SetBossesForMap(map, worldBosses.ToArray());
            }
        }

        public string GetPlayerSpirit(GameChar playerChar) {
            switch(playerChar) {
                case GameChar.Takuya: return "agunimon";
                case GameChar.Koji: return "lobomon";
                case GameChar.Zoe: return "kazemon";
                case GameChar.JP: return "beetlemon";
                case GameChar.Tommy: return "kumamon";
                case GameChar.Koichi: return "loweemon";
                default: return "";
            }
        }

        public string GetBossOfCurrentArea() {
            int currentMap = DistanceMgr.CurrentMap;
            int areasInMap = DistanceMgr.GetNumberOfAreasInMap(currentMap);
            int currentArea = DistanceMgr.CurrentArea;

            return loadedGame.GetBossesForMap(currentMap, areasInMap)[currentArea];
        }

        #region Animations
        public void EnqueueAnimation(IEnumerator animation) => screenMgr.EnqueueAnimation(animation);
        #endregion
    }
}