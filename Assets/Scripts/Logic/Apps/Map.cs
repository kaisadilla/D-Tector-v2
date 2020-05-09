using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.App {
    public class Map : DigiviceApp {
        private int currentScreen = 0; //0: map, 1: choosing area, 2: viewing distance.

        //Map and area information.
        private World thisWorldData;
        private int originalWorld; //The world the player actually is in the game. Only for reference, since the player can't travel between worlds by himself.
        private int originalArea; //The area the player actually is in the game.
        private int originalMap; //The map of the area the player actually is in the game.

        private int displayMap;
        private int displayArea;
        private int[] areasInCurrentMap;

        //Map screen:
        private ContainerBuilder cbMap;
        private RectangleBuilder currentAreaMarker;

        //ChoosingArea screen:
        private int currentArea; //The position of the array areasInCurrentMap the player is currently selecting.
        private int SelectedArea => areasInCurrentMap[currentArea];
        private RectangleBuilder hoveredMarker;
        private TextBoxBuilder hoveredAreaName;

        private int OriginalAreaIndexInCurrentMap {
            get {
                for(int i = 0; i < areasInCurrentMap.Length; i++) {
                    if (areasInCurrentMap[i] == originalArea) return i;
                }
                return 0;
            }
        }

        //ChoosingDistance screen:
        private SpriteBuilder distanceScreen;

        private List<RectangleBuilder> completedMarkers = new List<RectangleBuilder>();

        #region Input
        public override void InputA() {
            if (currentScreen == 0) {
                audioMgr.PlayButtonA();
                OpenAreaSelection();
            }
            else if (currentScreen == 1) {
                audioMgr.PlayButtonA();
                OpenViewDistance();
            }
            else if (currentScreen == 2) {
                audioMgr.PlayButtonA();
                ChooseArea();
            }
        }
        public override void InputB() {
            if (currentScreen == 0) {
                audioMgr.PlayButtonB();
                CloseApp();
            }
            else if (currentScreen == 1) {
                audioMgr.PlayButtonB();
                CloseAreaSelection();
            }
            else if (currentScreen == 2) {
                audioMgr.PlayButtonB();
                CloseViewDistance();
            }
        }
        public override void InputLeft() {
            if (currentScreen == 0) {
                if(thisWorldData.lockTravel || !thisWorldData.multiMap) {
                    audioMgr.PlayButtonB();
                }
                else {
                    audioMgr.PlayButtonA();
                    NavigateMap(Direction.Left);
                }
            }
            else if (currentScreen == 1) {
                if(thisWorldData.lockTravel) {
                    audioMgr.PlayButtonB();
                }
                else {
                    audioMgr.PlayButtonA();
                    NavigateAreaSelection(Direction.Left);
                }
            }
            else if (currentScreen == 2) {
                audioMgr.PlayButtonB();
            }
        }
        public override void InputRight() {
            if (currentScreen == 0) {
                if (thisWorldData.lockTravel || !thisWorldData.multiMap) {
                    audioMgr.PlayButtonB();
                }
                else {
                    audioMgr.PlayButtonA();
                    NavigateMap(Direction.Right);
                }
            }
            else if (currentScreen == 1) {
                if (thisWorldData.lockTravel) {
                    audioMgr.PlayButtonB();
                }
                else {
                    audioMgr.PlayButtonA();
                    NavigateAreaSelection(Direction.Right);
                }
            }
            else if (currentScreen == 2) {
                audioMgr.PlayButtonB();
            }
        }
        #endregion

        protected override void StartApp() {
            LoadInitialMapData();
            DrawMap();
        }

        private void LoadInitialMapData() {
            thisWorldData = gm.WorldMgr.CurrentWorldData;
            originalWorld = thisWorldData.number;
            originalMap = gm.WorldMgr.CurrentMap;
            originalArea = gm.WorldMgr.CurrentArea;
            displayArea = originalArea;
            displayMap = originalMap;
            areasInCurrentMap = thisWorldData.GetAreasInMap(displayMap);
        }

        private void DrawMap() {
            cbMap = gm.BuildMapScreen(thisWorldData.number, Parent);
            FocusCurrentMap();
            DrawAreaMarkers(true);
        }

        private void FocusCurrentMap() {
            switch (displayMap) {
                case 0:
                    cbMap.SetPosition(0, 0);
                    break;
                case 1:
                    cbMap.SetPosition(0, -32);
                    break;
                case 2:
                    cbMap.SetPosition(-32, -32);
                    break;
                case 3:
                    cbMap.SetPosition(-32, 0);
                    break;
            }
            ClearMarkers();
            DrawAreaMarkers(true);
        }

        private void NavigateMap(Direction dir) {
            int mapBefore = displayMap;

            if (dir == Direction.Left) displayMap = displayMap.CircularAdd(-1, 3);
            else if (dir == Direction.Right) displayMap = displayMap.CircularAdd(1, 3);

            areasInCurrentMap = thisWorldData.GetAreasInMap(displayMap);

            gm.EnqueueAnimation(gm.screenMgr.ATravelMap(originalWorld, mapBefore, displayMap, 1.5f));
            FocusCurrentMap();
        }

        private void DrawAreaMarkers(bool displayOriginalArea) {
            ClearMarkers(); //Destroy all current markers.

            int[] shownAreas = thisWorldData.GetAreasInMap(displayMap);

            foreach (int i in shownAreas) { 
                if(gm.WorldMgr.GetAreaCompleted(originalWorld, i)) {
                    RectangleBuilder marker = ScreenElement.BuildRectangle($"Area {i} Marker", Parent)
                        .SetSize(2, 2).SetPosition(thisWorldData.areas[i].coords);
                    completedMarkers.Add(marker);
                }
                if(displayOriginalArea) {
                    if(originalArea == i) {
                        currentAreaMarker = ScreenElement.BuildRectangle($"Current Area Marker", Parent)
                            .SetSize(2, 2).SetPosition(thisWorldData.areas[i].coords).SetFlickPeriod(0.25f);
                        completedMarkers.Add(currentAreaMarker);
                    }
                }
            }
        }
        private void ClearMarkers() {
            foreach (RectangleBuilder r in completedMarkers) {
                r.Dispose();
            }
            completedMarkers.Clear();
        }

        private void OpenAreaSelection() {
            currentScreen = 1;
            //If the player is entering the map he already is in, start hovering in his current area, rather than the 'area 0' of that map.
            currentArea = (displayMap == originalMap) ? OriginalAreaIndexInCurrentMap : 0;

            if (currentAreaMarker != null) currentAreaMarker.SetActive(false);

            //The marker that indicates the area that is being chosen.
            hoveredMarker = ScreenElement.BuildRectangle("OptionMarker", screenDisplay.transform).SetSize(2, 2).SetFlickPeriod(0.25f)
                .SetPosition(thisWorldData.areas[SelectedArea].coords);
            hoveredAreaName = ScreenElement.BuildTextBox("AreaName", screenDisplay.transform, DFont.Small).SetText("area").SetPosition(28, 5);

            if (displayMap == 0 || displayMap == 3) {
                hoveredAreaName.SetPosition(2, 1);
            }
            else {
                hoveredAreaName.SetPosition(2, 26);
            }

            hoveredAreaName.Text = string.Format("area{0:00}", SelectedArea + 1); //+1 because, in game, areas start at #1, not 0.
        }
        private void NavigateAreaSelection(Direction dir) {
            if(dir == Direction.Left) {
                currentArea = currentArea.CircularAdd(-1, areasInCurrentMap.Length - 1);
            }
            else {
                currentArea = currentArea.CircularAdd(1, areasInCurrentMap.Length - 1);
            }
            hoveredMarker.SetPosition(thisWorldData.areas[SelectedArea].coords);
            hoveredAreaName.Text = string.Format("area{0:00}", SelectedArea + 1);
        }
        private void CloseAreaSelection() {
            currentScreen = 0;
            hoveredMarker.Dispose();
            hoveredAreaName.Dispose();
            if (currentAreaMarker != null) currentAreaMarker.SetActive(true);
        }

        private void OpenViewDistance() {
            currentScreen = 2;
            //If the area chosen is the area the player is already in, the distance will not change. Otherwise, get the distance for the new area.

            int areaDist = (SelectedArea == originalArea) ? gm.WorldMgr.CurrentDistance : thisWorldData.areas[SelectedArea].distance;

            distanceScreen = ScreenElement.BuildSprite("DistanceScreen", screenDisplay.transform).SetSprite(gm.spriteDB.map_distanceScreen);
            ScreenElement.BuildTextBox("Distance", distanceScreen.transform, DFont.Regular)
                .SetText(areaDist.ToString()).SetSize(25, 5).SetPosition(6, 25).SetAlignment(TextAnchor.UpperRight);
        }
        private void CloseViewDistance() {
            currentScreen = 1;
            distanceScreen.Dispose();
        }

        private void ChooseArea() {
            if (SelectedArea != currentArea) gm.WorldMgr.MoveToArea(originalWorld, SelectedArea);
            CloseApp(Screen.Character);
        }
    }
}