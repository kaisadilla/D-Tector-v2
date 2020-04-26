using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Kaisa.Digivice;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace Kaisa.Digivice {
    public class Database {
        private GameManager gm;
        public Digimon[] DigimonDB { get; private set; }

        public Database(GameManager gm) {
            this.gm = gm;
            LoadDatabase();
        }

        /// <summary>
        /// Loads all the Digimon that are not marked as "disabled" from the json database.
        /// </summary>
        public void LoadDatabase() {
            string digimonDBJson = ((TextAsset)Resources.Load("digimonDB")).text;
            JArray dbArray = JArray.Parse(digimonDBJson);

            List<Digimon> tempList = new List<Digimon>();

            for(int i = 0; i < dbArray.Count; i++) {
                Digimon d = dbArray[i].ToObject<Digimon>();

                if (!d.disabled) {
                    tempList.Add(d);
                }
            }

            DigimonDB = tempList.ToArray();
        }

        public Digimon GetDigimon(string name) {
            foreach (Digimon d in DigimonDB) {
                if (d.name == name) {
                    return d;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets a digimon at random from the database. The chances of a given digimon being selected depends on its stats and the stats of the player.
        /// </summary>
        /// <param name="playerLevel">The level of the player.</param>
        /// <param name="threshold">The maximum difference between the level of the player and the level of the digimon chosen.</param>
        public Digimon GetWeightedDigimon(int playerLevel, int threshold = 10) {
            List<Digimon> candidates = new List<Digimon>();
            List<float> weightList = new List<float>();
            float totalWeight = 0f;
            //Populate the 'candidates' list with all eligible digimon, and store their individual weights and the total weight.
            foreach (Digimon d in DigimonDB) {
                if (!d.disabled && d.baseLevel >= (playerLevel - threshold) && d.baseLevel <= (playerLevel + threshold)) {
                    candidates.Add(d);
                    float thisWeight = (1.1f - (Mathf.Abs(playerLevel - d.baseLevel) / (float)threshold)) * d.weight;
                    weightList.Add(thisWeight);
                    totalWeight += thisWeight;
                }
            }

            float fChosen = Random.Range(0, totalWeight);
            float weightSum = 0f;
            for (int i = 0; i < weightList.Count; i++) {
                weightSum += weightList[i];
                if (weightSum > fChosen) {
                    return candidates[i];
                }
            }

            return null;
        }

        public void SetDigimonLevel(string name, int level) => SetDigimonLevel(GetDigimon(name), level);
        public void SetDigimonLevel(Digimon digimon, int level) {
            if (digimon == null) return;
            //Check if the level being stored is higher than the maximum.
            int maxExtraLevel = digimon.MaxExtraLevel + 1;
            if (level > maxExtraLevel) level = maxExtraLevel;
            if (level < 0) level = 0;

            gm.LoadedGame.SetDigimonLevel(digimon.name, level);
        }

        public int GetDigimonLevel(string name) => gm.LoadedGame.GetDigimonLevel(name);
        public int GetDigimonLevel(Digimon digimon) => GetDigimonLevel(digimon.name);

        public bool IsDigimonUnlocked(string name) => gm.LoadedGame.IsDigimonUnlocked(name);
        public bool IsDigimonUnlocked(Digimon digimon) => gm.LoadedGame.IsDigimonUnlocked(digimon.name);

        public void UnlockDigimon(string name) {
            if (!IsDigimonUnlocked(name)) gm.LoadedGame.SetDigimonLevel(name, 1);
        }
        public void UnlockDigimon(Digimon digimon) => UnlockDigimon(digimon.name);
        public void UnlockDigimonCode(string name) => gm.LoadedGame.SetDigimonCodeUnlocked(name, true);
        public void UnlockDigimonCode(Digimon digimon) => UnlockDigimonCode(digimon.name);

        public void UnlockAllDigimon() {
            foreach(Digimon d in DigimonDB) {
                UnlockDigimon(d.name);
            }
        }
        public void LockAllDigimon() {
            foreach (Digimon d in DigimonDB) {
                SetDigimonLevel(d.name, 0);
            }
        }
        /// <summary>
        /// Returns true if the player has unlocked, at least, one digimon in that stage.
        /// </summary>
        public bool OwnsDigimonInStage(Stage stage) {
            foreach(Digimon d in DigimonDB) {
                if(d.stage == stage && IsDigimonUnlocked(d)) {
                    return true;
                }
            }
            return false;
        }

        public bool OwnsSpiritDigimonOfElement(Element element) {
            foreach (Digimon d in DigimonDB) {
                if (d.stage == Stage.Spirit && d.spiritType != SpiritType.Fusion && d.element == element && IsDigimonUnlocked(d)) {
                    return true;
                }
            }
            return false;
        }
        public bool OwnsFusionSpiritDigimon() {
            foreach (Digimon d in DigimonDB) {
                if (d.stage == Stage.Spirit && d.spiritType == SpiritType.Fusion && IsDigimonUnlocked(d)) {
                    return true;
                }
            }
            return false;
        }

        public bool IsInDock(string digimon) {
            for(int i = 1; i <= 4; i++) {
                if (gm.LoadedGame.GetDockDigimon(i) == digimon) {
                    return true;
                }
            }
            return false;
        }
    }
}