﻿//All menus should have their option set when they are open, not when they are closed (i.e. when you open the 'MainMenu', it is first set to be in the 'Map' tab).

using Kaisa.Digivice.Extensions;
using Kaisa.Digivice.Apps;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Kaisa.Digivice {
    public class LogicManager : MonoBehaviour, IAppController {
        private GameManager gm;
        private AudioManager audioMgr;
        public void Initialize(GameManager gm) {
            this.gm = gm;

            audioMgr = gm.audioMgr;
        }

        public Screen currentScreen = Screen.Character;
        public MainMenu currentMainMenu = MainMenu.Map;
        //Submenues for the Game app.
        public int charSelectionIndex = 0;
        public int gamesMenuIndex = 0;
        public int gamesRewardMenuIndex = 0;
        public int gamesTravelMenuIndex = 0;

        public Apps.DigiviceApp loadedApp;
        public bool IsAppLoaded => loadedApp != null;
        public bool ShakeDisabled() {
            if (loadedApp != null && !(loadedApp is Apps.Status)) return true;
            else if (gm.IsCharacterDefeated) return true;
            else if (IsEventPending) return true;
            return false;
        }

        //Trigger event.
        public bool IsEventPending { get; private set; } = false;
        public delegate void TriggerEvent();
        public TriggerEvent triggerEvent;

        #region Input Management
        public void InputA() {
            if (currentScreen == Screen.Character) {
                if (IsEventPending) {
                    audioMgr.PlayButtonA();
                    triggerEvent();
                    return;
                }
                audioMgr.PlayButtonA();
                OpenGameMenu();
            }
            else if (currentScreen == Screen.MainMenu) {
                if (currentMainMenu == MainMenu.Camp) {
                    audioMgr.PlayButtonA();
                    OpenCamp();
                }
                else if (gm.IsCharacterDefeated) {
                    audioMgr.PlayButtonB();
                }
                else if (currentMainMenu == MainMenu.Map) {
                    audioMgr.PlayButtonA();
                    OpenMap();
                }
                else if (currentMainMenu == MainMenu.Status) {
                    audioMgr.PlayButtonA();
                    OpenStatus();
                }
                else if (currentMainMenu == MainMenu.Game) {
                    audioMgr.PlayButtonA();
                    gamesMenuIndex = 0;
                    currentScreen = Screen.GamesMenu;
                }
                else if (currentMainMenu == MainMenu.Database) {
                    audioMgr.PlayButtonA();
                    OpenDatabase();
                }
                else if (currentMainMenu == MainMenu.Digits) {
                    audioMgr.PlayButtonA();
                    OpenDigits();
                }
            }
            else if (currentScreen == Screen.App) {
                loadedApp.InputA();
            }
            else if (currentScreen == Screen.GamesMenu) {
                audioMgr.PlayButtonA();
                if (gamesMenuIndex == 0) {
                    audioMgr.PlayButtonA();
                    OpenFinder();
                }
                else if (gamesMenuIndex == 1) {
                    gamesRewardMenuIndex = 0;
                    currentScreen = Screen.GamesRewardMenu;
                }
                else {
                    gamesTravelMenuIndex = 0;
                    currentScreen = Screen.GamesTravelMenu;
                }
            }
            else if (currentScreen == Screen.GamesRewardMenu) {
                if (gamesRewardMenuIndex == 0) {
                    audioMgr.PlayButtonA();
                    OpenJackpotBox();
                }

            }
            else if (currentScreen == Screen.GamesTravelMenu) {
                if (gamesTravelMenuIndex == 0) {
                    audioMgr.PlayButtonA();
                    OpenSpeedRunner();
                }
                else if (gamesTravelMenuIndex == 2) {
                    audioMgr.PlayButtonA();
                    OpenDigiHunter();
                }
                else if (gamesTravelMenuIndex == 3) {
                    audioMgr.PlayButtonA();
                    OpenMaze();
                }
            }
            else if (currentScreen == Screen.CharSelection) {
                audioMgr.PlayButtonA();
                SelectCharacterAndCreateGame();
            }
        }
        public void InputB() {
            if (currentScreen == Screen.Character) {
                if (IsEventPending) {
                    audioMgr.PlayButtonB();
                    triggerEvent();
                    return;
                }
                else if (gm.WorldMgr.CurrentDistance == 1) { //Alternative to shaking the phone to trigger a boss battle.
                    gm.TakeAStep();
                }
                else {
                    audioMgr.PlayButtonB();
                }
            }
            else if (currentScreen == Screen.MainMenu) {
                audioMgr.PlayButtonB();
                CloseGameMenu();
            }
            else if (currentScreen == Screen.App) {
                loadedApp.InputB();
            }
            else if (currentScreen == Screen.GamesMenu) {
                audioMgr.PlayButtonB();
                currentScreen = Screen.MainMenu;
            }
            else if (currentScreen == Screen.GamesRewardMenu) {
                audioMgr.PlayButtonB();
                currentScreen = Screen.GamesMenu;
            }
            else if (currentScreen == Screen.GamesTravelMenu) {
                audioMgr.PlayButtonB();
                currentScreen = Screen.GamesMenu;
            }
            else if (currentScreen == Screen.CharSelection) {
                audioMgr.PlayButtonB();
            }
        }
        public void InputLeft() {
            if (currentScreen == Screen.App) {
                if (IsEventPending) {
                    audioMgr.PlayButtonA();
                    triggerEvent();
                    return;
                }
                loadedApp.InputLeft();
            }
            else if (currentScreen == Screen.Character) {
                audioMgr.PlayButtonA();
                OpenGameMenu();
            }
            else if (currentScreen == Screen.MainMenu) {
                audioMgr.PlayButtonA();
                NavigateMenu(ref currentMainMenu, Direction.Left);
            }
            else if (currentScreen == Screen.GamesMenu) {
                audioMgr.PlayButtonA();
                gamesMenuIndex = gamesMenuIndex.CircularAdd(-1, 2);
            }
            else if (currentScreen == Screen.GamesRewardMenu) {
                audioMgr.PlayButtonA();
                gamesRewardMenuIndex = gamesRewardMenuIndex.CircularAdd(-1, 2);
            }
            else if (currentScreen == Screen.GamesTravelMenu) {
                audioMgr.PlayButtonA();
                gamesTravelMenuIndex = gamesTravelMenuIndex.CircularAdd(-1, 3);
            }
            else if (currentScreen == Screen.CharSelection) {
                audioMgr.PlayButtonA();
                charSelectionIndex = charSelectionIndex.CircularAdd(-1, 5);
            }
        }
        public void InputRight() {
            if (IsEventPending) {
                audioMgr.PlayButtonA();
                triggerEvent();
                return;
            }
            if (currentScreen == Screen.App) {
                loadedApp.InputRight();
            }
            else if (currentScreen == Screen.Character) {
                audioMgr.PlayButtonA();
                OpenGameMenu();
            }
            else if (currentScreen == Screen.MainMenu) {
                audioMgr.PlayButtonA();
                NavigateMenu(ref currentMainMenu, Direction.Right);
            }
            else if (currentScreen == Screen.GamesMenu) {
                audioMgr.PlayButtonA();
                gamesMenuIndex = gamesMenuIndex.CircularAdd(1, 2);
            }
            else if (currentScreen == Screen.GamesRewardMenu) {
                audioMgr.PlayButtonA();
                gamesRewardMenuIndex = gamesRewardMenuIndex.CircularAdd(1, 2);
            }
            else if (currentScreen == Screen.GamesTravelMenu) {
                audioMgr.PlayButtonA();
                gamesTravelMenuIndex = gamesTravelMenuIndex.CircularAdd(1, 3);
            }
            else if (currentScreen == Screen.CharSelection) {
                audioMgr.PlayButtonA();
                charSelectionIndex = charSelectionIndex.CircularAdd(1, 5);
            }
        }
        //Down
        public void InputADown() {
            if (currentScreen == Screen.App) loadedApp.InputADown();
        }
        public void InputBDown() {
            if (currentScreen == Screen.App) loadedApp.InputBDown();
        }
        public void InputLeftDown() {
            if (currentScreen == Screen.App) loadedApp.InputLeftDown();
        }
        public void InputRightDown() {
            if (currentScreen == Screen.App) loadedApp.InputRightDown();
        }
        //Up
        public void InputAUp() {
            if (currentScreen == Screen.App) loadedApp.InputAUp();
        }
        public void InputBUp() {
            if (currentScreen == Screen.App) loadedApp.InputBUp();
        }
        public void InputLeftUp() {
            if (currentScreen == Screen.App) loadedApp.InputLeftUp();
        }
        public void InputRightUp() {
            if (currentScreen == Screen.App) loadedApp.InputRightUp();
        }
        #endregion
        public void EnqueueRegularEvent() {
            if (loadedApp == null) currentScreen = Screen.Character;

            audioMgr.PlaySound(audioMgr.triggerEvent);
            IsEventPending = true;

            if(Random.Range(0f, 1f) < 0.85f) {
                triggerEvent = CallRandomBattleForEvent;
            }
            else {
                triggerEvent = TriggerDataStorm;
            }

            triggerEvent += () => {
                SavedGame.SavedEvent = 0;
                IsEventPending = false;
            };
        }
        public void EnqueueBossEvent() {
            if (loadedApp == null) currentScreen = Screen.Character;

            audioMgr.PlaySound(audioMgr.triggerEvent);
            IsEventPending = true;
            triggerEvent = CallBossBattle;
            triggerEvent += () => {
                SavedGame.SavedEvent = 0;
                IsEventPending = false;
            };
        }

        public void CallRandomBattleForEvent() => CallRandomBattle(false);
        public void CallRandomBattle(bool reduceDistance) {
            currentScreen = Screen.App;
            Digimon randomDigimon = Database.GetRandomDigimonForBattle(GetPlayerLevel());
            loadedApp = gm.appLoader.LoadApp<Battle>(App.Battle, this).Initialize(randomDigimon.name, reduceDistance);
            loadedApp.StartApp();
        }

        public void CallBossBattle() {
            currentScreen = Screen.App;
            string boss = gm.WorldMgr.GetBossOfCurrentArea();
            loadedApp = gm.appLoader.LoadApp<Battle>(App.Battle, this).Initialize(boss, true, true);
            loadedApp.StartApp();
        }
        public void CallFixedBattle(string digimon, bool alterDistance, bool isBossBattle) {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<Battle>(App.Battle, this).Initialize(digimon, alterDistance, isBossBattle);
            loadedApp.StartApp();
        }

        public void SelectCharacterAndCreateGame() {
            gm.CreateNewGame((GameChar)charSelectionIndex);
        }

        private void OpenGameMenu() {
            currentMainMenu = 0;
            currentScreen = Screen.MainMenu;
        }

        private void CloseGameMenu() {
            currentScreen = Screen.Character;
        }
        private void NavigateMenu<T>(ref T menuEnum, Direction dir) where T : struct {
            if (!typeof(T).IsEnum) {
                throw new System.ArgumentException($"{typeof(T).FullName} is not an enum.");
            }

            if (dir == Direction.Left) {
                menuEnum.SetLast();
            }
            else if (dir == Direction.Right) {
                menuEnum.SetNext();
            }
        }

        private void OpenMap() {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<Map>(App.Map, this);
            loadedApp.StartApp();
        }
        private void OpenStatus() {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<Status>(App.Status, this);
            loadedApp.StartApp();
        }
        private void OpenDatabase() {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<DatabaseApp>(App.Database, this);
            loadedApp.StartApp();
        }
        private void OpenDigits() {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<CodeInput>(App.CodeInput, this).Initialize(false);
            loadedApp.StartApp();
        }
        private void OpenCamp() {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<Camp>(App.Camp, this);
            loadedApp.StartApp();
        }
        private void OpenFinder() {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<Finder>(App.Finder, this);
            loadedApp.StartApp();
        }
        private void OpenJackpotBox() {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<JackpotBox>(App.JackpotBox, this);
            loadedApp.StartApp();
        }
        private void OpenSpeedRunner() {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<SpeedRunner>(App.SpeedRunner, this);
            loadedApp.StartApp();
        }
        private void OpenDigiHunter() {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<DigiHunter>(App.DigiHunter, this);
            loadedApp.StartApp();
        }
        private void OpenMaze() {
            currentScreen = Screen.App;
            loadedApp = gm.appLoader.LoadApp<Maze>(App.Maze, this);
            loadedApp.StartApp();
        }

        public void CloseLoadedApp(Screen newScreen = Screen.MainMenu) {
            if (loadedApp is CodeInput ci) {
                string digimon = ci.ReturnedDigimon;
                if (digimon != null) {
                    gm.logicMgr.SetDigimonUnlocked(digimon, true);
                    gm.logicMgr.SetDigicodeUnlocked(digimon, true);

                    gm.EnqueueAnimation(Animations.SummonDigimon(digimon));
                    gm.EnqueueAnimation(Animations.UnlockDigimon(digimon));
                    gm.EnqueueAnimation(Animations.CharHappy());
                }
            }

            if (IsEventPending) currentScreen = Screen.Character;
            else currentScreen = newScreen;

            loadedApp.Dispose();
            loadedApp = null;
            gm.CheckPendingEvents();
        }

        #region Player and game stats
        /// <summary>
        /// Adds an amount of experience to the player, and returns true if their level changed.
        /// This method will disable player insurance if able.
        /// </summary>
        /// <param name="val">The unsigned amount to add.</param>
        /// <returns></returns>
        public bool AddPlayerExperience(uint val) {
            int playerLevelBefore = GetPlayerLevel();

            SavedGame.PlayerExperience += (int)val;
            if (SavedGame.PlayerExperience > 1_000_000) SavedGame.PlayerExperience = 1_000_000;

            int playerLevelNow = GetPlayerLevel();

            SavedGame.IsPlayerInsured = false;
            return (playerLevelBefore != playerLevelNow);
        }
        /// <summary>
        /// Substracts an amount of experience to the player, and returns true if their level changed.
        /// This method will trigger player insurance if able.
        /// </summary>
        /// <param name="val">The unsigned amount to substract.</param>
        /// <returns></returns>
        public bool RemovePlayerExperience(uint val) {
            int playerLevelBefore = GetPlayerLevel();

            if (SavedGame.IsPlayerInsured) {
                SavedGame.IsPlayerInsured = false;
            }
            else {
                SavedGame.PlayerExperience -= (int)val;
            }

            if (SavedGame.PlayerExperience < 0) SavedGame.PlayerExperience = 0;

            int playerLevelNow = GetPlayerLevel();

            //If the player has lost a level, activate their insurance.
            if (playerLevelNow < playerLevelBefore) {
                SavedGame.IsPlayerInsured = true;
            }
            return (playerLevelBefore != playerLevelNow);
        }
        public int PlayerExperience => SavedGame.PlayerExperience;
        /// <summary>
        /// Returns the level of a player based on its experience.
        /// </summary>
        public int GetPlayerLevel() {
            int playerXP = SavedGame.PlayerExperience;
            if (playerXP == 0) return 1;

            float level = Mathf.Pow(playerXP, 1f / 3f);
            return Mathf.FloorToInt(level);
        }
        /// <summary>
        /// Returns the percentage of the experience the player has compared to the experience needed to the next level
        /// (where 0% is the base experience for their level, not 0 experience).
        /// </summary>
        /// <returns></returns>
        public float GetPlayerLevelProgression() {
            int floorExperience = Mathf.FloorToInt(Mathf.Pow(GetPlayerLevel(), 3));
            int topExperience = Mathf.FloorToInt(Mathf.Pow(GetPlayerLevel() + 1, 3));

            int maxExperienceForLevel = topExperience - floorExperience;
            int playerExperienceForLevel = PlayerExperience - floorExperience;

            return playerExperienceForLevel / (float)maxExperienceForLevel;
        }
        /// <summary>
        /// Forcibly levels up the player, settings its experience to the necessary amount so the player is leveled up.
        /// </summary>
        public void LevelUpPlayer() {
            int playerLevel = GetPlayerLevel();
            float nextLevelExp = Mathf.Pow(playerLevel + 1, 3f);
            SavedGame.PlayerExperience = Mathf.CeilToInt(nextLevelExp);
        }
        /// <summary>
        /// Forcibly levels down the player, reducing its experience by an amount equal to the next level's experience minus this level's experience.
        /// </summary>
        public void LevelDownPlayer() {
            int playerLevel = GetPlayerLevel();
            float lastLevelExperience = Mathf.Pow(playerLevel - 1, 3f);
            float thisLevelExperience = Mathf.Pow(playerLevel, 3f);
            float nextLevelExperience = Mathf.Pow(playerLevel + 1, 3f);
            SavedGame.PlayerExperience -= (int)(nextLevelExperience - thisLevelExperience);
            if (SavedGame.PlayerExperience < lastLevelExperience) SavedGame.PlayerExperience = (int)lastLevelExperience;
        }
        public int SpiritPower {
            get => SavedGame.SpiritPower;
            set {
                int totalSpiritPower = value;
                if (totalSpiritPower > Constants.MAX_SPIRIT_POWER) totalSpiritPower = 99;
                if (totalSpiritPower < 0) totalSpiritPower = 0;
                SavedGame.SpiritPower = totalSpiritPower;
            }
        }
        public int TotalBattles => SavedGame.TotalBattles;
        public int TotalWins => SavedGame.TotalWins;
        public float WinPercentage {
            get {
                if (TotalBattles == 0) return 0f;
                else return TotalWins / (float)TotalBattles;
            }
        }
        /// <summary>
        /// Increases the total battle count by 1 and returns the new value.
        /// </summary>
        public int IncreaseTotalBattles() => ++SavedGame.TotalBattles;
        /// <summary>
        /// Increases the total win count by 1 and returns the new value.
        /// </summary>
        public int IncreaseTotalWins() => ++SavedGame.TotalWins;
        #endregion

        #region Digimon data
        /// <summary>
        /// If true, sets the digimon as unlocked, as long as it isn't unlocked yet. If false, sets the digimon as locked regardless of their level.
        /// </summary>
        public void SetDigimonUnlocked(string digimon, bool val) {
            if (val == true) {
                if (SavedGame.GetDigimonLevel(digimon) == 0) {
                    SavedGame.SetDigimonLevel(digimon, 1);
                    VisualDebug.WriteLine("Unlocked digimon: " + digimon);

                    Stage digimonStage = Database.GetDigimon(digimon).stage;

                    if(digimonStage != Stage.Spirit && digimonStage != Stage.Armor) {
                        string[] ddocks = gm.GetAllDDockDigimons();

                        for (int i = 0; i < ddocks.Length; i++) {
                            if (ddocks[i] == null || ddocks[i] == "") {
                                SetDDockDigimon(i, digimon);
                                break;
                            }
                        }
                    }

                }
            }
            else {
                SavedGame.SetDigimonLevel(digimon, 0);
                string[] ddocks = gm.GetAllDDockDigimons();

                for (int i = 0; i < ddocks.Length; i++) {
                    if(ddocks[i] == digimon) {
                        SetDDockDigimon(i, "");
                    }
                }
            }
        }
        /// <summary>
        /// Returns true if the player has unlocked that digimon
        /// </summary>
        public bool GetDigimonUnlocked(string digimon) => SavedGame.GetDigimonLevel(digimon) > 0;
        /// <summary>
        /// Sets the level of a Digimon. This method accounts for the maximum level the Digimon can have. This method can't be used to lock a Digimon.
        /// </summary>
        public void SetDigimonExtraLevel(string digimon, int val) {
            int maxExtraLevel = Database.GetDigimon(digimon).MaxExtraLevel;
            if (val > maxExtraLevel) val = maxExtraLevel;
            if (val < 1) val = 1;
            SavedGame.SetDigimonLevel(digimon, val + 1);
        }
        /// <summary>
        /// Returns an array of the names of every unlocked Digimon.
        /// </summary>
        public Digimon[] GetAllUnlockedDigimon() {
            List<Digimon> allUnlockedDigimon = new List<Digimon>();
            foreach(Digimon d in Database.Digimons) {
                if (GetDigimonUnlocked(d.name)) allUnlockedDigimon.Add(d);
            }
            return allUnlockedDigimon.ToArray();
        }
        /// <summary>
        /// Returns the extra level of a Digimon. Returns -1 if the Digimon is not unlocked.
        /// </summary>
        /// <param name="digimon"></param>
        /// <returns></returns>
        public int GetDigimonExtraLevel(string digimon) => SavedGame.GetDigimonLevel(digimon) - 1;
        /// <summary>
        /// Returns true if the player already has that Digimon at the maximum level.
        /// </summary>
        public bool IsDigimonAtMaxLevel(string digimon) {
            return GetDigimonExtraLevel(digimon) == Database.GetDigimon(digimon).MaxExtraLevel;
        }
        public void SetDigicodeUnlocked(string name, bool val) => SavedGame.SetDigicodeUnlocked(name, val);
        public bool GetDigicodeUnlocked(string name) => SavedGame.GetDigicodeUnlocked(name);
        /// <summary>
        /// Unlocks or levels up a Digimon. Returns true if it levels up a Digimon, false if it unlocks it.
        /// It also outputs the level before and after being rewarded.
        /// </summary>
        public bool RewardDigimon(string digimon, out int levelBefore, out int levelAfter) {
            levelBefore = GetDigimonExtraLevel(digimon);
            //If the player has the digimon already, level it up.
            if(GetDigimonUnlocked(digimon)) {
                SetDigimonExtraLevel(digimon, levelBefore + 1);
                levelAfter = GetDigimonExtraLevel(digimon);
                VisualDebug.WriteLine($"The Digimon was rewarded by increasing its level from {levelBefore} to {levelAfter}");
                return true;
            }
            //Else, unlock it.
            else {
                SetDigimonUnlocked(digimon, true);
                levelAfter = 0;
                VisualDebug.WriteLine($"The Digimon was rewarded by unlocking it.");
                return false;
            }
        }
        /// <summary>
        /// Erases or levels down a Digimon. Returns true if it levels down a Digimon, false if it erases it.
        /// It also outputs the level before and after being punished.
        /// </summary>
        public bool PunishDigimon(string digimon, out int levelBefore, out int levelAfter) {
            levelBefore = GetDigimonExtraLevel(digimon);
            //If the player has the digimon at level 1 or higher, level it down.
            if (GetDigimonExtraLevel(digimon) > 0) {
                SetDigimonExtraLevel(digimon, levelBefore - 1);
                levelAfter = GetDigimonExtraLevel(digimon);
                Debug.Log($"The Digimon was punished by decreasing its level from {levelBefore} to {levelAfter}");
                return true;
            }
            //Else, erase it.
            else {
                SetDigimonUnlocked(digimon, false);
                levelAfter = -1;
                Debug.Log($"The Digimon was punished by locking it.");
                return false;
            }
        }
        /// <summary>
        /// Locks a Spirit and adds it to the list of spirits lost by the player.
        /// </summary>
        public void LoseSpirit(string spirit) {
            SetDigimonUnlocked(spirit, false);
            SavedGame.LostSpirits.Add(spirit);
        }
        /// <summary>
        /// Unlocks a random Spirit from the list of spirits lost and returns the name of that Spirit.
        /// </summary>
        public string RecoverSpirit() {
            int index = SavedGame.LostSpirits.GetRandomIndex();
            string recoveredSpirit = SavedGame.LostSpirits[index];
            SavedGame.LostSpirits.RemoveAt(index);

            SetDigimonUnlocked(recoveredSpirit, true);

            return recoveredSpirit;
        }
        /// <summary>
        /// Returns true if the player has any spirit lost that they can recover.
        /// </summary>
        public bool IsAnySpiritLost => SavedGame.LostSpirits.Count > 0;

        public string GetDDockDigimon(int ddock) => SavedGame.DDockDigimon[ddock];
        public void SetDDockDigimon(int ddock, string digimon) {
            if (ddock > 3) return; //The player only has 4 D-Docks.
            SavedGame.DDockDigimon[ddock] = digimon;
        }
        public bool IsDDockEmpty(int ddock) => (SavedGame.DDockDigimon[ddock] == "");

        public string[] GetAllDDockDigimon() {
            List<string> notEmptyDDocks = new List<string>();
            string[] allDDocks = SavedGame.DDockDigimon;
            foreach(string s in allDDocks) {
                if (s != null && s != "") notEmptyDDocks.Add(s);
            }
            return notEmptyDDocks.ToArray();
        }
        #endregion

        /// <summary>
        /// Applies a reward, to an objective if necessary. Outputs the result before and after the reward is applied.
        /// </summary>
        /// <param name="objective">The Digimon that will be punished, if able, etc...</param>
        /// <param name="resultBefore">A variable holding the result before the reward was applied.</param>
        /// <param name="resultAfter">A variable holding the result after tthe reward is applied.</param>
        public void ApplyReward(Reward reward, string objective, out object resultBefore, out object resultAfter) {
            resultBefore = null;
            resultAfter = null;

            if (reward == Reward.IncreaseDistance300) {
                resultBefore = gm.WorldMgr.CurrentDistance;
                gm.WorldMgr.IncreaseDistance(300);
                resultAfter = gm.WorldMgr.CurrentDistance;
            }
            else if (reward == Reward.IncreaseDistance500) {
                resultBefore = gm.WorldMgr.CurrentDistance;
                gm.WorldMgr.IncreaseDistance(500);
                resultAfter = gm.WorldMgr.CurrentDistance;
            }
            else if (reward == Reward.IncreaseDistance2000) {
                resultBefore = gm.WorldMgr.CurrentDistance;
                gm.WorldMgr.IncreaseDistance(2000);
                resultAfter = gm.WorldMgr.CurrentDistance;
            }
            else if (reward == Reward.ReduceDistance500) {
                resultBefore = gm.WorldMgr.CurrentDistance;
                gm.WorldMgr.ReduceDistance(500);
                resultAfter = gm.WorldMgr.CurrentDistance;
            }
            else if (reward == Reward.ReduceDistance1000) {
                resultBefore = gm.WorldMgr.CurrentDistance;
                gm.WorldMgr.ReduceDistance(1000);
                resultAfter = gm.WorldMgr.CurrentDistance;
            }
            else if (reward == Reward.PunishDigimon) {
                PunishDigimon(objective, out int pdBef, out int pdAft);
                resultBefore = pdBef;
                resultAfter = pdAft;
            }
            else if (reward == Reward.RewardDigimon) {
                Rarity rarity;
                float rng = Random.Range(0f, 1f);
                if (rng < 0.50f) rarity = Rarity.Common;
                else if (rng < 0.80f) rarity = Rarity.Rare;
                else if (rng < 0.95f) rarity = Rarity.Epic;
                else rarity = Rarity.Legendary;
                string rewardedDigimon;
                //30% chance to forcibly select a Digimon already owned.
                if (Random.Range(0f, 1f) < 0.3f) {
                    rewardedDigimon = GetAllUnlockedDigimon().Where(d => d.Rarity == rarity).GetRandomElement().name;
                }
                //Reward any Digimon, owned or not.
                else {
                    rewardedDigimon = Database.GetAllDigimonOfRarity(rarity, gm.logicMgr.GetPlayerLevel() + 20).GetRandomElement().name;
                }

                RewardDigimon(rewardedDigimon, out int rdBef, out int rdAft);
                resultBefore = rdBef;
                resultAfter = rdAft;
            }
            else if (reward == Reward.UnlockDigicodeOwned) {
                Digimon[] ownedDigimon = gm.logicMgr.GetAllUnlockedDigimon();
                SetDigicodeUnlocked(ownedDigimon.GetRandomElement().name, true);
            }
            else if (reward == Reward.UnlockDigicodeNotOwned) {
                Rarity rarity;
                float rng = Random.Range(0f, 1f);
                if (rng < 0.50f) rarity = Rarity.Common;
                else if (rng < 0.80f) rarity = Rarity.Rare;
                else if (rng < 0.95f) rarity = Rarity.Epic;
                else rarity = Rarity.Legendary;
                string rewardedDigimon2 = Database.GetAllDigimonOfRarity(rarity, 100).GetRandomElement().name;
                SetDigimonUnlocked(rewardedDigimon2, true);
                SetDigicodeUnlocked(rewardedDigimon2, true);
            }
            else if (reward == Reward.DataStorm) {
                bool moved = ApplyDataStorm(out int newArea);
                resultBefore = moved; //resultBefore is a boolean true if the player was moved.
                resultAfter = newArea;
            }
            else if (reward == Reward.LoseSpiritPower10) {
                resultBefore = SpiritPower;
                SpiritPower -= 10;
                resultAfter = SpiritPower;
            }
            else if (reward == Reward.LoseSpiritPower50) {
                resultBefore = SpiritPower;
                SpiritPower -= 50;
                resultAfter = SpiritPower;
            }
            else if (reward == Reward.GainSpiritPower10) {
                resultBefore = SpiritPower;
                SpiritPower += 10;
                resultAfter = SpiritPower;
            }
            else if (reward == Reward.GainSpiritPowerMax) {
                resultBefore = SpiritPower;
                SpiritPower = Constants.MAX_SPIRIT_POWER;
                resultAfter = SpiritPower;
            }
            else if (reward == Reward.LevelDown) {
                if (GetPlayerLevelProgression() < 0.5f) {
                    resultBefore = GetPlayerLevel();
                    LevelDownPlayer();
                    resultAfter = GetPlayerLevel();
                }
            }
            else if (reward == Reward.ForceLevelDown) {
                if (GetPlayerLevelProgression() > 0f) {
                    resultBefore = GetPlayerLevel();
                    LevelDownPlayer();
                    resultAfter = GetPlayerLevel();
                }
            }
            else if (reward == Reward.LevelUp) {
                if (GetPlayerLevelProgression() < 0.5f) {
                    resultBefore = GetPlayerLevel();
                    LevelDownPlayer();
                    resultAfter = GetPlayerLevel();
                }
            }
            else if (reward == Reward.ForceLevelUp) {
                if (GetPlayerLevelProgression() > 0f) {
                    resultBefore = GetPlayerLevel();
                    LevelUpPlayer();
                    resultAfter = GetPlayerLevel();
                }
            }
            else if (reward == Reward.TriggerBattle) {
                CallRandomBattle(true);
            }
        }
        private void TriggerDataStorm() {
            bool move = ApplyDataStorm(out _);
            gm.EnqueueAnimation(Animations.DataStorm(gm.spriteDB.GetCharacterSprites(gm.PlayerChar), move));
        }
        /// <summary>
        /// Triggers a Datastorm, and returns true if the player has been moved. It outputs the new area.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool ApplyDataStorm(out int newArea) {
            newArea = gm.WorldMgr.CurrentArea;

            float rng = UnityEngine.Random.Range(0f, 1f);
            bool moveArea = rng < 0.33f;
            VisualDebug.WriteLine($"Data storm rng: {rng}, move area: {moveArea}");

            List<int> uncompletedAreas = gm.WorldMgr.GetUncompletedAreas(gm.WorldMgr.CurrentWorld);

            if (uncompletedAreas.Count < 2) {
                moveArea = false;
            }

            if (moveArea) {
                newArea = uncompletedAreas.GetRandomElement();
                gm.WorldMgr.MoveToArea(gm.WorldMgr.CurrentWorld, newArea, gm.WorldMgr.CurrentDistance + 1000);
                //TODO: Chance to be moved to world 9.
            }
            return moveArea;
        }

        #region Calculations
        /// <summary>
        /// Returns the amount of experience the winner of a battle takes from the loser.
        /// </summary>
        /// <param name="friendlyLevel">The level of the victor.</param>
        /// <param name="enemyLevel">The level of the loser.</param>
        /// <returns></returns>
        public uint GetExperienceGained(int friendlyLevel, int enemyLevel) {
            float a = 30 * enemyLevel;
            float b = Mathf.Pow((2 * enemyLevel) + 10, 2.5f);
            float c = Mathf.Pow(enemyLevel + friendlyLevel + 10, 2.5f);
            float d = 0.025f + (0.025f * friendlyLevel);
            if (d > 0.5f) d = 0.5f;
            float expGained = ((a * (b / c)) + 1) * d;

            return (uint)Mathf.CeilToInt(expGained);
        }
        #endregion
    }
}