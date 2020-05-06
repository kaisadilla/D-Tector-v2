using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public class DatabaseManager {
        private GameManager gm;
        public Digimon[] Digimons { get; private set; }
        public Dictionary<string, string> DigiCodes { get; private set; }
        public int[] AreasPerMap; //Stores the number of areas that there are in each map.
        public string[][][] Bosses { get; private set; }
        public DatabaseManager(GameManager gm) {
            this.gm = gm;
            LoadDatabase();
        }

        public void LoadDatabase() {
            LoadDigimonDB();
            LoadDigiCodeDB();
            LoadBossesDB();
            LoadAreaCount();
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
            string digiCodes = ((TextAsset)Resources.Load("codeDB")).text;
            DigiCodes = JsonConvert.DeserializeObject<Dictionary<string, string>>(digiCodes);
        }

        private void LoadBossesDB() {
            string boses = ((TextAsset)Resources.Load("bosses")).text;
            Bosses = JsonConvert.DeserializeObject<string[][][]>(boses);
        }
        private void LoadAreaCount() {
            string areas = ((TextAsset)Resources.Load("areas")).text;
            AreasPerMap = JsonConvert.DeserializeObject<int[]>(areas);
        }

        public Digimon GetDigimon(string name) {
            foreach (Digimon d in Digimons) {
                if (d.name == name?.ToLower()) {
                    return d;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets a digimon at random from those in the database that can be chosen for a random battle.
        /// The level of the Digimon will be similar to the level of the player.
        /// </summary>
        /// <param name="playerLevel">The level of the player.</param>
        public Digimon GetRandomDigimonForBattle(int playerLevel) {
            List<Digimon> candidates = new List<Digimon>();
            List<float> weightList = new List<float>();
            float totalWeight = 0f;

            int threshold; //The maximum level difference between the player and the chosen digimon.
            if (playerLevel <= 2) threshold = 3;
            else if (playerLevel <= 4) threshold = 4;
            else if (playerLevel <= 10) threshold = 7;
            else if (playerLevel <= 60) threshold = 10;
            else if (playerLevel <= 80) threshold = 20;
            else threshold = 40;

            //Populate the 'candidates' list with all eligible digimon, and store their individual weights and the total weight.
            foreach (Digimon d in Digimons) {
                if (!d.disabled && d.GetCurrentRarity() != Rarity.Boss
                        && d.baseLevel >= (playerLevel - threshold)
                        && d.baseLevel <= (playerLevel + threshold))
                {
                    if (d.rarity == Rarity.Boss || d.rarity == Rarity.none || d.exclusive) continue; //Ignore bosses and 'none' rarity.

                    candidates.Add(d);

                    float baseWeight = 0f;
                    switch(d.rarity) {
                        case Rarity.Common:
                            baseWeight = 10f;
                            break;
                        case Rarity.Rare:
                            baseWeight = 6f;
                            break;
                        case Rarity.Epic:
                            baseWeight = 3f;
                            break;
                        case Rarity.Legendary:
                            baseWeight = 1f;
                            break;
                    }


                    float thisWeight = (1.1f - (Mathf.Abs(playerLevel - d.baseLevel) / (float)threshold)) * baseWeight;
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
        /// <summary>
        /// Gets a random Digimon of the given rarity, with level no higher than the level specified.
        /// </summary>
        /// <returns></returns>
        public Digimon GetRandomDigimonOfRarity(Rarity rarity, int maximumLevel) {
            List<Digimon> candidates = new List<Digimon>();
            foreach(Digimon d in Digimons) {
                if(d.rarity == rarity && d.baseLevel <= maximumLevel) {
                    candidates.Add(d);
                }
            }
            return candidates[Random.Range(0, candidates.Count)];
        }

        public bool TryGetDigimonFromCode(string code, out string digimon) {
            if (DigiCodes.TryGetValue(code.ToLower(), out digimon)) {
                return true;
            }
            return false;
        }

        public bool TryGetCodeOfDigimon(string digimon, out string code) {
            foreach(KeyValuePair<string, string> kv in DigiCodes) {
                if (kv.Value == digimon) {
                    code = kv.Key;
                    return true;
                }
            }
            code = "";
            return false;
        }
    }
}