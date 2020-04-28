//All menus should have their option set when they are open, not when they are closed (i.e. when you open the 'MainMenu', it is first set to be in the 'Map' tab).

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Kaisa.Digivice.Extensions;

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

        public IDigiviceApp loadedApp;

        #region Input Management
        public void InputA() {
            if (currentScreen == Screen.Character) {
                audioMgr.PlayButtonA();
                OpenGameMenu();
            }
            else if (currentScreen == Screen.MainMenu) {
                if (currentMainMenu == MainMenu.Map) {
                    audioMgr.PlayButtonA();
                    OpenMap();
                }
                else if (currentMainMenu == MainMenu.Status) {
                    audioMgr.PlayButtonA();
                    OpenStatus();
                }
                else if (currentMainMenu == MainMenu.Game) {
                    audioMgr.PlayButtonA();
                    currentGameMenu = 0;
                    currentScreen = Screen.GameMenu;
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
            else if (currentScreen == Screen.GameMenu) {
                audioMgr.PlayButtonA();
                if (currentGameMenu == GameMenu.Reward) {
                    currentGameRewardMenu = 0;
                    currentScreen = Screen.GameRewardMenu;
                }
                else {
                    currentGameTravelMenu = 0;
                    currentScreen = Screen.GameTravelmenu;
                }
            }
            else if (currentScreen == Screen.GameRewardMenu) {
                audioMgr.PlayButtonA();
                //Open whatever.
            }
            else if (currentScreen == Screen.GameTravelmenu) {
                audioMgr.PlayButtonA();
                //Open whatever.
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
            else if (currentScreen == Screen.GameMenu) {
                audioMgr.PlayButtonB();
                currentScreen = Screen.MainMenu;
            }
            else if (currentScreen == Screen.GameRewardMenu) {
                audioMgr.PlayButtonB();
                currentScreen = Screen.GameMenu;
            }
            else if (currentScreen == Screen.GameTravelmenu) {
                audioMgr.PlayButtonB();
                currentScreen = Screen.GameMenu;
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
            else if (currentScreen == Screen.GameMenu) {
                audioMgr.PlayButtonA();
                NavigateMenu(ref currentGameMenu, Direction.Left);
            }
            else if (currentScreen == Screen.GameRewardMenu) {
                audioMgr.PlayButtonA();
                NavigateMenu(ref currentGameRewardMenu, Direction.Left);
            }
            else if (currentScreen == Screen.GameTravelmenu) {
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
            else if (currentScreen == Screen.GameMenu) {
                audioMgr.PlayButtonA();
                NavigateMenu(ref currentGameMenu, Direction.Right);
            }
            else if (currentScreen == Screen.GameRewardMenu) {
                audioMgr.PlayButtonA();
                NavigateMenu(ref currentGameRewardMenu, Direction.Right);
            }
            else if (currentScreen == Screen.GameTravelmenu) {
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
        public void FinalizeApp(bool closeMenu) {
            if (closeMenu) {
                currentScreen = Screen.Character;
            }
            else {
                currentScreen = Screen.MainMenu;
            }
            loadedApp.Dispose();
            loadedApp = null;
        }

        private void OpenMap() {
            currentScreen = Screen.App;
            loadedApp = AppMap.LoadApp(gm);
        }

        private void OpenStatus() {
            currentScreen = Screen.App;
            loadedApp = AppStatus.LoadApp(gm);
        }
        private void OpenDatabase() {
            currentScreen = Screen.App;
            loadedApp = AppDatabase.LoadApp(gm);
        }
        private void OpenDigits() {
            currentScreen = Screen.App;
            loadedApp = AppDigits.LoadApp(gm);
        }
    }
}