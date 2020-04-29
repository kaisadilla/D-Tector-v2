using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.App {
    public class Map : DigiviceApp {
        private enum ScreenMap { //TODO: Replace with an int.
            Map,
            ChoosingArea,
            ViewingDistance
        }
        private ScreenMap currentScreen = ScreenMap.Map;

        //Map and area information.
        private int loadedGameSector; //The sector the player is already in, in the loaded game.
        //The map, sector and area the player is visualizing.
        private int currentMap; //The current map the player is in (i.e. standard map, area 13 map, royal knights' map...)
        private int currentSector; //The sector of the map, which is called 'map' in game.
        private int currentArea;

        //Map screen:
        private RectangleBuilder currentAreaMarker;

        //ChoosingArea screen:
        private int hoveredArea;
        private RectangleBuilder hoveredMarker;
        private TextBoxBuilder hoveredAreaName;

        //ChoosingDistance screen:
        private SpriteBuilder distanceScreen;

        private List<RectangleBuilder> markerPoints = new List<RectangleBuilder>();

        #region Input
        public override void InputA() {
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
        public override void InputB() {
            if (currentScreen == ScreenMap.Map) {
                audioMgr.PlayButtonB();
                CloseApp();
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
        public override void InputLeft() {
            if (currentScreen == ScreenMap.Map) {
                audioMgr.PlayButtonA();
                NavigateMap(Direction.Left);
            }
            else if (currentScreen == ScreenMap.ChoosingArea) {
                audioMgr.PlayButtonA();
                NavigateAreaSelection(Direction.Left);
            }
            else if (currentScreen == ScreenMap.ViewingDistance) {
                audioMgr.PlayButtonB();
            }
        }
        public override void InputRight() {
            if (currentScreen == ScreenMap.Map) {
                audioMgr.PlayButtonA();
                NavigateMap(Direction.Right);
            }
            else if (currentScreen == ScreenMap.ChoosingArea) {
                audioMgr.PlayButtonA();
                NavigateAreaSelection(Direction.Right);
            }
            else if (currentScreen == ScreenMap.ViewingDistance) {
                audioMgr.PlayButtonB();
            }
        }
        #endregion

        protected override void StartApp() {
            SetMapData();
            DrawScreen();
        }
        private void DrawScreen() {
            if (currentMap == 0) {
                screenDisplay.sprite = gm.spriteDB.GetMapSectorSprites(currentMap)[currentSector];
            }
            DrawAreaMarkers();
        }

        /// <summary>
        /// Retrieves and sets various parameters about the map.
        /// </summary>
        private void SetMapData() {
            currentMap = gm.DistanceMgr.CurrentMap;
            currentArea = gm.DistanceMgr.CurrentArea;

            //TODO: Create all maps.
            if (currentMap == 0) { //Starting map.
                currentSector = Mathf.FloorToInt(currentArea / 3f);
                loadedGameSector = currentSector;
            }
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
                gm.PlayAnimation(gm.screenMgr.ATravelMap(animDirection, currentMap, currentSector, newSector));
                currentSector = newSector;
            }
            DrawScreen();
        }

        private void DrawAreaMarkers() {
            ClearMarkers(); //Destroy all current markers.
            if(currentMap == 0) {
                int firstArea = currentSector * 3;
                for (int i = firstArea; i <= firstArea + 2; i++) {
                    if (gm.DistanceMgr.GetAreaCompleted(currentMap, i)) {
                        Vector2Int markerPos = Constants.areaPositions[currentMap][i];
                        RectangleBuilder marker = gm.BuildRectangle("Area" + i + "Marker", screenDisplay.transform, 2, 2, markerPos.x, markerPos.y);
                        markerPoints.Add(marker);
                    }
                    //If this area is the current area, use a flickering point to mark it. This point is drawn on top of the marker that indicates the area has been completed.
                    if (currentArea == i) {
                        Vector2Int markerPos = Constants.areaPositions[currentMap][i];
                        currentAreaMarker = gm.BuildRectangle("CurrentAreaMarker", screenDisplay.transform, 2, 2, markerPos.x, markerPos.y, 0.25f);
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
            hoveredMarker = gm.BuildRectangle("OptionMarker", screenDisplay.transform, 2, 2, flickPeriod: 0.25f);
            hoveredAreaName = gm.BuildTextBox("AreaName", screenDisplay.transform, "area", DFont.Small, 28, 5);

            if(currentMap == 0) {
                hoveredArea = (currentSector == loadedGameSector) ? currentArea : currentSector * 3;
                hoveredMarker.SetPosition(Constants.areaPositions[currentMap][hoveredArea]);

                if (currentSector == 0 || currentSector == 3) {
                    hoveredAreaName.SetPosition(2, 1);
                }
                else {
                    hoveredAreaName.SetPosition(2, 26);
                }

                hoveredAreaName.Text = string.Format("area{0:00}", hoveredArea + 1);
            }
        }
        private void NavigateAreaSelection(Direction dir) {
            if(currentMap == 0) {
                if(dir == Direction.Left) {
                    hoveredArea = hoveredArea.CircularAdd(-1, (currentSector * 3) + 2, currentSector * 3);
                }
                else {
                    hoveredArea = hoveredArea.CircularAdd(1, (currentSector * 3) + 2, currentSector * 3);
                }
                hoveredMarker.SetPosition(Constants.areaPositions[currentMap][hoveredArea]);
                hoveredAreaName.Text = string.Format("area{0:00}", hoveredArea + 1);
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

            distanceScreen = gm.BuildSprite("DistanceScreen", screenDisplay.transform, sprite: gm.spriteDB.map_distanceScreen);
            gm.BuildTextBox("Distance", distanceScreen.transform, areaDist.ToString(), DFont.Regular, 25, 5, 6, 25, TextAnchor.UpperRight);
        }
        private void CloseViewDistance() {
            currentScreen = ScreenMap.ChoosingArea;
            distanceScreen.Dispose();
        }

        private void ChooseArea() {
            if (hoveredArea != currentArea) gm.DistanceMgr.MoveToArea(currentMap, hoveredArea);
            CloseApp(Screen.Character);
        }
    }
}