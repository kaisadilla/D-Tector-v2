//All menus should have their option set when they are open, not when they are closed (i.e. when you open the 'MainMenu', it is first set to be in the 'Map' tab).

using Kaisa.Digivice.Extensions;
using System;
using UnityEngine;

namespace Kaisa.Digivice {
    public class LogicManager : MonoBehaviour {
        private GameManager gm;
        private AudioManager audioMgr;
        private SavedGame loadedGame;
        public void Initialize(GameManager gm, SavedGame loadedGame) {
            this.gm = gm;
            this.loadedGame = loadedGame;

            audioMgr = gm.audioMgr;
        }

        public Screen currentScreen = Screen.Character;
        public MainMenu currentMainMenu = MainMenu.Map;
        //Submenues for the Game app.
        public int gamesMenuIndex = 0;
        public int gamesRewardMenuIndex = 0;
        public int gamesTravelMenuIndex = 0;

        public App.DigiviceApp loadedApp;

        #region Input Management
        public void InputA() {
            if (currentScreen == Screen.Character) {
                audioMgr.PlayButtonA();
                OpenGameMenu();
            }
            else if (currentScreen == Screen.MainMenu) {
                if (currentMainMenu == MainMenu.Map) {
                    audioMgr.PlayButtonA();
                    OpenApp(gm.pAppMap);
                }
                else if (currentMainMenu == MainMenu.Status) {
                    audioMgr.PlayButtonA();
                    OpenApp(gm.pAppStatus);
                }
                else if (currentMainMenu == MainMenu.Game) {
                    audioMgr.PlayButtonA();
                    gamesMenuIndex = 0;
                    currentScreen = Screen.GamesMenu;
                }
                else if (currentMainMenu == MainMenu.Database) {
                    audioMgr.PlayButtonA();
                    OpenApp(gm.pAppDatabase);
                }
                else if (currentMainMenu == MainMenu.Digits) {
                    audioMgr.PlayButtonA();
                    OpenApp(gm.pAppDigits);
                }
            }
            else if (currentScreen == Screen.App) {
                loadedApp.InputA();
            }
            else if (currentScreen == Screen.GamesMenu) {
                audioMgr.PlayButtonA();
                if (gamesMenuIndex == 0) {
                    //TODO: This is only temporary. Find Battle is not exactly Battle.
                    audioMgr.PlayButtonA();
                    OpenAppBattle();
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

            }
            else if (currentScreen == Screen.GamesTravelMenu) {
                if (gamesTravelMenuIndex == 0) {
                    audioMgr.PlayButtonA();
                    OpenApp(gm.pAppSpeedRunner);
                }
                else if (gamesTravelMenuIndex == 3) {
                    audioMgr.PlayButtonA();
                    OpenApp(gm.pAppMaze);
                }
            }
        }

        public void InputB() {
            if (currentScreen == Screen.Character) {
                audioMgr.PlayButtonB();
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
        }

        public void InputLeft() {
            if (currentScreen == Screen.App) {
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
        }

        public void InputRight() {
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
        }
        #endregion
        private void OpenGameMenu() {
            currentMainMenu = 0;
            currentScreen = Screen.MainMenu;
        }

        private void CloseGameMenu() {
            currentScreen = Screen.Character;
        }
        private void NavigateMenu<T>(ref T menuEnum, Direction dir) where T : struct {
            if (!typeof(T).IsEnum) {
                throw new ArgumentException($"{typeof(T).FullName} is not an enum.");
            }

            if (dir == Direction.Left) {
                menuEnum.SetLast();
            }
            else if (dir == Direction.Right) {
                menuEnum.SetNext();
            }
        }
        public void FinalizeApp(Screen newScreen = Screen.MainMenu) {
            currentScreen = newScreen;
            loadedApp.Dispose();
            loadedApp = null;
        }

        private void OpenApp(GameObject appPrefab) {
            currentScreen = Screen.App;
            loadedApp = App.DigiviceApp.LoadApp(appPrefab, gm);
        }

        private void OpenAppBattle() {
            currentScreen = Screen.App;
            Digimon randomDigimon = gm.DatabaseMgr.GetWeightedDigimon(GetPlayerLevel());
            loadedApp = App.DigiviceApp.LoadApp(gm.pAppBattle, gm, randomDigimon.name, "false");
        }

        #region Player and game stats
        /// <summary>
        /// Adds (or substracts) an amount of experience to the player, and returns true if their level changed.
        /// This method will trigger player insurance if able.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool AddPlayerExperience(int val) {
            int playerLevelBefore = GetPlayerLevel();
            //If the player has insurance and they would lose experience, don't do anything.
            if(!(val < 0 && loadedGame.IsPlayerInsured)) {
                loadedGame.PlayerExperience += val;
                if (loadedGame.PlayerExperience < 0) loadedGame.PlayerExperience = 0;
            }

            int playerLevelNow = GetPlayerLevel();
            //If the player has lost a level, activate their insurance.
            if(playerLevelNow < playerLevelBefore) {
                loadedGame.IsPlayerInsured = true;
            }
            //Else, toggle it off, regardless of whether they won or lost experience this time.
            else {
                loadedGame.IsPlayerInsured = false;
            }

            return (playerLevelBefore != playerLevelNow);
        }
        /// <summary>
        /// Returns the level of a player based on its experience.
        /// </summary>
        public int GetPlayerLevel() {
            int playerXP = loadedGame.PlayerExperience;
            if (playerXP == 0) return 1;

            float level = Mathf.Pow(playerXP, 1f / 3f);
            return Mathf.FloorToInt(level);
        }
        /// <summary>
        /// Forcibly levels up the player, settings its experience to the necessary amount so the player is leveled up.
        /// </summary>
        public void LevelUpPlayer() {
            int playerLevel = GetPlayerLevel();
            float nextLevelExp = Mathf.Pow(playerLevel + 1, 3f);
            loadedGame.PlayerExperience = Mathf.CeilToInt(nextLevelExp);
        }
        public int SpiritPower {
            get => loadedGame.SpiritPower;
            set {
                int totalSpiritPower = value;
                if (totalSpiritPower > Constants.MAX_SPIRIT_POWER) totalSpiritPower = 99;
                if (totalSpiritPower < 0) totalSpiritPower = 0;
                loadedGame.SpiritPower = totalSpiritPower;
            }
        }
        public int TotalBattles => loadedGame.TotalBattles;
        public int TotalWins => loadedGame.TotalWins;
        public float WinPercentage => TotalWins / (float)TotalBattles;
        /// <summary>
        /// Increases the total battle count by 1 and returns the new value.
        /// </summary>
        public int IncreaseTotalBattles() => ++loadedGame.TotalBattles;
        /// <summary>
        /// Increases the total win count by 1 and returns the new value.
        /// </summary>
        public int IncreaseTotalWins() => ++loadedGame.TotalWins;
        #endregion

        #region Digimon data
        /// <summary>
        /// If true, sets the digimon as unlocked, as long as it isn't unlocked yet. If false, sets the digimon as locked regardless of their level.
        /// </summary>
        public void SetDigimonUnlocked(string digimon, bool val) {
            if (val == true) {
                if (loadedGame.GetDigimonLevel(digimon) == 0) {
                    loadedGame.SetDigimonLevel(digimon, 1);

                    string[] ddocks = gm.GetAllDDockDigimons();

                    for (int i = 0; i < ddocks.Length; i++) {
                        if (ddocks[i] == "") {
                            SetDDockDigimon(i, digimon);
                            break;
                        }
                    }
                }
            }
            else {
                loadedGame.SetDigimonLevel(digimon, 0);
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
        public bool GetDigimonUnlocked(string digimon) => loadedGame.GetDigimonLevel(digimon) > 0;
        /// <summary>
        /// Sets the level of a Digimon. This method accounts for the maximum level the Digimon can have. This method can't be used to lock a Digimon.
        /// </summary>
        public void SetDigimonExtraLevel(string digimon, int val) {
            int maxExtraLevel = gm.DatabaseMgr.GetDigimon(digimon).MaxExtraLevel;
            if (val > maxExtraLevel) val = maxExtraLevel;
            if (val < 1) val = 1;
            loadedGame.SetDigimonLevel(digimon, val + 1);
        }
        /// <summary>
        /// Returns the extra level of a Digimon. Returns -1 if the Digimon is not unlocked.
        /// </summary>
        /// <param name="digimon"></param>
        /// <returns></returns>
        public int GetDigimonExtraLevel(string digimon) => loadedGame.GetDigimonLevel(digimon) - 1;
        public void SetDigimonCodeUnlocked(string name, bool val) => loadedGame.SetDigimonCodeUnlocked(name, val);
        public bool GetDigimonCodeUnlocked(string name) => loadedGame.GetDigimonCodeUnlocked(name);
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

        public string GetDDockDigimon(int ddock) => loadedGame.GetDDockDigimon(ddock);
        public void SetDDockDigimon(int ddock, string digimon) {
            if (ddock > 3) return; //The player only has 4 D-Docks.
            loadedGame.SetDDockDigimon(ddock, digimon);
        }
        #endregion

        #region Calculations
        /// <summary>
        /// Returns the amount of experience the winner of a battle takes from the loser.
        /// </summary>
        /// <param name="friendlyLevel">The level of the victor.</param>
        /// <param name="enemyLevel">The level of the loser.</param>
        /// <returns></returns>
        public int GetExperienceGained(int friendlyLevel, int enemyLevel) {
            float a = 30 * enemyLevel;
            float b = Mathf.Pow((2 * enemyLevel) + 10, 2.5f);
            float c = Mathf.Pow(enemyLevel + friendlyLevel + 10, 2.5f);
            float d = 0.025f + (0.025f * friendlyLevel);
            if (d > 0.5f) d = 0.5f;
            float expGained = ((a * (b / c)) + 1) * d;

            return Mathf.CeilToInt(expGained);
        }
        #endregion
    }
}