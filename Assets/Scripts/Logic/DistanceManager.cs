using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    /// <summary>
    /// A class that manages the area and distance parameters of the game.
    /// Any interaction with those parameters should be done through this class, rather than directly altering the SavedGame class.
    /// </summary>
    public class DistanceManager {
        public int[][] Distances { get; private set; }
        private GameManager gm;
        private SavedGame loadedGame;

        public DistanceManager(GameManager gm, SavedGame loadedGame) {
            string distancesJson = ((TextAsset)Resources.Load("distances")).text;
            Distances = JsonConvert.DeserializeObject<int[][]>(distancesJson);
            this.gm = gm;
            this.loadedGame = loadedGame;
        }

        public int GetNumberOfAreasInMap(int map) {
            switch(map) {
                case 0: return 12;
                case 1: return 1;
                case 2: return 12;
                case 3: return 1;
                case 4: return 10;
                case 5: return 4;
                case 6: return 1;
                case 7: return 8;
                case 8: return 1;
                case 9: return 1;
                default: return -1;
            }
        }

        /// <summary>
        /// Returns the current distance of the area the player is in.
        /// </summary>
        public int CurrentDistance {
            get => loadedGame.CurrentDistance;
            set => loadedGame.CurrentDistance = value;
        }
        /// <summary>
        /// Returns the current map the player is in.
        /// </summary>
        public int CurrentMap {
            get => loadedGame.CurrentMap;
            set => loadedGame.CurrentMap = value;
        }
        /// <summary>
        /// Returns the current area the player is in.
        /// </summary>
        public int CurrentArea {
            get => loadedGame.CurrentArea;
            set => loadedGame.CurrentArea = value;
        }
        /// <summary>
        /// Returns the total amount of steps taken by the player.
        /// </summary>
        public int TotalSteps => loadedGame.Steps;
        /// <summary>
        /// Returns true if the area is already completed.
        /// </summary>
        public bool GetAreaCompleted(int map, int area) => loadedGame.GetAreaCompleted(map, area);
        /// <summary>
        /// Sets whether the area is completed or not.
        /// </summary>
        public void SetAreaCompleted(int map, int area, bool completed) => loadedGame.SetAreaCompleted(map, area, completed);
        /// <summary>
        /// Returns a list of all the areas in a map that haven't been completed yet.
        /// </summary>
        public List<int> GetUncompletedAreas(int map) {
            List<int> uncompletedAreas = new List<int>();
            for(int i = 0; i < gm.DatabaseMgr.AreasPerMap[map]; i++) {
                if (!GetAreaCompleted(map, i)) uncompletedAreas.Add(i);
            }
            return uncompletedAreas;
        }
        /// <summary>
        /// Moves the player to a new map and area, and sets the current distance to the default distance for that map and area.
        /// </summary>
        public void MoveToArea(int map, int area) {
            MoveToArea(map, area, Distances[map][area]);
        }
        /// <summary>
        /// Moves the player to a new map and area, and sets the current distance for that new area.
        /// </summary>
        public void MoveToArea(int map, int area, int distance) {
            loadedGame.CurrentMap = map;
            loadedGame.CurrentArea = area;
            loadedGame.CurrentDistance = distance;
        }
        /// <summary>
        /// Takes a number of steps. This does not actually reduce distance. Returns true if the player takes enough steps to trigger an event, 
        /// and, in that case, resets the next event counter.
        /// </summary>
        /// <param name="steps"></param>
        public void TakeSteps(int steps = 1) {
            loadedGame.StepsToNextEvent -= steps;
            loadedGame.Steps += steps;

            if(loadedGame.StepsToNextEvent <= 0) {
                loadedGame.StepsToNextEvent = Random.Range(3, 6) * 100;
                loadedGame.SavedEvent = 1;
            }
        }

        //TODO: Test if semibosses work properly.
        /// <summary>
        /// Tries to reduce the distance by an amount, and outputs the actual distance reduced.
        /// Returns the actual distance reduced.
        /// </summary>
        /// <returns></returns>
        public int ReduceDistance(int distance) {
            int nextStop = 1; //Indicates the distance at which an event will trigger (and no extra distance will be removed).
            //Check semibosses for maps 4 and 8:
            if(loadedGame.CurrentMap == 4) {
                int firstDeva = (int)((Distances[4][loadedGame.CurrentArea] / 4f) * 3);
                int secondDeva = (int)((Distances[4][loadedGame.CurrentArea] / 4f) * 2);
                int thirdDeva = (int)(Distances[4][loadedGame.CurrentArea] / 4f);
                if (loadedGame.CurrentDistance > firstDeva) nextStop = firstDeva + 1;
                else if (loadedGame.CurrentDistance > secondDeva) nextStop = secondDeva + 1;
                else if (loadedGame.CurrentDistance > thirdDeva) nextStop = thirdDeva + 1;
            }
            else if(loadedGame.CurrentMap == 8) {
                int murmukusmon = (int)(Distances[8][loadedGame.CurrentArea] / 2f);
                if (loadedGame.CurrentDistance > murmukusmon) nextStop = murmukusmon + 1;
            }

            if (loadedGame.CurrentDistance - distance <= nextStop) {
                loadedGame.CurrentDistance = nextStop;
                loadedGame.SavedEvent = 2;
                return loadedGame.CurrentDistance - 1 - nextStop;
            }
            else {
                loadedGame.CurrentDistance -= distance;
                return distance;
            }
        }

        /// <summary>
        /// Forcibly reduces distance by an amount. This method will bypass boss encounters.
        /// </summary>
        /// <param name="distance"></param>
        public void ForceReduceDistance(int distance) => loadedGame.CurrentDistance -= distance;
        /// <summary>
        /// Increases the distance by an amount.
        /// </summary>
        /// <param name="distance"></param>
        public void IncreaseDistance(int distance) => loadedGame.CurrentDistance += distance;
    }
}