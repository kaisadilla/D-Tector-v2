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
        public Digimon[] Digimons { get; private set; }
        public Dictionary<string, string> DigiCodes { get; private set; }
        public Database(GameManager gm) {
            this.gm = gm;
            LoadDatabase();
        }

        public void LoadDatabase() {
            LoadDigimonDB();
            LoadDigiCodeDB();
        }

        /// <summary>
        /// Loads all the Digimon that are not marked as "disabled" from the json database.
        /// </summary>
        private void LoadDigimonDB() {
            string digimonDBJson = ((TextAsset)Resources.Load("digimonDB")).text;
            JArray dbArray = JArray.Parse(digimonDBJson);

            List<Digimon> tempList = new List<Digimon>();

            for (int i = 0; i < dbArray.Count; i++) {
                Digimon d = dbArray[i].ToObject<Digimon>();

                if (!d.disabled) {
                    tempList.Add(d);
                }
            }

            Digimons = tempList.ToArray();
        }

        private void LoadDigiCodeDB() {
            string digiCode = ((TextAsset)Resources.Load("codeDB")).text;
            DigiCodes = JsonConvert.DeserializeObject<Dictionary<string, string>>(digiCode);
        }

        public Digimon GetDigimon(string name) {
            foreach (Digimon d in Digimons) {
                if (d.name.ToLower() == name.ToLower()) {
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
            foreach (Digimon d in Digimons) {
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
            //Check if the level being stored is higher than the maximum or lower than -1. Note that -1 means the Digimon is locked.
            int maxExtraLevel = digimon.MaxExtraLevel;
            if (level > maxExtraLevel) level = maxExtraLevel;
            if (level < -1) level = -1;

            gm.LoadedGame.SetDigimonLevel(digimon.name, level);
        }

        public int GetDigimonLevel(string name) => gm.LoadedGame.GetDigimonLevel(name);

        public bool IsDigimonUnlocked(string name) => gm.LoadedGame.IsDigimonUnlocked(name);

        public void UnlockDigimon(string name) {
            if (!IsDigimonUnlocked(name)) gm.LoadedGame.SetDigimonLevel(name, 0);
        }
        public void UnlockDigimonCode(string name) => gm.LoadedGame.SetDigimonCodeUnlocked(name, true);
        public bool IsDigimonCodeUnlocked(string name) {
            return gm.LoadedGame.GetDigimonCodeUnlocked(name);
        }
        public void UnlockAllDigimon() {
            foreach(Digimon d in Digimons) {
                UnlockDigimon(d.name);
            }
        }
        public void LockAllDigimon() {
            foreach (Digimon d in Digimons) {
                SetDigimonLevel(d.name, -1);
            }
        }
        /// <summary>
        /// Returns true if the player has unlocked, at least, one digimon in that stage.
        /// </summary>
        public bool OwnsDigimonInStage(Stage stage) {
            foreach(Digimon d in Digimons) {
                if(d.stage == stage && IsDigimonUnlocked(d.name)) {
                    return true;
                }
            }
            return false;
        }

        public bool OwnsSpiritDigimonOfElement(Element element) {
            foreach (Digimon d in Digimons) {
                if (d.stage == Stage.Spirit && d.spiritType != SpiritType.Fusion && d.element == element && IsDigimonUnlocked(d.name)) {
                    return true;
                }
            }
            return false;
        }
        public bool OwnsFusionSpiritDigimon() {
            foreach (Digimon d in Digimons) {
                if (d.stage == Stage.Spirit && d.spiritType == SpiritType.Fusion && IsDigimonUnlocked(d.name)) {
                    return true;
                }
            }
            return false;
        }

        public bool IsInDock(string digimon) {
            for(int i = 0; i < 4; i++) {
                if (gm.LoadedGame.GetDDockDigimon(i) == digimon) {
                    return true;
                }
            }
            return false;
        }

        public string GetDDockDigimon(int ddock) => gm.LoadedGame.GetDDockDigimon(ddock);
        public void SetDDockDigimon(int ddock, string digimon) {
            if (ddock > 3) return; //The player only has 4 D-Docks.
            gm.LoadedGame.SetDDockDigimon(ddock, digimon);
        }

        public string GetDigimonFromCode(string code) {
            DigiCodes.TryGetValue(code.ToUpper(), out string digimon);
            return digimon;
        }

        public bool TryGetCodeOfDigimon(string digimon, out string code) {
            foreach(KeyValuePair<string, string> kv in DigiCodes) {
                if(kv.Value == digimon) {
                    code = kv.Key;
                    return true;
                }
            }
            code = "";
            return false;
        }
    }
}