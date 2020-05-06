using Kaisa.Digivice.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public class SavedGame {
        private int slot = 0;

        private SavedGame(int slot) {
            this.slot = slot;
        }

        public static SavedGame CreateNewSavedGame() {
            int candidateSlot = 0;
            while (IsSlotUsed(candidateSlot)) candidateSlot++;

            SavedGame s = new SavedGame(candidateSlot);
            return s;
        }

        public static SavedGame LoadSavedGame(int sgSlot) {
            return new SavedGame(sgSlot);
        }

        public static bool IsSlotUsed(int candidateSlot) {
            return (EncryptedPlayerPrefs.GetInt("s" + candidateSlot) == 1);
        }

        //Direct variables: these are variables that are directly stored in the PlayerPrefs.
        public bool SlotExists {
            get => (EncryptedPlayerPrefs.GetInt("s" + slot) == 1);
            set => EncryptedPlayerPrefs.SetInt("s" + slot, (value == true) ? 1 : 0);
        }
        public int Slot {
            get => slot;
        }
        public string Name {
            get => EncryptedPlayerPrefs.GetString("s" + slot + "_name");
            set => EncryptedPlayerPrefs.SetString("s" + slot + "_name", value);
        }
        public bool CheatsUsed {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_cheatsUsed") != 5;
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_cheatsUsed", value ? 12 : 5);
        }
        public GameChar PlayerChar {
            get => (GameChar)EncryptedPlayerPrefs.GetInt("s" + slot + "_character");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_character", (int)value);
        }
        public int CurrentArea {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_currentArea");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_currentArea", value);
        }
        public int CurrentDistance {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_currentDistance");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_currentDistance", value);
        }
        public int Steps {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_steps");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_steps", value);
        }
        public int StepsToNextEvent {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_nextEncounter");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_nextEncounter", value);
        }
        public int PlayerExperience {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_playerExperience");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_playerExperience", value);
        }
        public bool IsPlayerInsured {
            get => EncryptedPlayerPrefs.GetInt($"s{slot}_playerInsurance") == 1;
            set => EncryptedPlayerPrefs.SetInt($"s{slot}_playerInsurance", value ? 1 : 0);
        }
        public int SpiritPower {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_spiritPower");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_spiritPower", value);
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
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_battleCount");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_battleCount", value);
        }
        public int TotalWins {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_winCount");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_winCount", value);
        }
        // Note: DDocks are stored on values 0 to 3, even if they are called 1 to 4 in-game.
        public string GetDDockDigimon(int dock) {
            return EncryptedPlayerPrefs.GetString("s" + slot + "_dock" + dock);
        }
        public void SetDDockDigimon(int dock, string digimon) {
            EncryptedPlayerPrefs.SetString("s" + slot + "_dock" + dock, digimon);
        }
        // Value 0 means locked, value 1 means unlocked, value 2 represents 1 extra level, value 3 represents 2 extra levels, and so on.
        // GetDigimonLevel() in the Logic Manager should be used to access the actual level of a Digimon, and will always return the exact level.
        // so no calculations are needed for the value that method offers.
        public void SetDigimonLevel(string digimon, int level) {
            EncryptedPlayerPrefs.SetInt("s" + slot + "_digimon_" + digimon + "_level", level);
        }
        public int GetDigimonLevel(string digimon) {
            return EncryptedPlayerPrefs.GetInt("s" + slot + "_digimon_" + digimon + "_level");
        }
        /// <summary>
        /// Sets whether the player has unlocked the code for a digimon.
        /// </summary>
        public void SetDigimonCodeUnlocked(string digimon, bool value) {
            EncryptedPlayerPrefs.SetInt("s" + slot + "_digimon_" + digimon + "_code", value ? 1 : 0);
        }
        /// <summary>
        /// Returns true if the player has unlocked the code for a digimon.
        /// </summary>
        public bool GetDigimonCodeUnlocked(string digimon) {
            return EncryptedPlayerPrefs.GetInt("s" + slot + "_digimon_" + digimon + "_code") == 1;
        }
        //Adventure: the player travels through different maps. Each map has a number of areas distributed between one or more sectors.
        /// <summary>
        /// Represents the current map the player is traveling. The initial map is map 0, and the animal version of that map is map 1, and so on.
        /// </summary>
        public int CurrentMap {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_currentMap");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_currentMap", value);
        }
        //Each map has a number of areas. For example, map 0 and 1 have 12 areas (index 0 to 11).
        public bool GetAreaCompleted(int map, int area) {
            string key = "s" + slot + "_map" + map + "_area" + area;
            return EncryptedPlayerPrefs.GetInt(key) == 1;
        }
        public void SetAreaCompleted(int map, int area, bool completed) {
            string key = "s" + slot + "_map" + map + "_area" + area;
            EncryptedPlayerPrefs.SetInt(key, (completed == true) ? 1 : 0);
        }
        /// <summary>
        /// Returns an array of all the bosses assigned with that map, in the order they were assigned..
        /// </summary>
        /// <param name="map">The map to obtain.</param>
        /// <param name="totalAreas">The total amount of areas in this map.</param>
        /// <returns></returns>
        public string[] GetBossesForMap(int map, int totalAreas) {
            string[] order = new string[totalAreas];
            for(int i = 0; i < order.Length; i++) {
                order[i] = EncryptedPlayerPrefs.GetString($"s{slot}_map{map}_boss{i}");
            }
            return order;
        }
        public void SetBossesForMap(int map, string[] value) {
            for(int i = 0; i < value.Length; i++) {
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
    }
}