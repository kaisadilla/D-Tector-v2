using Kaisa.Digivice.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Kaisa.Digivice {
    public static class SavedGame {
        public static string CurrentlyLoadedFilePath {
            get => EncryptedPlayerPrefs.GetString("CurrentFilePath");
            set => EncryptedPlayerPrefs.SetString("CurrentFilePath", value);
        }

        private static SavedGameFile lg;
        private static void SaveGame() => lg.WriteToFile();

        public static List<BriefSavedGame> GetAllSavedGames() {
            List<BriefSavedGame> briefSavedGames = new List<BriefSavedGame>();

            Directory.CreateDirectory($@"{Application.persistentDataPath}/savedGames");
            string[] files = Directory.GetFiles($@"{Application.persistentDataPath}/savedGames", "*.digivice");

            foreach(string file in files) {
                briefSavedGames.Add(GetBriefSavedGame(file));
            }

            return briefSavedGames;
        }
        public static BriefSavedGame GetBriefSavedGame(string filePath) {
            SavedGameFile temp = SavedGameFile.LoadSavedGame(filePath);
            BriefSavedGame bsg = new BriefSavedGame(temp.filePath, temp.name, ((GameChar)temp.gameChar).ToString(), temp.playerExperience);
            return bsg;
        }

        /// <summary>
        /// Creates a new Game and loads it. Returns the path to the created file.
        /// </summary>
        /// <param name="name">The name of the game.</param>
        /// <param name="index">The index associated with this game.</param>
        public static string CreateSavedGame(string name) {
            lg = SavedGameFile.CreateSavedGame(name);
            CurrentlyLoadedFilePath = lg.filePath;
            //PlayerChar = GameChar.none;
            return lg.filePath;
        }
        /// <summary>
        /// Loads an existing Game from a file.
        /// </summary>
        /// <param name="filePath">The path of the file.</param>
        public static void LoadSavedGame(string filePath) {
            lg = SavedGameFile.LoadSavedGame(filePath);
            CurrentlyLoadedFilePath = lg.filePath;
        }
        /// <summary>
        /// Deletes the physical file holding a saved game.
        /// </summary>
        /// <param name="filePath">The file to delete.</param>
        public static void DeleteSavedGame(string filePath) {
            //File.Delete(filePath);
            string disabledPath = filePath.Replace(".digivice", ".disabled");

            int savedGameNumber = 0;
            while (File.Exists($"{disabledPath}{savedGameNumber}")) {
                savedGameNumber++;
            }
            disabledPath += savedGameNumber.ToString();

            Debug.Log(disabledPath);
            File.Move(filePath, disabledPath);
        }
        /// <summary>
        /// Destroys the class holding the saved game information and replaces it with null.
        /// </summary>
        public static void CloseSavedGame() {
            lg = null;
        }
        public static string FilePath => lg.filePath;

        //Technical data:
        public static string Name {
            get => lg.name;
            set {
                lg.name = value;
                SaveGame();
            }
        }
        public static GameChar PlayerChar {
            get => (GameChar)lg.gameChar;
            set {
                lg.gameChar = (int)value;
                SaveGame();
            }
        }
        public static bool CheatsUsed {
            get => lg.cheatsUsed;
            set {
                lg.cheatsUsed = value;
                SaveGame();
            }
        }

        //Volatile data:
        public static int SavedEvent {
            get => lg.pendingEvent;
            set {
                lg.pendingEvent = value;
                SaveGame();
            }
        }
        public static bool IsPlayerInsured {
            get => lg.isPlayerInsured;
            set {
                lg.isPlayerInsured = value;
                SaveGame();
            }
        }
        public static bool IsLeaverBusterActive {
            get => lg.isLeaverBusterActive;
            set {
                lg.isLeaverBusterActive = value;
                SaveGame();
            }
        }
        public static int LeaverBusterExpLoss {
            get => lg.leaverBusterExpLoss;
            set {
                lg.leaverBusterExpLoss = value;
                SaveGame();
            }
        }
        public static string LeaverBusterDigimonLoss {
            get => lg.leaverBusterDigimonLoss;
            set {
                lg.leaverBusterDigimonLoss = value;
                SaveGame();
            }
        }

        //Current situation:
        public static int CurrentArea {
            get => lg.currentArea;
            set {
                lg.currentArea = value;
                SaveGame();
            }
        }
        public static int CurrentMap {
            get => lg.currentMap;
            set {
                lg.currentMap = value;
                SaveGame();
            }
        }
        public static int CurrentDistance {
            get => lg.currentDistance;
            set {
                lg.currentDistance = value;
                SaveGame();
            }
        }
        public static int Steps {
            get => lg.steps;
            set {
                lg.steps = value;
                SaveGame();
            }
        }
        public static int StepsToNextEvent {
            get => lg.stepsToNextEvent;
            set {
                lg.stepsToNextEvent = value;
                SaveGame();
            }
        }
        public static int PlayerExperience {
            get => lg.playerExperience;
            set {
                lg.playerExperience = value;
                SaveGame();
            }
        }
        public static int SpiritPower {
            get => lg.spiritPower;
            set {
                lg.spiritPower = value;
                SaveGame();
            }
        }
        public static int[] RandomSeed {
            get => lg.battleSeed;
            set {
                lg.battleSeed = value;
                SaveGame();
            }
        }
        public static int TotalBattles {
            get => lg.totalBattles;
            set {
                lg.totalBattles = value;
                SaveGame();
            }
        }
        public static int TotalWins {
            get => lg.totalWins;
            set {
                lg.totalWins = value;
                SaveGame();
            }
        }
        public static string[] DDockDigimon {
            get => lg.ddockDigimon;
            set {
                lg.ddockDigimon = value;
                SaveGame();
            }
        }

        //Progress data.
        public static int GetDigimonLevel(string digimon) {
            if(lg.digimonLevel.TryGetValue(digimon, out int level)) {
                return level;
            }
            return 0;
        }
        public static void SetDigimonLevel(string digimon, int level) {
            lg.digimonLevel[digimon] = level;
            SaveGame();
        }
        public static bool GetDigicodeUnlocked(string digimon) {
            if(lg.digicodeUnlocked.TryGetValue(digimon, out bool unlocked)) {
                return unlocked;
            }
            return false;
        }
        public static void SetDigicodeUnlocked(string digimon, bool value) {
            lg.digicodeUnlocked[digimon] = value;
            SaveGame();
        }
        public static bool[][] CompletedAreas {
            get => lg.areasCompleted;
            set {
                lg.areasCompleted = value;
                SaveGame();
            }
        }
        public static string[][] Bosses {
            get => lg.bosses;
            set {
                lg.bosses = value;
                SaveGame();
            }
        }
        public static int[] SemibossGroupForEachMap {
            get => lg.semibossGroup;
            set {
                lg.semibossGroup = value;
                SaveGame();
            }
        }
    }

    public struct BriefSavedGame {
        public readonly string filePath;
        public readonly string name;
        public readonly int level;
        public readonly string character;
        public BriefSavedGame(string filePath, string name, string character, int experience) {
            this.filePath = filePath;
            this.name = name;
            this.character = character;
            if (experience == 0) level = 1;
            else level = Mathf.FloorToInt(Mathf.Pow(experience, 1f / 3f));
        }
    }

    [System.Serializable]
    public class SavedGameFile {
        public string filePath;

        //Technical data:
        public string name;
        public int gameChar = (int)(GameChar.none);
        public bool cheatsUsed;

        //Volatile data:
        public int pendingEvent;
        public bool isPlayerInsured;
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
        public Dictionary<string, bool> digicodeUnlocked = new Dictionary<string, bool>();
        public bool[][] areasCompleted; //areaCompleted[mapIndex][areaIndex]
        public string[][] bosses; //bosses[mapIndex][areaIndex]
        public int[] semibossGroup; //semibossGroup[maxIndex]

        private SavedGameFile(string filePath, string name) {
            this.filePath = filePath;
            this.name = name;

            int[] areasPerMap = Database.AreasPerMap;

            areasCompleted = new bool[areasPerMap.Length][];
            bosses = new string[areasPerMap.Length][];
            semibossGroup = new int[areasPerMap.Length];
            //For each map, create an array with Length equal to the number of areas in that map.
            for (int i = 0; i < areasCompleted.Length; i++) {
                areasCompleted[i] = new bool[areasPerMap[i]];
                bosses[i] = new string[areasPerMap[i]];
            }
        }
        ~SavedGameFile() {
            WriteToFile();
        }

        public static SavedGameFile LoadSavedGame(string filePath) {
            SavedGameFile savedGame = null;
            if (File.Exists(filePath)) {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(filePath, FileMode.Open)) {
                    savedGame = (SavedGameFile)formatter.Deserialize(stream);
                }
                savedGame.filePath = filePath;
            }
            return savedGame;
        }
        public static SavedGameFile CreateSavedGame(string name) {
            int savedGameNumber = 0;
            while (File.Exists($"{Application.persistentDataPath}/savedGames/game{savedGameNumber}.digivice")) {
                savedGameNumber++;
            }
            string filePath = $"{Application.persistentDataPath}/savedGames/game{savedGameNumber}.digivice";
            SavedGameFile savedGame = new SavedGameFile(filePath, name);
            savedGame.WriteToFile();
            return savedGame;
        }

        public void WriteToFile() {
            BinaryFormatter formatter = new BinaryFormatter();
            using(FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate)) {
                formatter.Serialize(stream, this);
            }
        }
    }
}