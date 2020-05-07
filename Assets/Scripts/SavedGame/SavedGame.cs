using Kaisa.Digivice.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Kaisa.Digivice {
    public class SavedGame {
        public static int CurrentlyLoadedSlot {
            get => EncryptedPlayerPrefs.GetInt("CurrentSlot");
            set => EncryptedPlayerPrefs.SetInt("CurrentSlot", value);
        }
        private readonly int slot;

        private SavedGame(int slot) {
            this.slot = slot;
        }

        /// <summary>
        /// Return a list of all the saved games in their brief mode.
        /// </summary>
        /// <returns></returns>
        public static List<BriefSavedGame> GetAllSavedGames() {
            List<BriefSavedGame> slots = new List<BriefSavedGame>();
            bool foundSlot = true;
            int currentSlot = 0;
            while (foundSlot) {
                //If the slot exists, continue.
                if (DoesSlotExist(currentSlot)) {
                    if (IsSlotUsed(currentSlot)) {
                        slots.Add(GetBriefSavedGame(currentSlot));
                    }
                    currentSlot++;
                }
                //Else, no more slots will exist so we end the loop.
                else {
                    foundSlot = false;
                    break;
                }
            }
            return slots;
        }

        public static int GetFirstEmptySlot() {
            int freeSlot = -1;
            int candidateSlot = 0;
            while (freeSlot == -1) {
                if (IsSlotUsed(candidateSlot)) {
                    candidateSlot++;
                }
                else {
                    freeSlot = candidateSlot;
                }
            }
            return freeSlot;
        }
        public static SavedGame CreateSavedGame(int slot) {
            SavedGame newGame = new SavedGame(slot);
            newGame.SlotExists = true;
            newGame.Overwrittable = false;
            newGame.CheatsUsed = false;
            newGame.PlayerChar = GameChar.none;
            return newGame;
        }
        public static SavedGame LoadSavedGame(int slot) {
            return new SavedGame(slot);
        }
        public static BriefSavedGame GetBriefSavedGame(int slot) {
            return new BriefSavedGame(
                slot,
                EncryptedPlayerPrefs.GetString($"s{slot}_name"),
                EncryptedPlayerPrefs.GetString($"s{slot}_character"),
                EncryptedPlayerPrefs.GetInt($"s{slot}_playerExperience")
                );
        }

        /// <summary>
        /// Returns true if a key for a slot is found, whether or not that slot is available.
        /// </summary>
        public static bool DoesSlotExist(int slot) => EncryptedPlayerPrefs.GetInt($"s{slot}") == 1;
        /// <summary>
        /// Returns true if a key for a slot is found and that key is not marked as overwrittable.
        /// </summary>
        public static bool IsSlotUsed(int slot) {
            if (EncryptedPlayerPrefs.GetInt($"s{slot}") == 1) {
                if (IsSlotOverwrittable(slot)) {
                    return false;
                }
                else {
                    return true;
                }
            }
            else {
                return false;
            }
        }
        public static bool IsSlotOverwrittable(int slot) => EncryptedPlayerPrefs.GetInt($"s{slot}_overwrittable") == 1;

        /// <summary>
        /// If true, sets the slot as empty, meaning that it can be overwritten.
        /// </summary>
        public bool Overwrittable {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_overwrittable") == 1;
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_overwrittable", value ? 1 : 0);
        }

        //Direct variables: these are variables that are directly stored in the PlayerPrefs.
        public bool SlotExists {
            get => (EncryptedPlayerPrefs.GetInt($"s{slot}") == 1);
            set => EncryptedPlayerPrefs.SetInt($"s{slot}", (value == true) ? 1 : 0);
        }
        public int Slot {
            get => slot;
        }
        public string Name {
            get => EncryptedPlayerPrefs.GetString($"s{slot}_name");
            set => EncryptedPlayerPrefs.SetString($"s{slot}_name", value);
        }
        public bool CheatsUsed {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_cheatsUsed") != 5;
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_cheatsUsed", value ? 12 : 5);
        }
        public GameChar PlayerChar {
            get => (GameChar)EncryptedPlayerPrefs.GetInt($"s{slot}_character");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_character", (int)value);
        }
        public int CurrentArea {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_currentArea");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_currentArea", value);
        }
        public int CurrentDistance {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_currentDistance");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_currentDistance", value);
        }
        public int Steps {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_steps");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_steps", value);
        }
        public int StepsToNextEvent {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_nextEncounter");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_nextEncounter", value);
        }
        public int PlayerExperience {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_playerExperience");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_playerExperience", value);
        }
        public bool IsPlayerInsured {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_playerInsurance") == 1;
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_playerInsurance", value ? 1 : 0);
        }
        public int SpiritPower {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_spiritPower");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_spiritPower", value);
        }

        public void SetRandomSeed(int index, int value) {
            EncryptedPlayerPrefs.SetInt($"s{slot}_seed{index}", value);
        }
        public int GetRandomSeed(int index) {
            return EncryptedPlayerPrefs.GetInt($"s{slot}_seed{index}");
        }

        public bool IsLeaverBusterActive {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_leaverBuster") == 1;
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_leaverBuster", value ? 1 : 0);
        }
        public int LeaverBusterExpLoss {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_leaverBusterExpLoss");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_leaverBusterExpLoss", value);
        }
        public string LeaverBusterDigimonLoss {
            get => EncryptedPlayerPrefs.GetString($"s{slot}_leaverBusterDigimonLoss");
            set => EncryptedPlayerPrefs.SetString($"s{slot}_leaverBusterDigimonLoss", value);
        }
        public int TotalBattles {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_battleCount");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_battleCount", value);
        }
        public int TotalWins {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_winCount");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_winCount", value);
        }
        // Note: DDocks are stored on values 0 to 3, even if they are called 1 to 4 in-game.
        public string GetDDockDigimon(int dock) {
            return EncryptedPlayerPrefs.GetString($"s{slot}_dock{dock}");
        }
        public void SetDDockDigimon(int dock, string digimon) {
            EncryptedPlayerPrefs.SetString($"s{slot}_dock{dock}", digimon);
        }
        // Value 0 means locked, value 1 means unlocked, value 2 represents 1 extra level, value 3 represents 2 extra levels, and so on.
        // GetDigimonLevel() in the Logic Manager should be used to access the actual level of a Digimon, and will always return the exact level.
        // so no calculations are needed for the value that method offers.
        public void SetDigimonLevel(string digimon, int level) {
            EncryptedPlayerPrefs.SetInt($"s{slot}_digimon_{digimon}_level", level);
        }
        public int GetDigimonLevel(string digimon) {
            return EncryptedPlayerPrefs.GetInt($"s{slot}_digimon_{digimon}_level");
        }
        /// <summary>
        /// Sets whether the player has unlocked the code for a digimon.
        /// </summary>
        public void SetDigimonCodeUnlocked(string digimon, bool value) {
            EncryptedPlayerPrefs.SetInt($"s{slot}_digimon_{digimon}_code", value ? 1 : 0);
        }
        /// <summary>
        /// Returns true if the player has unlocked the code for a digimon.
        /// </summary>
        public bool GetDigimonCodeUnlocked(string digimon) {
            return EncryptedPlayerPrefs.GetInt($"s{slot}_digimon_{digimon}_code") == 1;
        }
        //Adventure: the player travels through different maps. Each map has a number of areas distributed between one or more sectors.
        /// <summary>
        /// Represents the current map the player is traveling. The initial map is map 0, and the animal version of that map is map 1, and so on.
        /// </summary>
        public int CurrentMap {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_currentMap");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_currentMap", value);
        }
        //Each map has a number of areas. For example, map 0 and 1 have 12 areas (index 0 to 11).
        public bool GetAreaCompleted(int map, int area) {
            return EncryptedPlayerPrefs.GetInt($"s{slot}_map{map}_area{area}") == 1;
        }
        public void SetAreaCompleted(int map, int area, bool completed) {
            EncryptedPlayerPrefs.SetInt($"s{slot}_map{map}_area{area}", (completed == true) ? 1 : 0);
        }
        /// <summary>
        /// Returns an array of all the bosses assigned with that map, in the order they were assigned..
        /// </summary>
        /// <param name="map">The map to obtain.</param>
        /// <param name="totalAreas">The total amount of areas in this map.</param>
        /// <returns></returns>
        public string[] GetBossesForMap(int map, int totalAreas) {
            string[] order = new string[totalAreas];
            for (int i = 0; i < order.Length; i++) {
                order[i] = EncryptedPlayerPrefs.GetString($"s{slot}_map{map}_boss{i}");
            }
            return order;
        }
        public void SetBossesForMap(int map, string[] value) {
            for (int i = 0; i < value.Length; i++) {
                EncryptedPlayerPrefs.SetString($"s{slot}_map{map}_boss{i}", value[i]);
            }
        }
        public int GetSemibossGroupForMap(int map) {
            return EncryptedPlayerPrefs.GetInt($"s{slot}_map{map}_semibossGroup");
        }

        public void SetSemibossGroupForMap(int map, int value) {
            EncryptedPlayerPrefs.SetInt($"s{slot}_map{map}_semibossGroup", value);
        }

        /// <summary>
        /// Equals 0 if no event is currently active, 1 if a regular event is active and 2 if a boss event is active.
        /// </summary>
        public int SavedEvent {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_pendingEvent");
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_pendingEvent", value);
        }

        /// <summary>
        /// Completely deletes all the keys for one slot, except for s{slot} and s{slot}_overwrittable, which is turned to true.
        /// (needed to keep track of all the slots used when creating the savedgame list).
        /// </summary>
        public static void DeleteSlot(int slot) {
            DatabaseManager DatabaseMgr = new DatabaseManager();

            EncryptedPlayerPrefs.SetInt($"s{slot}_overwrittable", 1);
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_name");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_cheatsUsed");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_character");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_currentArea");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_currentDistance");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_steps");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_nextEncounter");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_playerExperience");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_spiritPower");
            for (int i = 0; i < 3; i++) {
                EncryptedPlayerPrefs.DeleteKey($"s{slot}_seed{i}");
            }
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_leaverBuster");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_leaverBusterExpLoss");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_leaverBusterDigimonLoss");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_battleCount");
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_winCount");
            for (int i = 0; i < 3; i++) {
                EncryptedPlayerPrefs.DeleteKey($"s{slot}_dock{i}");
            }
            foreach (Digimon d in DatabaseMgr.Digimons) {
                EncryptedPlayerPrefs.DeleteKey($"s{slot}_digimon_{d}_level");
                EncryptedPlayerPrefs.DeleteKey($"s{slot}_digimon_{d}_code");
            }
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_currentMap");
            for (int map = 0; map < DatabaseMgr.Bosses.Length; map++) {
                for (int area = 0; area < DatabaseMgr.AreasPerMap[map]; area++) {
                    EncryptedPlayerPrefs.DeleteKey($"s{slot}_map{map}_area{area}");
                    EncryptedPlayerPrefs.DeleteKey($"s{slot}_map{map}_boss{area}");
                }
                EncryptedPlayerPrefs.DeleteKey($"s{slot}_map{map}_semibossGroup");
            }
            EncryptedPlayerPrefs.DeleteKey($"s{slot}_pendingEvent");
            PlayerPrefs.Save();
        }
    }

    public struct BriefSavedGame {
        public readonly int slot;
        public readonly string name;
        public readonly int level;
        public readonly string character;
        public BriefSavedGame(int slot, string name, string character, int experience) {
            this.slot = slot;
            this.name = name;
            this.character = character;
            if (experience == 0) level = 1;
            else level = Mathf.FloorToInt(Mathf.Pow(experience, 1f / 3f));
        }
    }

    [System.Serializable]
    public class SavedGame2 {
        public string filePath;
        public int index;

        //Technical data:
        public string name;
        public int gameChar;
        public bool cheatsUsed;

        //Volatile data:
        public int pendingEvent;
        public int isPlayerInsured;
        public bool isLeaverBusterActive;
        public int leaverBusterExpLoss;
        public string leaverBusterDigimonLoss;

        //Current situation data:
        public int currentMap;
        public int currentArea;
        public int currentDistance;
        public int steps;
        public int stepsToNextEvent;
        public int playerExperience;
        public int spiritPower;
        public int[] battleSeed = new int[3];
        public int totalBattles;
        public int totalWins;
        public string[] ddockDigimon = new string[4];

        //Progress data:
        public Dictionary<string, int> digimonLevel = new Dictionary<string, int>();
        public Dictionary<string, int> digicodeUnlocked = new Dictionary<string, int>();
        //Maybe use the same logic than original menu here: areaCompleted[0][3] generates 0 and 3 automatically.
        public List<List<int>> areaCompleted; //areaCompleted[mapIndex][areaIndex]
        public List<List<string>> bosses; //bosses[mapIndex][areaIndex]
        public List<int> semibossGroup; //semibossGroup[maxIndex]

        private SavedGame2(string filePath, int index, string name) {
            this.filePath = filePath;
            this.index = index;
            this.name = name;
        }

        public static SavedGame2 LoadSavedGame(string filePath) {
            SavedGame2 savedGame = null;
            if (File.Exists(filePath)) {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(filePath, FileMode.Open)) {
                    savedGame = (SavedGame2)formatter.Deserialize(stream);
                }
                savedGame.filePath = filePath;
            }
            return savedGame;
        }
        public static SavedGame2 CreateSavedGame(string name, int index) {
            int savedGameNumber = 0;
            while (File.Exists($"{Application.persistentDataPath}/savedGames/game{savedGameNumber}.digivice")) {
                savedGameNumber++;
            }
            string filePath = $"{Application.persistentDataPath}/savedGames/game{savedGameNumber}.digivice";
            SavedGame2 savedGame = new SavedGame2(filePath, index, name);
            savedGame.SaveAllDataToFile();
            return savedGame;
        }

        public void SaveAllDataToFile() {
            BinaryFormatter formatter = new BinaryFormatter();
            using(FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate)) {
                formatter.Serialize(stream, this);
            }
        }
    }
}