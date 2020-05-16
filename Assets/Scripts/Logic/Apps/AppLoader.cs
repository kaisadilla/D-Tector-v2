using Kaisa.Digivice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice.Apps {
    public enum App {
        Map,
        Status,
        Database,
        CodeInput,
        Camp,
        Connect,
        Finder,
        Battle,
        JackpotBox,
        EnergyWars,
        DigiCatch,
        SpeedRunner,
        Asteroids,
        DigiHunter,
        Maze
    }
    public class AppLoader : MonoBehaviour {
        [SerializeField] private GameManager gm;
        private Transform RootParent => gm.RootParent;
        //Regular apps
        [SerializeField] private GameObject pAppMap;
        [SerializeField] private GameObject pAppStatus;
        [SerializeField] private GameObject pAppDatabase;
        [SerializeField] private GameObject pAppDigits;
        [SerializeField] private GameObject pAppCamp;
        [SerializeField] private GameObject pAppConnect;
        //Games
        [SerializeField] private GameObject pAppFinder;
        [SerializeField] private GameObject pAppBattle;
        [SerializeField] private GameObject pAppJackpotBox;
        [SerializeField] private GameObject pAppEnergyWars;
        [SerializeField] private GameObject pAppDigiCatch;
        [SerializeField] private GameObject pAppSpeedRunner;
        [SerializeField] private GameObject pAppAsteroids;
        [SerializeField] private GameObject pAppDigiHunter;
        [SerializeField] private GameObject pAppMaze;

        public T LoadApp<T>(App app, IAppController controller) where T : DigiviceApp {
            GameObject appGO;
            DigiviceApp appController;

            switch (app) {
                case App.Map:
                    appGO = Instantiate(pAppMap, gm.RootParent);
                    break;
                case App.Status:
                    appGO = Instantiate(pAppStatus, gm.RootParent);
                    break;
                case App.Database:
                    appGO = Instantiate(pAppDatabase, gm.RootParent);
                    break;
                case App.CodeInput:
                    appGO = Instantiate(pAppDigits, gm.RootParent);
                    break;
                case App.Camp:
                    appGO = Instantiate(pAppCamp, gm.RootParent);
                    break;
                case App.Finder:
                    appGO = Instantiate(pAppFinder, gm.RootParent);
                    break;
                case App.Battle:
                    appGO = Instantiate(pAppBattle, gm.RootParent);
                    break;
                case App.JackpotBox:
                    appGO = Instantiate(pAppJackpotBox, gm.RootParent);
                    break;
                case App.SpeedRunner:
                    appGO = Instantiate(pAppSpeedRunner, gm.RootParent);
                    break;
                case App.DigiHunter:
                    appGO = Instantiate(pAppDigiHunter, gm.RootParent);
                    break;
                case App.Maze:
                    appGO = Instantiate(pAppMaze, gm.RootParent);
                    break;
                default:
                    return null;
            }

            appController = appGO.GetComponent<DigiviceApp>();
            appController.Setup(gm, controller);

            return appController as T;
        }
    }
}
