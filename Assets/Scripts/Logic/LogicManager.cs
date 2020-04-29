//All menus should have their option set when they are open, not when they are closed (i.e. when you open the 'MainMenu', it is first set to be in the 'Map' tab).

using Kaisa.Digivice.Extensions;
using System;
using UnityEngine;

namespace Kaisa.Digivice {
    public class LogicManager : MonoBehaviour {
        private GameManager gm;
        private AudioManager audioMgr;
        private ScreenManager screenMgr;
        public void AssignManagers(GameManager gm) {
            this.gm = gm;
            this.screenMgr = gm.screenMgr;
            this.audioMgr = gm.audioMgr;
        }

        public Screen currentScreen = Screen.Character;
        public MainMenu currentMainMenu = MainMenu.Map;
        //Submenues for the Game app.
        public GameMenu currentGameMenu = GameMenu.Reward;
        public GameRewardMenu currentGameRewardMenu = GameRewardMenu.FindBattle;
        public GameTravelMenu currentGameTravelMenu = GameTravelMenu.SpeedRunner;

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
                    currentGameMenu = 0;
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
                if (currentGameMenu == GameMenu.Reward) {
                    currentGameRewardMenu = 0;
                    currentScreen = Screen.GamesRewardMenu;
                }
                else {
                    currentGameTravelMenu = 0;
                    currentScreen = Screen.GamesTravelMenu;
                }
            }
            else if (currentScreen == Screen.GamesRewardMenu) {
                audioMgr.PlayButtonA();
                //Open whatever.
            }
            else if (currentScreen == Screen.GamesTravelMenu) {
                if (currentGameTravelMenu == GameTravelMenu.SpeedRunner) {
                    audioMgr.PlayButtonA();
                    OpenApp(gm.pAppSpeedRunner);
                }
                else if (currentGameTravelMenu == GameTravelMenu.Maze) {
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
                NavigateMenu(ref currentGameMenu, Direction.Left);
            }
            else if (currentScreen == Screen.GamesRewardMenu) {
                audioMgr.PlayButtonA();
                NavigateMenu(ref currentGameRewardMenu, Direction.Left);
            }
            else if (currentScreen == Screen.GamesTravelMenu) {
                audioMgr.PlayButtonA();
                NavigateMenu(ref currentGameTravelMenu, Direction.Left);
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
                NavigateMenu(ref currentGameMenu, Direction.Right);
            }
            else if (currentScreen == Screen.GamesRewardMenu) {
                audioMgr.PlayButtonA();
                NavigateMenu(ref currentGameRewardMenu, Direction.Right);
            }
            else if (currentScreen == Screen.GamesTravelMenu) {
                audioMgr.PlayButtonA();
                NavigateMenu(ref currentGameTravelMenu, Direction.Right);
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
    }
}