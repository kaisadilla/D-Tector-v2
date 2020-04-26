using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kaisa.Digivice.Extensions;

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

        //Indirect variables: these are variables that are calculated from other variables stored.
        /// <summary>
        /// Returns the level of a player based on its experience.
        /// </summary>
        public int PlayerLevel {
            get {
                int playerXP = PlayerExperience;
                if (playerXP == 0) return 1;

                float level = Mathf.Pow(playerXP, 1f / 3f);
                return Mathf.FloorToInt(level);
            }
        }
        /// <summary>
        /// Returns the amount of experience the player earns from winning a game.
        /// </summary>
        /// <param name="enemyLevel">The level of the enemy Digimon.</param>
        /// <returns></returns>
        public int ExperienceGained(int enemyLevel) {
            int playerLevel = PlayerLevel;

            //Original formula: ((((Mathf.Pow(enemyLevel, 2)) / 5f) * ((Mathf.Pow((2f * enemyLevel) + 10f, 2.5f) / Mathf.Pow((enemyLevel + playerLevel + 10), 2.5f)))) + 1) * 0.75f;
            float a = Mathf.Pow(enemyLevel, 2) / 5f; // 1/5th of the square power of the enemy level.
            float b = Mathf.Pow((2f * enemyLevel) + 10f, 2.5f); // 2x the enemy level + 10, raised to the power of 2.5.
            float c = Mathf.Pow(enemyLevel + playerLevel + 10, 2.5f); // Enemy level + player level + 10, raised to the power of 2.5.
            float expGained = ((a * (b / c)) + 1) * 0.75f;

            return Mathf.CeilToInt(expGained);
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
        /*public int PlayerLevel {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_playerLevel");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_playerLevel", value);
        }*/
        public int PlayerExperience {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_playerExperience");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_playerExperience", value);
        }
        /// <summary>
        /// The current amount of Spirit Power the player has.
        /// The method AddSpiritPower() is provided to modify this value.
        /// </summary>
        public int SpiritPower {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_spiritPower");
            set {
                if (value > 99) value = 99;
                if (value < 0) value = 0;
                EncryptedPlayerPrefs.SetInt("s" + slot + "_spiritPower", value);
            }
        }

        public void AddSpiritPower(int val) => SpiritPower += val;
        public int TotalBattles {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_battleCount");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_battleCount", value);
        }
        public int TotalWins {
            get => EncryptedPlayerPrefs.GetInt("s" + slot + "_winCount");
            set => EncryptedPlayerPrefs.SetInt("s" + slot + "_winCount", value);
        }
        public float WinPercentage {
            get => TotalWins / (float)TotalBattles;
        }
        public string GetDockDigimon(int dock) {
            return EncryptedPlayerPrefs.GetString("s" + slot + "_dock" + dock);
        }
        public void SetDockDigimon(int dock, string digimon) {
            EncryptedPlayerPrefs.SetString("s" + slot + "_dock" + dock, digimon);
        }
        public bool IsDigimonUnlocked(string digimon) {
            return !(GetDigimonLevel(digimon) == 0);
            /*if(PlayerPrefs.GetInt("s" + slot + "_digimon_" + digimon + "_unlocked") == 1) {
                return true;
            }
            return false;*/
        }
        /// <summary>
        /// Level 0 means locked, level 1 means unlocked, levels beyond 1 add levels to that digimon (i.e. level 3 means the digimon has 2 extra levels)
        /// </summary>
        public void SetDigimonLevel(string digimon, int level) {
            EncryptedPlayerPrefs.SetInt("s" + slot + "_digimon_" + digimon + "_level", level);
        }
        /// <summary>
        /// Level 0 means locked, level 1 means unlocked, levels beyond 1 add levels to that digimon (i.e. level 3 means the digimon has 2 extra levels)
        /// </summary>
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