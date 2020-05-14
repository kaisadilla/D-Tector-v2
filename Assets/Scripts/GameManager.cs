using Kaisa.Digivice.Apps;
using Kaisa.Digivice.Extensions;
using Newtonsoft.Json;
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

        public AppLoader appLoader;

        public WorldManager WorldMgr { get; private set; }
        //Other objects
        [SerializeField] private ShakeDetector shakeDetector;
        [SerializeField] private PlayerCharacter playerChar;

        [SerializeField] private GameObject mainScreen;

        [Header("Screen elements")]
        public GameObject pContainer;
        public GameObject pSolidSprite;
        public GameObject pRectangle;
        public GameObject pTextBox;

        public int JackpotValue {
            get => SavedGame.JackpotValue;
            set => SavedGame.JackpotValue = value;
        }
        public bool IsCharacterDefeated {
            get => SavedGame.IsPlayerDefeated;
            set => SavedGame.IsPlayerDefeated = value;
        }
        public bool IsEventActive => logicMgr.IsEventPending;
        public bool isCharacterWalking = false;
        public bool showEyes => Database.Worlds[WorldMgr.CurrentWorld].showEyes;

        public void Awake() {
            if(SavedGame.CurrentlyLoadedFilePath == "") {
                SceneManager.LoadScene("MainMenu");
            }
            else {
                SavedGame.LoadSavedGame(SavedGame.CurrentlyLoadedFilePath);
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
            debug.ShowDebug();
            debug.EnableCheats();
            #endif

            Database.LoadDatabases(); //So it isn't loaded mid-game.
            Animations.Initialize(this, audioMgr, spriteDB);

            SetupManagers();
            SetupStaticClasses();
            if (SavedGame.PlayerChar == GameChar.none) {
                VisualDebug.WriteLine("Saved character assigned to 'none'. A new game will be created.");
                logicMgr.currentScreen = Screen.CharSelection;
                EnqueueAnimation(Animations.LoadCharacterSelection());
            }
            else {
                logicMgr.currentScreen = Screen.Character;
                CheckLeaverBuster();
                CheckPendingEvents();
            }

            AttemptUpdateGame();
            StartCoroutine(IncreaseJackpotValue());

            //EnqueueAnimation(Animations.EnemyEscapes("Duskmon", "Baihumon"));
            MutableCombatStats suka = Database.GetDigimon("devimon").GetBossStats(10);
            Debug.Log($"devimon hp {suka}");
            suka = Database.GetDigimon("lanamon").GetBossStats(10);
            Debug.Log($"lanamon hp {suka}");
            suka = Database.GetDigimon("nanimon").GetBossStats(10);
            Debug.Log($"nanimon hp {suka}");
            suka = Database.GetDigimon("ogremon").GetBossStats(10);
            Debug.Log($"ogremon hp {suka}");
            suka = Database.GetDigimon("sukamon").GetBossStats(10);
            Debug.Log($"sukamon hp {suka}");
            suka = Database.GetDigimon("gatomon").GetBossStats(10);
            Debug.Log($"gatomon hp {suka}");
            suka = Database.GetDigimon("kuwagamon").GetBossStats(10);
            Debug.Log($"kuwagamon hp {suka}");
            suka = Database.GetDigimon("woodmon").GetBossStats(10);
            Debug.Log($"woodmon hp {suka}");
            suka = Database.GetDigimon("raremon").GetBossStats(10);
            Debug.Log($"raremon hp {suka}");
            suka = Database.GetDigimon("doggymon").GetBossStats(10);
            Debug.Log($"doggymon hp {suka}");
            suka = Database.GetDigimon("wendigomon").GetBossStats(10);
            Debug.Log($"wendigomon hp {suka}");
            suka = Database.GetDigimon("wizardmon").GetBossStats(10);
            Debug.Log($"wizardmon hp {suka}");
            suka = Database.GetDigimon("devidramon").GetBossStats(10);
            Debug.Log($"devidramon hp {suka}");

            //CompleteWorld(0);

            //EnqueueAnimation(Animations.TransitionToMap1(PlayerChar));
        }

        //Called via InvokeRepeating
        private IEnumerator IncreaseJackpotValue() {
            while (true) {
                yield return new WaitForSeconds(300f);
                if (JackpotValue < 20) JackpotValue++;
            }
        }

        public void CloseGame() {
            SavedGame.CurrentlyLoadedFilePath = "";
            SavedGame.CloseSavedGame();
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Creates the necessary keys in the SavedGame to run the game for the first time.
        /// </summary>
        public void CreateNewGame(GameChar chosenGameChar) {
            VisualDebug.WriteLine("Created new game.");

            string randomInitial = Database.LoadInitialDigimonsFromFile().GetRandomElement();
            string playerSpirit = Database.PlayerSpirit[chosenGameChar];

            SavedGame.PlayerChar = chosenGameChar;

            for(int i = 0; i < SavedGame.RandomSeed.Length; i++) {
                SavedGame.RandomSeed[i] = Random.Range(0, 2147483647);
            }

            SavedGame.CheatsUsed = false;
            SavedGame.StepsToNextEvent = 300;
            WorldMgr.MoveToArea(0, 0);
            logicMgr.SetDigimonUnlocked(playerSpirit, true);
            logicMgr.SetDigimonUnlocked(randomInitial, true);
            logicMgr.SpiritPower = 99;

            WorldMgr.SetupWorlds();

            int spiritEnergy = Database.GetDigimon(playerSpirit).GetBossStats(1).GetEnergyRank();
            int enemyEnergy = Database.GetDigimon(randomInitial).GetRegularStats().GetEnergyRank();

            EnqueueAnimation(Animations.StartGameAnimation(chosenGameChar, playerSpirit, spiritEnergy, randomInitial, enemyEnergy));

            logicMgr.currentScreen = Screen.Character;
        }

        private void SetupManagers() {
            inputMgr.AssignManagers(this);
            logicMgr.Initialize(this);
            screenMgr.Initialize(this);

            debug.Initialize(this);

            shakeDetector.AssignManagers(this);
            WorldMgr = new WorldManager(this);
        }

        private void SetupStaticClasses() {
            ScreenElement.Initialize(pContainer, pSolidSprite, pRectangle, pTextBox);
        }

        private void LoadGame() {
            GameChar gameChar = SavedGame.PlayerChar;
            playerChar.Initialize(this, gameChar);
        }

        private void CheckLeaverBuster() {
            if (SavedGame.IsLeaverBusterActive) {
                uint expLoss = SavedGame.LeaverBusterExpLoss;
                string digimonLoss = SavedGame.LeaverBusterDigimonLoss;
                VisualDebug.WriteLine($"Leaver Buster triggered. Experience lost: {expLoss}. Digimon lost: {digimonLoss}");
                logicMgr.RemovePlayerExperience(expLoss);

                if (Database.GetDigimon(digimonLoss).stage != Stage.Spirit) {
                    logicMgr.PunishDigimon(digimonLoss, out _, out _);
                }
                else {
                    logicMgr.LoseSpirit(digimonLoss);
                }

                WorldMgr.IncreaseDistance(2000);
                logicMgr.IncreaseTotalBattles();
                DisableLeaverBuster();
            }
        }
        /// <summary>
        /// Triggers an event if there's any event pending in the distance manager.
        /// </summary>
        public void CheckPendingEvents() {
            //Don't trigger while an app is loaded. When an app is closed, this is called again.
            if (logicMgr.IsAppLoaded && !(logicMgr.loadedApp is Apps.Status)) return;

            int savedEvent = SavedGame.SavedEvent;
            if (savedEvent == 0) return;
            else if (savedEvent == 1) {
                logicMgr.EnqueueRegularEvent();
                logicMgr.CloseLoadedApp();
            }
            else if(savedEvent == 2) {
                logicMgr.EnqueueBossEvent();
                logicMgr.CloseLoadedApp();
            }
        }

        public GameChar PlayerChar => playerChar.currentChar;

        /// <summary>
        /// Attempts to update the game if the version of the last update does not match the current version of the game.
        /// This method should be changed whenever an update in the save file(s) is wanted.
        /// </summary>
        public void AttemptUpdateGame() {
            if(SavedGame.PlayerChar != GameChar.none && SavedGame.LastUpdateVersion != Constants.GAME_VERSION) {
                JackpotValue = 20;
                SavedGame.LastUpdateVersion = Constants.GAME_VERSION;
                SavedGame.LostSpirits = new List<string>();
                VisualDebug.WriteLine($"Updated game to version {Constants.GAME_VERSION}");
            }
        }

        /// <summary>
        /// Reduces distance by 1, if possible, and increases the step count by one.
        /// </summary>
        public void TakeAStep() {
            //TODO: Trigger both methods' events if needed.
            WorldMgr.TakeSteps(1);
            WorldMgr.ReduceDistance(1);
            if (WorldMgr.CurrentDistance == 1) SavedGame.SavedEvent = 2;
            CheckPendingEvents();
        }
        /// <summary>
        /// Returns the Transform of the main screen, usually to submit it as a parent to other gameobjects.
        /// </summary>
        public Transform RootParent => screenMgr.RootParent;
        /// <summary>
        /// Returns the array of sprites corresponding with the current character.
        /// </summary>
        public Sprite[] PlayerCharSprites => spriteDB.GetCharacterSprites(SavedGame.PlayerChar);

        #region Input interaction
        public void LockInput() => inputMgr.inhibitInput = true;
        public void UnlockInput() => inputMgr.inhibitInput = false;
        public bool IsInputLocked => inputMgr.inhibitInput;
        #endregion
        #region PlayerChar interaction
        public int CurrentPlayerCharSprite => playerChar.CurrentSprite;
        #endregion

        /// <summary>
        /// Applies the score to the current distance and triggers the animation for beating a game.
        /// </summary>
        public void SubmitGameScore(int score) {
            int oldDistance = WorldMgr.CurrentDistance;
            WorldMgr.ReduceDistance(score);
            int newDistance = WorldMgr.CurrentDistance;
            WorldMgr.TakeSteps(Mathf.RoundToInt(score / 5f));
            screenMgr.EnqueueAnimation(Animations.AwardDistance(score, oldDistance, newDistance));
        }
        public SpriteBuilder GetDDockScreenElement(int ddock, Transform parent) {
            SpriteBuilder sbDDockName = ScreenElement.BuildSprite("$DDock{ddock}", parent).SetSprite(spriteDB.status_ddock[ddock]);
            Sprite dockDigimon = spriteDB.GetDigimonSprite(logicMgr.GetDDockDigimon(ddock));
            if (dockDigimon == null) dockDigimon = spriteDB.status_ddockEmpty;
            return ScreenElement.BuildSprite($"DigimonDDock{ddock}", sbDDockName.transform).SetSize(24, 24).SetPosition(4, 8).SetSprite(dockDigimon);
        }
        public ContainerBuilder BuildMapScreen(int world, Transform parent) {
            ContainerBuilder cbMap = ScreenElement.BuildContainer("Map Container", parent);
            string worldSprite = Database.Worlds[world].worldSprite;
            if (Database.Worlds[world].multiMap) {
                cbMap.SetSize(64, 64);
                ScreenElement.BuildSprite("Map 0", cbMap.transform).SetSprite(spriteDB.GetWorldSprite(worldSprite, 0));
                ScreenElement.BuildSprite("Map 1", cbMap.transform).SetSprite(spriteDB.GetWorldSprite(worldSprite, 1)).SetPosition(0, 32);
                ScreenElement.BuildSprite("Map 2", cbMap.transform).SetSprite(spriteDB.GetWorldSprite(worldSprite, 2)).SetPosition(32, 32);
                ScreenElement.BuildSprite("Map 3", cbMap.transform).SetSprite(spriteDB.GetWorldSprite(worldSprite, 3)).SetPosition(32, 0);
            }
            else {
                cbMap.SetSize(32, 32);
                ScreenElement.BuildSprite("Map 0", cbMap.transform).SetSprite(spriteDB.GetWorldSprite(worldSprite, 0));
            }
            return cbMap;
        }

        /// <summary>
        /// Returns one of the three seeds of this game at random.
        /// </summary>
        public int GetRandomSavedSeed() => SavedGame.RandomSeed.GetRandomElement();

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
            List<Digimon> allDigimon = new List<Digimon>();
            foreach (Digimon d in Database.Digimons) {
                if (d.stage == stage && logicMgr.GetDigimonUnlocked(d.name)) {
                    allDigimon.Add(d);
                }
            }
            return allDigimon.OrderBy(d => d.order).Select(d => d.name).ToList();
        }
        public List<string> GetAllUnlockedSpiritsOfElement(Element element) {
            List<string> allDigimon = new List<string>();
            foreach (Digimon d in Database.Digimons) {
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
            foreach (Digimon d in Database.Digimons) {
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
            foreach (Digimon d in Database.Digimons) {
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
                foreach (Digimon d in Database.Digimons) {
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
                foreach (Digimon d in Database.Digimons) {
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
                foreach (Digimon d in Database.Digimons) {
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
        public void UpdateLeaverBuster(uint expLoss, string digimonLoss) {
            SavedGame.IsLeaverBusterActive = true;
            SavedGame.LeaverBusterExpLoss = expLoss;
            SavedGame.LeaverBusterDigimonLoss = digimonLoss;
        }
        /// <summary>
        /// Disables LeaverBuster.
        /// </summary>
        public void DisableLeaverBuster() {
            SavedGame.IsLeaverBusterActive = false;
            SavedGame.LeaverBusterExpLoss = 0;
            SavedGame.LeaverBusterDigimonLoss = "";
        }

        #region Animations
        public void EnqueueAnimation(IEnumerator animation) => screenMgr.EnqueueAnimation(animation);

        public void EnqueueRewardAnimation(Reward reward, string objective, object resultBefore, object resultAfter) {
            switch(reward) {
                case Reward.Empty:
                    EnqueueAnimation(Animations.RewardEmpty());
                    EnqueueAnimation(Animations.CharSad());
                    break;
                case Reward.IncreaseDistance300:
                case Reward.IncreaseDistance500:
                case Reward.IncreaseDistance2000:
                    EnqueueAnimation(Animations.RewardDistance(true, (int)resultBefore, (int)resultAfter));
                    EnqueueAnimation(Animations.CharSad());
                    break;
                case Reward.ReduceDistance500:
                case Reward.ReduceDistance1000:
                    EnqueueAnimation(Animations.RewardDistance(false, (int)resultBefore, (int)resultAfter));
                    EnqueueAnimation(Animations.CharHappy());
                    break;
                case Reward.PunishDigimon:
                    if((int)resultAfter == -1) {
                        EnqueueAnimation(Animations.EraseDigimon(objective));
                    }
                    else {
                        EnqueueAnimation(Animations.LevelDownDigimon(objective));
                    }
                    EnqueueAnimation(Animations.CharSad());
                    break;
                case Reward.RewardDigimon:
                    if ((int)resultBefore == -1) {
                        EnqueueAnimation(Animations.SummonDigimon(objective));
                        EnqueueAnimation(Animations.UnlockDigimon(objective));
                    }
                    else {
                        EnqueueAnimation(Animations.SummonDigimon(objective));
                        EnqueueAnimation(Animations.LevelUpDigimon(objective));
                    }
                    EnqueueAnimation(Animations.CharHappy());
                    break;
                case Reward.UnlockDigicodeOwned:
                    string code = Database.GetDigimon(objective).code;
                    EnqueueAnimation(Animations.RewardCode(objective, code));
                    EnqueueAnimation(Animations.CharHappy());
                    break;
                case Reward.UnlockDigicodeNotOwned:
                    string code2 = Database.GetDigimon(objective).code;
                    EnqueueAnimation(Animations.RewardCode(objective, code2));
                    EnqueueAnimation(Animations.UnlockDigimon(objective));
                    EnqueueAnimation(Animations.CharHappy());
                    break;
                case Reward.DataStorm:
                    EnqueueAnimation(Animations.DataStorm(spriteDB.GetCharacterSprites(PlayerChar), (bool)resultBefore));
                    if((bool)resultBefore) {
                        EnqueueAnimation(Animations.DisplayNewArea(WorldMgr.CurrentWorld, WorldMgr.CurrentArea, WorldMgr.CurrentDistance));
                    }
                    else {
                        EnqueueAnimation(Animations.CharHappy());
                    }
                    break;
                case Reward.LoseSpiritPower10:
                case Reward.LoseSpiritPower50:
                    EnqueueAnimation(Animations.RewardSpiritPower(true, (int)resultBefore, (int)resultAfter));
                    EnqueueAnimation(Animations.CharSad());
                    break;
                case Reward.GainSpiritPower10:
                case Reward.GainSpiritPowerMax:
                    EnqueueAnimation(Animations.RewardSpiritPower(false, (int)resultBefore, (int)resultAfter));
                    EnqueueAnimation(Animations.CharHappy());
                    break;
                case Reward.LevelDown:
                case Reward.ForceLevelDown:
                    EnqueueAnimation(Animations.LevelDown((int)resultBefore, (int)resultAfter));
                    EnqueueAnimation(Animations.CharSad());
                    break;
                case Reward.LevelUp:
                case Reward.ForceLevelUp:
                    EnqueueAnimation(Animations.LevelUp((int)resultBefore, (int)resultAfter));
                    EnqueueAnimation(Animations.CharHappy());
                    break;
            }
        }

        public void CompleteWorld(int world) {
            if (world == 0) CompleteWorld0();
        }
        private void CompleteWorld0() {
            WorldMgr.MoveToArea(1, 0);
            IsCharacterDefeated = true;
            EnqueueAnimation(Animations.TransitionToMap1(PlayerChar));
        }

        #endregion
    }
}