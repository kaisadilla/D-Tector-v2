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
            s.AssignRandomBosses();
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
        private void AssignRandomBosses() {
            List<string> humanBosses = new List<string>(12) {
            "Agunimon",
            "Lobomon",
            "Beetlemon",
            "Kazemon",
            "Kumamon",
            "Loweemon",
            "Grumblemon",
            "Arbormon",
            "Mercurymon",
            "Lanamon",
            "Dragomon",
            "Orochimon"
        };
            List<string> humanSemiBosses = new List<string>(12) {
            "Candlemon",
            "ToyAgumon",
            "Tapirmon",
            "ShellNumemon",
            "Snimon",
            "Woodmon",
            "Raremon",
            "Devimon",
            "Cerberumon",
            "Phantomon",
            "Karatenmon",
            "IceDevimon"
        };
            List<string> animalBosses = new List<string>(12) {
            "BurningGreymon",
            "KendoGarurumon",
            "MetalKabuterimon",
            "Zephyrmon",
            "Korikakumon",
            "KaiserLeomon",
            "Gigasmon",
            "Petaldramon",
            "Sephirothmon",
            "Calmaramon",
            "SkullSatamon",
            "Apocalymon"
        };
            List<string> animalSemiBosses = new List<string>(12) {
            "Mihiramon",
            "Antylamon (Deva)",
            "Majiramon",
            "Sandiramon",
            "Indramon",
            "Pajiramon",
            "Makuramon",
            "Sinduramon",
            "Caturamon",
            "Vikaralamon",
            "Kumbhiramon",
            "Vajramon"
        };
            List<string> sacredBosses = new List<string>(4) {
            "Azulongmon",
            "Baihumon",
            "Zhuqiaomon",
            "Ebonwumon"
        };
            List<string> royalKnights = new List<string>(8) {
            "Omnimon",
            "Gallantmon (Crimson)",
            "Dynasmon",
            "LordKnightmon",
            "Kentaurosmon",
            "Alphamon",
            "Craniamon",
            "ShadowSeraphimon"
        };

            humanBosses.Shuffle();
            animalBosses.Shuffle();
            animalSemiBosses.Shuffle();
            sacredBosses.Shuffle();
            royalKnights.Shuffle();

            for (int i = 0; i < 12; i++) {
                //store human bossnames in hnbe(slot)sK(#area)j35 (i.e. hnbe1sK10j35
                EncryptedPlayerPrefs.SetString("s" + slot + "_area" + (i + 1) + "_human_bossname", humanBosses[i]);
            }

            for (int i = 1; i <= 12; i++) {
                EncryptedPlayerPrefs.SetString("s" + slot + "_human_semiboss_number_" + i, humanSemiBosses[i - 1]);
            }

            for (int i = 0; i < 12; i++) {
                EncryptedPlayerPrefs.SetString("s" + slot + "_area" + (i + 1) + "_animal_bossname", animalBosses[i]);
            }

            for (int i = 0; i < 12; i++) {
                EncryptedPlayerPrefs.SetString("s" + slot + "_area" + (i + 1) + "_animal_semibossname", animalSemiBosses[i]);
            }

            for (int i = 0; i < 4; i++) {
                EncryptedPlayerPrefs.SetString("s" + slot + "_area" + (i + 14) + "_bossname", sacredBosses[i]);
            }

            for (int i = 0; i < 8; i++) {
                EncryptedPlayerPrefs.SetString("s" + slot + "_area" + (i + 19) + "_bossname", royalKnights[i]);
            }

            EncryptedPlayerPrefs.SetInt("s" + slot + "_hsbc", 0);

        }
    }
}