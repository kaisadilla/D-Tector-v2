using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public class DatabaseManager {
        private GameManager gm;
        public Digimon[] Digimons { get; private set; }
        public Dictionary<string, string> DigiCodes { get; private set; }
        public DatabaseManager(GameManager gm) {
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

        public bool TryGetDigimonFromCode(string code, out string digimon) {
            if (DigiCodes.TryGetValue(code.ToUpper(), out digimon)) {
                return true;
            }
            return false;
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