using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Kaisa.CircularTypes;
using UnityEngine.Timeline;
using Kaisa.Digivice.Extensions;
using System;

namespace Kaisa.Digivice {
    public class AppMap : MonoBehaviour, IDigiviceApp {
        private enum ScreenMap {
            Map,
            ChoosingArea,
            ViewingDistance
        }

        [Header("UI Elements")]
        [SerializeField]
        private Image screenDisplay;

        private GameManager gm;
        private AudioManager audioMgr;
        private SpriteDatabase spriteDB;

        private int loadedGameSector; //The sector the player is already in, in the loaded game.
        //The map, sector and area the player is visualizing.
        private int currentMap; //The current map the player is in (i.e. standard map, area 13 map, royal knights' map...)
        private int currentSector; //The sector of the map, which is called 'map' in game.
        private int currentArea;
        private ScreenMap currentScreen = ScreenMap.Map;
        //Map screen:
        private RectangleBuilder currentAreaMarker;
        //ChoosingArea screen:
        private int hoveredArea;
        private RectangleBuilder hoveredMarker;
        private SpriteBuilder hoveredAreaName;
        //ChoosingDistance screen:
        private SpriteBuilder distanceScreen;

        private List<RectangleBuilder> markerPoints = new List<RectangleBuilder>();

        public static AppMap LoadApp(GameManager gm) {
            GameObject appGO = Instantiate(gm.pAppMap, gm.mainScreen.transform);
            AppMap appMap = appGO.GetComponent<AppMap>();
            appMap.Initialize(gm);
            return appMap;
        }
        public void Initialize(GameManager gm) {
            this.gm = gm;
            audioMgr = gm.audioMgr;
            spriteDB = gm.spriteDB;
            StartApp();
        }
        public void Dispose() {
            Destroy(gameObject);
        }

        private void StartApp() {
            LoadDataFromGame();
        }
        private void CloseApp(bool closeMenu) {
            gm.logicMgr.FinalizeApp(closeMenu);
        }

        public void InputA() {
            if (currentScreen == ScreenMap.Map) {
                audioMgr.PlayButtonA();
                OpenAreaSelection();
            }
            else if (currentScreen == ScreenMap.ChoosingArea) {
                audioMgr.PlayButtonA();
                OpenViewDistance();
            }
            else if (currentScreen == ScreenMap.ViewingDistance) {
                audioMgr.PlayButtonA();
                ChooseArea();
            }
        }
        public void InputB() {
            if (currentScreen == ScreenMap.Map) {
                audioMgr.PlayButtonB();
                CloseApp(false);
            }
            else if (currentScreen == ScreenMap.ChoosingArea) {
                audioMgr.PlayButtonB();
                CloseAreaSelection();
            }
            else if (currentScreen == ScreenMap.ViewingDistance) {
                audioMgr.PlayButtonB();
                CloseViewDistance();
            }
        }
        public void InputLeft() {
            if (currentScreen == ScreenMap.Map) {
                audioMgr.PlayButtonA();
                NavigateMap(Direction.Left);
            }
            else if(currentScreen == ScreenMap.ChoosingArea) {
                audioMgr.PlayButtonA();
                NavigateAreas(Direction.Left);
            }
            else if(currentScreen == ScreenMap.ViewingDistance) {
                audioMgr.PlayButtonB();
            }
        }
        public void InputRight() {
            if (currentScreen == ScreenMap.Map) {
                audioMgr.PlayButtonA();
                NavigateMap(Direction.Right);
            }
            else if (currentScreen == ScreenMap.ChoosingArea) {
                audioMgr.PlayButtonA();
                NavigateAreas(Direction.Right);
            }
            else if (currentScreen == ScreenMap.ViewingDistance) {
                audioMgr.PlayButtonB();
            }
        }

        private void LoadDataFromGame() {
            currentMap = gm.DistanceMgr.CurrentMap;
            currentArea = gm.DistanceMgr.CurrentArea;

            //TODO: Create all maps.
            if (currentMap == 0) { //Starting map.
                currentSector = Mathf.FloorToInt(currentArea / 3f);
                loadedGameSector = currentSector;
            }

            UpdateScreen();
        }

        private void NavigateMap(Direction dir) {
            ClearMarkers();
            if(currentMap == 0) {
                int newSector;
                if (dir == Direction.Left) newSector = currentSector.CircularAdd(-1, 3);
                else if (dir == Direction.Right) newSector = currentSector.CircularAdd(1, 3);
                else return;

                Direction animDirection = Direction.Left;

                if (dir == Direction.Left) {
                    if (currentSector == 0) {
                        animDirection = Direction.Left;
                    }
                    else if (currentSector == 1) {
                        animDirection = Direction.Down;
                    }
                    else if (currentSector == 2) {
                        animDirection = Direction.Right;
                    }
                    else if (currentSector == 3) {
                        animDirection = Direction.Up;
                    }
                }
                else if (dir == Direction.Right) {
                    if (currentSector == 0) {
                        animDirection = Direction.Up;
                    }
                    else if (currentSector == 1) {
                        animDirection = Direction.Left;
                    }
                    else if (currentSector == 2) {
                        animDirection = Direction.Down;
                    }
                    else if (currentSector == 3) {
                        animDirection = Direction.Right;
                    }
                }
                StartCoroutine(AnimateMap(animDirection, currentMap, currentSector, newSector));
                currentSector = newSector;
            }
        }

