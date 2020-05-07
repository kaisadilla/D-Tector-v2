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

        public DistanceManager(GameManager gm) {
            string distancesJson = ((TextAsset)Resources.Load("distances")).text;
            Distances = JsonConvert.DeserializeObject<int[][]>(distancesJson);
            this.gm = gm;
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
            get => SavedGame.CurrentDistance;
            set => SavedGame.CurrentDistance = value;
        }
        /// <summary>
        /// Returns the current map the player is in.
        /// </summary>
        public int CurrentMap {
            get => SavedGame.CurrentMap;
            set => SavedGame.CurrentMap = value;
        }
        /// <summary>
        /// Returns the current area the player is in.
        /// </summary>
        public int CurrentArea {
            get => SavedGame.CurrentArea;
            set => SavedGame.CurrentArea = value;
        }
        /// <summary>
        /// Returns the total amount of steps taken by the player.
        /// </summary>
        public int TotalSteps => SavedGame.Steps;
        /// <summary>
        /// Returns true if the area is already completed.
        /// </summary>
        public bool GetAreaCompleted(int map, int area) => SavedGame.CompletedAreas[map][area];
        /// <summary>
        /// Sets whether the area is completed or not.
        /// </summary>
        public void SetAreaCompleted(int map, int area, bool completed) => SavedGame.CompletedAreas[map][area] = completed;
        /// <summary>
        /// Returns a list of all the areas in a map that haven't been completed yet.
        /// </summary>
        public List<int> GetUncompletedAreas(int map) {
            List<int> uncompletedAreas = new List<int>();
            for(int i = 0; i < Database.AreasPerMap[map]; i++) {
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
            SavedGame.CurrentMap = map;
            SavedGame.CurrentArea = area;
            SavedGame.CurrentDistance = distance;
        }
        /// <summary>
        /// Takes a number of steps. This does not actually reduce distance. Returns true if the player takes enough steps to trigger an event, 
        /// and, in that case, resets the next event counter.
        /// </summary>
        /// <param name="steps"></param>
        public void TakeSteps(int steps = 1) {
            SavedGame.StepsToNextEvent -= steps;
            SavedGame.Steps += steps;

            if(SavedGame.StepsToNextEvent <= 0) {
                SavedGame.StepsToNextEvent = Random.Range(3, 6) * 100;
                SavedGame.SavedEvent = 1;
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
            if(SavedGame.CurrentMap == 4) {
                int firstDeva = (int)((Distances[4][SavedGame.CurrentArea] / 4f) * 3);
                int secondDeva = (int)((Distances[4][SavedGame.CurrentArea] / 4f) * 2);
                int thirdDeva = (int)(Distances[4][SavedGame.CurrentArea] / 4f);
                if (SavedGame.CurrentDistance > firstDeva) nextStop = firstDeva + 1;
                else if (SavedGame.CurrentDistance > secondDeva) nextStop = secondDeva + 1;
                else if (SavedGame.CurrentDistance > thirdDeva) nextStop = thirdDeva + 1;
            }
            else if(SavedGame.CurrentMap == 8) {
                int murmukusmon = (int)(Distances[8][SavedGame.CurrentArea] / 2f);
                if (SavedGame.CurrentDistance > murmukusmon) nextStop = murmukusmon + 1;
            }

            if (SavedGame.CurrentDistance - distance <= nextStop) {
                SavedGame.CurrentDistance = nextStop;
                //loadedGame.SavedEvent = 2; Do not trigger boss events unless the player shakes the device or presses B.
                return SavedGame.CurrentDistance - 1 - nextStop;
            }
            else {
                SavedGame.CurrentDistance -= distance;
                return distance;
            }
        }

        /// <summary>
        /// Forcibly reduces distance by an amount. This method will bypass boss encounters.
        /// </summary>
        /// <param name="distance"></param>
        public void ForceReduceDistance(int distance) => SavedGame.CurrentDistance -= distance;
        /// <summary>
        /// Increases the distance by an amount.
        /// </summary>
        /// <param name="distance"></param>
        public void IncreaseDistance(int distance) => SavedGame.CurrentDistance += distance;
    }
}