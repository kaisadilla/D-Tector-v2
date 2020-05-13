using Kaisa.Digivice.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    public static class Database {
        private static bool databasesLoaded = false;
        public static Digimon[] Digimons { get; private set; }
        public static DigimonRarity[] Rarities { get; private set; }
        public static World[] Worlds { get; private set; }
        public static Dictionary<GameChar, string> PlayerSpirit = new Dictionary<GameChar, string>();

        static Database() {
            Debug.Log("Loading Database...");
            LoadDatabases();
        }

        public static void LoadDatabases() {
            if (databasesLoaded) return;
            Digimons = LoadDigimonFromFile();
            Rarities = LoadRaritiesFromFile();
            Worlds = LoadWorldsFromFile();

            SetupPlayerSpirit();
            databasesLoaded = true;
        }

        /// <summary>
        /// Loads all the Digimon that are not marked as "disabled" from the json database.
        /// </summary>
        private static Digimon[] LoadDigimonFromFile() {
            string digimonDBJson = ((TextAsset)Resources.Load("digimonDB")).text;
            JArray dbArray = JArray.Parse(digimonDBJson);

            List<Digimon> tempList = new List<Digimon>();

            for (int i = 0; i < dbArray.Count; i++) {
                Digimon d = dbArray[i].ToObject<Digimon>();

                if (!d.disabled) {
                    tempList.Add(d);
                }
            }

            return tempList.ToArray();
        }

        private static DigimonRarity[] LoadRaritiesFromFile() {
            string rarities = ((TextAsset)Resources.Load("frontier_rarities")).text;
            return JsonConvert.DeserializeObject<DigimonRarity[]>(rarities);
        }

        private static World[] LoadWorldsFromFile() {
            string worlds = ((TextAsset)Resources.Load("worlds")).text;
            return JsonConvert.DeserializeObject<World[]>(worlds);
        }

        /*private static int[] LoadAreasFromFile() {
            string areas = ((TextAsset)Resources.Load("areas")).text;
            return JsonConvert.DeserializeObject<int[]>(areas);
        }
        public static string[][][] LoadBossesFromFile() {
            string boses = ((TextAsset)Resources.Load("bosses")).text;
            return JsonConvert.DeserializeObject<string[][][]>(boses);
        }*/

        //These files are not loaded into the database as they won't be normally used. Instead, other classes call these methods when needed.

        public static string[] LoadInitialDigimonsFromFile() {
            string initials = ((TextAsset)Resources.Load("initials")).text;
            return JsonConvert.DeserializeObject<string[]>(initials);
        }
        private static void SetupPlayerSpirit() {
            PlayerSpirit[GameChar.Takuya] = "agunimon";
            PlayerSpirit[GameChar.Koji] = "lobomon";
            PlayerSpirit[GameChar.Zoe] = "kazemon";
            PlayerSpirit[GameChar.JP] = "beetlemon";
            PlayerSpirit[GameChar.Tommy] = "kumamon";
            PlayerSpirit[GameChar.Koichi] = "loweemon";
        }

        public static Digimon GetDigimon(string name) {
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
        public static Digimon GetRandomDigimonForBattle(int playerLevel) {
            List<string> candidates = new List<string>();
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
            foreach (DigimonRarity r in Rarities) {
                if(r.EligibleForBattle) {
                    Digimon thisDigimon = GetDigimon(r.digimon);
                    if(thisDigimon != null || thisDigimon.disabled) {
                        if (thisDigimon.baseLevel > (playerLevel - threshold) //The digimon's level is higher than the minimum.
                            && thisDigimon.baseLevel < (playerLevel + threshold)) //And lower than the maximum.
                        {
                            candidates.Add(r.digimon);

                            float baseWeight = 0f;
                            switch (r.Rarity) {
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

                            float thisWeight = (1.1f - (Mathf.Abs(playerLevel - thisDigimon.baseLevel) / (float)threshold)) * baseWeight;
                            weightList.Add(thisWeight);
                            totalWeight += thisWeight;
                        }
                    }
                }
            }

            float fChosen = Random.Range(0, totalWeight);
            float weightSum = 0f;

            VisualDebug.WriteLine($"Weighted Digimon: candidates found: {candidates.Count} fChosen: {fChosen}, totalWeight: {totalWeight}");

            for (int i = 0; i < weightList.Count; i++) {
                weightSum += weightList[i];
                if (weightSum > fChosen) {
                    return GetDigimon(candidates[i]);
                }
            }

            return null;
        }
        /// <summary>
        /// Gets a random Digimon of the given rarity, with level no higher than the level specified.
        /// </summary>
        /// <returns></returns>
        public static Digimon GetRandomDigimonOfRarity(Rarity rarity, int maximumLevel) {
            List<Digimon> candidates = new List<Digimon>();

            foreach (DigimonRarity r in Rarities) {
                if(r.Rarity == rarity) {
                    Digimon digimon = GetDigimon(r.digimon);
                    if (digimon.baseLevel <= maximumLevel) {
                        candidates.Add(digimon);
                    }
                }
            }

            return candidates.GetRandomElement();
        }

        /// <summary>
        /// Returns the chance that this Digimon will be erased, based on its rarity.
        /// </summary>
        public static float GetEraseChance(string digimon) {
            //You can't lose Default digimons.
            if (digimon == Constants.DEFAULT_DIGIMON.ToLower()
                || digimon == Constants.DEFAULT_SPIRIT_DIGIMON.ToLower()) 
            {
                return 0f; 
            }

            Rarity rarity = GetDigimonRarity(digimon);
            switch (rarity) {
                case Rarity.Common: return 0.75f;
                case Rarity.Rare: return 0.50f;
                case Rarity.Epic: return 0.25f;
                case Rarity.Legendary: return 0.10f;
                case Rarity.Boss:
                    Digimon d = GetDigimon(digimon);
                    if (d.stage == Stage.Spirit) {
                        if (d.spiritType == SpiritType.Human || d.spiritType == SpiritType.Animal) return 0.50f;
                        else return 0f;
                    }
                    else return 0.10f;
                case Rarity.none: return 0f;
                default: return 0f;
            }
        }

        public static Rarity GetDigimonRarity(string digimon) {
            foreach(DigimonRarity r in Rarities) {
                if (r.digimon == digimon) return r.Rarity;
            }
            return Rarity.none;
        }
        
        public static Digimon GetDigimonFromCode(string code) {
            code = code.ToLower();
            foreach(Digimon d in Digimons) {
                if(d.code == code) {
                    return d;
                }
            }
            return null;
        }
        /*
        public static bool TryGetCodeOfDigimon(string digimon, out string code) {
            foreach(KeyValuePair<string, string> kv in DigiCodes) {
                if (kv.Value == digimon) {
                    code = kv.Key;
                    return true;
                }
            }
            code = "";
            return false;
        }*/
    }
}