        private IEnumerator AnimateMap(Direction dir, int currentMap, int currentSector, int newSector) {
            gm.LockInput();
            Sprite mapCurrentSprite = spriteDB.GetMapSectorSprites(currentMap)[currentSector];
            SpriteBuilder sMapCurrent = gm.CreateSprite("AnimCurrentSector", gm.MainScreenTransform, posX: 0, posY: 0, sprite: mapCurrentSprite);

            Sprite mapNewSprite = spriteDB.GetMapSectorSprites(currentMap)[newSector];
            SpriteBuilder sMapNew = gm.CreateSprite("AnimNewSector", gm.MainScreenTransform, sprite: mapNewSprite);
            sMapNew.PlaceOutside(dir.OppositeDirection());

            float animDuration = 1.5f;
            for(int i = 0; i < 32; i++) {
                sMapCurrent.MoveSprite(dir);
                sMapNew.MoveSprite(dir);
                yield return new WaitForSeconds(animDuration / 32f);
            }

            sMapCurrent.Dispose();
            sMapNew.Dispose();

            UpdateScreen();
            gm.UnlockInput();
        }
        private void UpdateScreen() {
            if(currentMap == 0) {
                screenDisplay.sprite = spriteDB.GetMapSectorSprites(currentMap)[currentSector];
            }
            ShowCompleteAreas();
        }
        private void ShowCompleteAreas() {
            ClearMarkers(); //Destroy all current markers.
            //If sector is between 0 and 3, it is a sector of map 0, containing 3 areas.
            if(currentSector >= 0 && currentSector <= 3) {
                int firstArea = currentSector * 3;
                for(int i = firstArea; i <= firstArea + 2; i++) {
                    if(gm.DistanceMgr.GetAreaCompleted(currentMap, i)) {
                        Vector2Int markerPos = Constants.areaPositions[currentMap][i];
                        RectangleBuilder marker = gm.CreateRectangle("Area" + i + "Marker", screenDisplay.transform, 2, 2, markerPos.x, markerPos.y);
                        markerPoints.Add(marker);
                    }
                    //If this area is the current area, use a flickering point to mark it. This point is drawn on top of the marker that indicates the area has been completed.
                    if (currentArea == i) {
                        Vector2Int markerPos = Constants.areaPositions[currentMap][i];
                        currentAreaMarker = gm.CreateRectangle("CurrentAreaMarker", screenDisplay.transform, 2, 2, markerPos.x, markerPos.y, 250);
                        markerPoints.Add(currentAreaMarker);
                    }
                }
            }
        }

        private void ClearMarkers() {
            foreach (RectangleBuilder r in markerPoints) r.Dispose();
            markerPoints.Clear();
        }

        private void OpenAreaSelection() {
            currentScreen = ScreenMap.ChoosingArea;
            if (currentAreaMarker != null) currentAreaMarker.SetActive(false);

            //The marker that indicates the area that is being chosen.
            hoveredMarker = gm.CreateRectangle("OptionMarker", screenDisplay.transform, 2, 2, flickPeriod: 250);
            hoveredAreaName = gm.CreateSprite("AreaName", screenDisplay.transform, 28, 5);

            if(currentMap == 0) {
                hoveredArea = (currentSector == loadedGameSector) ? currentArea : currentSector * 3;
                hoveredMarker.SetPosition(Constants.areaPositions[currentMap][hoveredArea]);

                if (currentSector == 0 || currentSector == 3) {
                    hoveredAreaName.SetPosition(2, 1);
                }
                else {
                    hoveredAreaName.SetPosition(2, 26);
                }

                hoveredAreaName.SetSprite(spriteDB.GetMapAreaSprites(currentMap)[hoveredArea]);
            }
        }
        private void NavigateAreas(Direction dir) {
            if(currentMap == 0) {
                if(dir == Direction.Left) {
                    hoveredArea = hoveredArea.CircularAdd(-1, (currentSector * 3) + 2, currentSector * 3);
                }
                else {
                    hoveredArea = hoveredArea.CircularAdd(1, (currentSector * 3) + 2, currentSector * 3);
                }
                hoveredMarker.SetPosition(Constants.areaPositions[currentMap][hoveredArea]);
                hoveredAreaName.SetSprite(spriteDB.GetMapAreaSprites(currentMap)[hoveredArea]);
            }
        }
        private void CloseAreaSelection() {
            currentScreen = ScreenMap.Map;
            hoveredMarker.Dispose();
            hoveredAreaName.Dispose();
            if (currentAreaMarker != null) currentAreaMarker.SetActive(true);
        }

        private void OpenViewDistance() {
            currentScreen = ScreenMap.ViewingDistance;
            //If the area chosen is the area the player is already in, the distance will not change. Otherwise, get the distance for the new area.
            int areaDist = (hoveredArea == currentArea) ? gm.DistanceMgr.CurrentDistance : gm.DistanceMgr.Distances[currentMap][hoveredArea];

            distanceScreen = gm.CreateSprite("DistanceScreen", screenDisplay.transform, sprite: spriteDB.map_distanceScreen);
            gm.CreateTextBox("Distance", distanceScreen.transform, areaDist.ToString(), DFont.Regular, 25, 5, 6, 25, TextAnchor.UpperRight);
        }

        private void CloseViewDistance() {
            currentScreen = ScreenMap.ChoosingArea;
            distanceScreen.Dispose();
        }

        private void ChooseArea() {
            if (hoveredArea != currentArea) gm.DistanceMgr.MoveToArea(currentMap, hoveredArea);
            CloseApp(true);
        }
    }
}