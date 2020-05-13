using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static System.Environment;

namespace Kaisa.Digivice {
    public class DebugManager : MonoBehaviour {
        [SerializeField] private GameObject debugConsole;
        [SerializeField] private Text consoleOutput;
        [SerializeField] private InputField input;
        [SerializeField] private Scrollbar verticalScrollbar;

        private GameManager gm;

        private bool consoleActivated = false;
        public void Initialize(GameManager gm) {
            this.gm = gm;
        }
        public void Write(string output) {
            if (consoleOutput == null) return;

            consoleOutput.text += output;
            Canvas.ForceUpdateCanvases();
            verticalScrollbar.value = 0f;
        }
        public void Write(object output) => Write(output.ToString());

        public void WriteLine(string output) {
            Write(output + "\n");
        }
        public void WriteLine(object output) => WriteLine(output.ToString());

        public void ConsumeInput() {
            WriteLine(AnalyzeCommand(input.text));
            input.text = "";
        }
        public string AnalyzeCommand(string command) {
            command = command.ToLower();
            if (command.StartsWith("/loadedgame")) {
                return "Filepath of the current game: " + SavedGame.FilePath;
            }
            if (command.StartsWith("/gamename")) {
                return "Game name: " + SavedGame.Name;
            }
            if (command.StartsWith("/gamecharacter")) {
                return "Character: " + SavedGame.Name;
            }
            if (command.StartsWith("/currentmap")) {
                return "Current map: " + SavedGame.CurrentWorld;
            }
            if (command.StartsWith("/currentarea")) {
                return "Current area: " + SavedGame.CurrentArea;
            }
            if (command.StartsWith("/currentdistance")) {
                return "Current distance: " + SavedGame.CurrentDistance;
            }
            if (command.StartsWith("/totalsteps")) {
                return "Steps: " + SavedGame.Steps;
            }
            if (command.StartsWith("/stepstonextevent")) {
                return "Steps until the next event: " + SavedGame.StepsToNextEvent;
            }
            if (command.StartsWith("/playerexperience")) {
                return "Player experience: " + SavedGame.PlayerExperience;
            }
            if (command.StartsWith("/playerspiritpower")) {
                return "Player spirit power: " + SavedGame.SpiritPower;
            }
            if (command.StartsWith("/totalwins")) {
                return "Total wins: " + SavedGame.TotalWins;
            }
            if (command.StartsWith("/isInputLocked")) {
                return "Input Locked: " + gm.IsInputLocked;
            }
            if (command.StartsWith("/getdigimonlevel")) {
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    if (Database.GetDigimon(args[1]) == null) return "Digimon not found.";
                    return "Digimon " + args[1] + " level: " + SavedGame.GetDigimonLevel(args[1]);
                }
                return "Invalid parameters. Expected (string)digimonName";
            }
            if (command.StartsWith("/isareacompleted")) {
                string[] args = command.Split(' ');
                if (args.Length == 3) {
                    try {
                        return $"Area + {args[1]}, {args[2]} completed: {SavedGame.CompletedAreas[int.Parse(args[1])][int.Parse(args[2])]}";
                    }
                    catch {
                        return "Invalid parameter.";
                    }
                }
                return "Invalid parameters. Expected (int)map, (int)area";
            }
            //Reports
            if (command.StartsWith("/generatereport")) {
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    if (args[1] == "expgain") {
                        string filePath = GenerateLevelEmulation();
                        return $"Report created in {filePath}";
                    }
                    else {
                        return "Unknown report.";
                    }
                }
                return "Invalid parameters. Expected (string)report.";
            }
            if (command.StartsWith("/emulateattacks")) {
                string[] args = command.Split(' ');
                if (args.Length == 1) {
                    return GenerateAttackEmulation();
                }
                else if (args.Length == 2) {
                    return GenerateAttackEmulationForDigimon(args[1]);
                }
            }
            if (command.StartsWith("/printallbosses")) {
                string filePath = PrintAllBossesToFile();
                return $"Report created in {filePath}";
            }
            if (command.StartsWith("/cheatsused")) {
                return $"Cheats used this game: {SavedGame.CheatsUsed}";
            }
            if (command.StartsWith("/getcurrentdistance")) {
                return SavedGame.CurrentDistance.ToString();
            }
            if (command.StartsWith("/getdistanceforarea")) {
                string[] args = command.Split(' ');
                if (args.Length == 3) {
                    try {
                        string result = Database.Worlds[int.Parse(args[1])].areas[int.Parse(args[2])].distance.ToString();
                        return result;
                    }
                    catch {
                    }
                }
                return "Invalid parameters. Expected (int)map, (int)area";
            }
            if (command.StartsWith("/checkabilitysprites")) {
                CheckDigimonAbilities();
                return "";
            }
            //===============================================================//
            //====== COMMANDS THAT WON'T WORK BEFORE /letmecheatplease ======//
            //===============================================================//
            if (command.StartsWith("/letmecheatplease")) {
                EnableCheats();
                return "Cheat commands are now enabled.";
            }
            //These commands modify the data of the game and will trigger "CheatsUsed".
            if (!consoleActivated) return "Invalid command.";

            if (command.StartsWith("/cancelbattle")) {
                gm.DisableLeaverBuster();
                return "LeaverBuster disabled for this battle if you close the application now.";
            }
            if (command.StartsWith("/setspiritpower")) {
                SavedGame.CheatsUsed = true;
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    try {
                        SavedGame.SpiritPower = int.Parse(args[1]);
                        return "Current spirit power: " + SavedGame.SpiritPower;
                    }
                    catch {
                        return "Invalid number.";
                    }
                }
                return "Invalid parameters. Expected (int)amount";
            }
            if (command.StartsWith("/setdigimonlevel")) {
                SavedGame.CheatsUsed = true;
                string[] args = command.Split(' ');
                if (args.Length == 3) {
                    if (Database.GetDigimon(args[1]) == null) return "Digimon not found.";
                    if (args[2].StartsWith("max")) {
                        int maxLevel = Database.GetDigimon(args[1]).MaxExtraLevel + 1;
                        SavedGame.SetDigimonLevel(args[1], maxLevel);
                        return $"Digimon {args[1]} level set to: {maxLevel}";
                    }
                    else {
                        try {
                            SavedGame.SetDigimonLevel(args[1], int.Parse(args[2]));
                            return "Digimon " + args[1] + " level set to: " + SavedGame.GetDigimonLevel(args[1]);
                        }
                        catch {
                            return "Invalid number.";
                        }
                    }
                }
                return "Invalid parameters. Expected (string)digimonName, (int)level / (string)\"max\".";
            }
            if (command.StartsWith("/unlockalldigimon")) {
                SavedGame.CheatsUsed = true;
                UnlockAllDigimon();
                return "All Digimon have been unlocked.";
            }
            if (command.StartsWith("/lockalldigimon")) {
                SavedGame.CheatsUsed = true;
                LockAllDigimon();
                return "All Digimon have been locked.";
            }
            if (command.StartsWith("/setdigimoncodeunlocked")) {
                SavedGame.CheatsUsed = true;
                string[] args = command.Split(' ');
                if (args.Length == 3) {
                    if (Database.GetDigimon(args[1]) == null) return "Digimon not found.";
                    if (args[2].ToLower() == "true") {
                        SavedGame.SetDigicodeUnlocked(args[1], true);
                        return "Code for " + args[1] + " unlocked set to: true.";
                    }
                    else if (args[2].ToLower() == "false") {
                        SavedGame.SetDigicodeUnlocked(args[1], false);
                        return "Code for " + args[1] + " unlocked set to: false.";
                    }
                    else {
                        return "Invalid value. Value can only be 'true' or 'false'";
                    }
                }
                return "Invalid parameters. Expected (string)digimonName, (true/false)unlocked.";
            }
            if (command.StartsWith("/setplayerexperience")) {
                SavedGame.CheatsUsed = true;
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    try {
                        SavedGame.PlayerExperience = int.Parse(args[1]);
                        return $"Player experience set to {args[1]}";
                    }
                    catch {
                        return "Invalid number.";
                    }
                }
                return "Invalid parameters. Expected (int)experience.";
            }
            if (command.StartsWith("/setcurrentdistance")) {
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    try {
                        SavedGame.CurrentDistance = int.Parse(args[1]);
                        return $"Current distance set to {int.Parse(args[1])}";
                    }
                    catch {
                    }
                }
                return "Invalid parameters. Expected (int)distance";
            }
            if (command.StartsWith("/setstepstonextevent")) {
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    try {
                        SavedGame.StepsToNextEvent = int.Parse(args[1]);
                        return $"Steps to next event: {int.Parse(args[1])}";
                    }
                    catch {
                    }
                }
                return "Invalid parameters. Expected (int)steps";
            }
            if (command.StartsWith("/empowerenergy")) {
                if (gm.logicMgr.loadedApp is App.Battle b) {
                    b.CheatEnergy();
                    return "Energy empowered.";
                }
                else {
                    return "Player is not in a battle!";
                }
            }

            return "Invalid command.";
        }

        public void EnableCheats() {
            consoleActivated = true;
        }
        private void UnlockAllDigimon() {
            foreach (Digimon d in Database.Digimons) {
                if (d.name != Constants.DEFAULT_SPIRIT_DIGIMON) gm.logicMgr.SetDigimonUnlocked(d.name, true);
            }
        }
        private void LockAllDigimon() {
            foreach (Digimon d in Database.Digimons) {
                gm.logicMgr.SetDigimonUnlocked(d.name, false);
            }
        }

        private string GenerateLevelEmulation() {
            string filePath = GetFolderPath(SpecialFolder.MyDocuments) + @"\dtector_level_emulation.txt";
            using (StreamWriter file = new StreamWriter(filePath)) {
                file.WriteLine("Enemy\tLevel\tVictoryExp\tPercTotal\tDefeatExp\tPercTotal");
                for (int level = 1; level < 100; level++) {
                    int expThisLevel = (int)Mathf.Pow(level, 3);
                    int expNeeded = (int)Mathf.Pow(level + 1, 3) - expThisLevel;
                    file.WriteLine($"==== LEVEL {level} ====");
                    file.WriteLine($"base exp: {expThisLevel}. Next level at: {expNeeded}");
                    for (int i = 0; i < 10; i++) {
                        Digimon d = Database.GetRandomDigimonForBattle(level);
                        if (d == null) {
                            file.WriteLine($"Digimon not found.");
                        }
                        else {
                            uint victoryExp = gm.logicMgr.GetExperienceGained(level, d.baseLevel);
                            uint defeatExp = gm.logicMgr.GetExperienceGained(d.baseLevel, level);
                            file.WriteLine($"{d.name}\t{d.baseLevel}\t{victoryExp}\t{victoryExp / (float)expNeeded}\t{defeatExp}\t{defeatExp / (float)expNeeded}");
                        }
                    }
                }
            }
            return filePath;
        }

        private string GenerateAttackEmulation() {
            int energy = 0, crush = 0, ability = 0, invalid = 0;
            System.Random enemyAttackRNG;

            for (int level = 1; level < 100; level++) {
                Digimon d = Database.GetRandomDigimonForBattle(level);
                if (d == null) continue;
                enemyAttackRNG = new System.Random(gm.GetRandomSavedSeed());
                for (int i = 0; i < 10; i++) {
                    int attack = ChooseEnemyAttack(d.GetRegularStats());
                    if (attack == 0) energy++;
                    else if (attack == 1) crush++;
                    else if (attack == 2) ability++;
                    else invalid++;
                }
            }

            return $"En: {energy}, Cr: {crush}, Ab: {ability}, Invalid {invalid} <= This should be 0.";

            int ChooseEnemyAttack(MutableCombatStats enemyStats) {
                int total = enemyStats.EN + enemyStats.CR + enemyStats.AB;
                int rngNumber = enemyAttackRNG.Next(total);

                if (rngNumber < enemyStats.EN) return 0;
                else if (rngNumber < enemyStats.EN + enemyStats.CR) return 1;
                else return 2;
            }
        }
        private string GenerateAttackEmulationForDigimon(string digimon) {
            int energy = 0, crush = 0, ability = 0, invalid = 0;
            System.Random enemyAttackRNG;

            for (int attempt = 1; attempt < 50; attempt++) {
                Digimon d = Database.GetDigimon(digimon);
                if (d == null) continue;
                enemyAttackRNG = new System.Random(gm.GetRandomSavedSeed());
                Debug.Log($"Attack selected: {enemyAttackRNG}");
                for (int i = 0; i < 20; i++) {
                    int attack = ChooseEnemyAttack(d.GetRegularStats());
                    Debug.Log($"Attack selected: {attack}");
                    if (attack == 0) energy++;
                    else if (attack == 1) crush++;
                    else if (attack == 2) ability++;
                    else invalid++;
                }
            }

            return $"En: {energy}, Cr: {crush}, Ab: {ability}, Invalid {invalid} <= This should be 0.";

            int ChooseEnemyAttack(MutableCombatStats enemyStats) {
                int total = enemyStats.EN + enemyStats.CR + enemyStats.AB;
                Debug.Log($"EN {enemyStats.EN}, CR: {enemyStats.CR}, AB: {enemyStats.AB}, total {total}");
                int rngNumber = enemyAttackRNG.Next(total);
                Debug.Log($"Number chosen: {rngNumber}");

                if (rngNumber < enemyStats.EN) return 0;
                else if (rngNumber < enemyStats.EN + enemyStats.CR) return 1;
                else return 2;
            }
        }

        public void Initialize() {
            gm.AttemptUpdateGame();
        }

        public void ShowDebug() {
            debugConsole.SetActive(!debugConsole.activeSelf);
        }
        private string PrintAllBossesToFile() {
            string filePath = GetFolderPath(SpecialFolder.MyDocuments) + @"\dtector_all_bosses.txt";
            using (StreamWriter file = new StreamWriter(filePath)) {
                for(int world = 0; world < Database.Worlds.Length; world++) {
                    string[] bosses = gm.WorldMgr.GetBossesForWorld(world);
                    file.WriteLine($"\n==== World {world} ====");

                    for(int area = 0; area < bosses.Length; area++) {
                        file.WriteLine($"World {world}, area {area}: {bosses[area]}");
                    }

                    file.WriteLine($"World {world} semiboss set: {SavedGame.SemibossGroupForEachMap[world]}");
                }
            }
            return filePath;
        }

        private void CheckDigimonAbilities() {
            foreach (Digimon d in Database.Digimons) {
                Sprite ability = gm.spriteDB.GetAbilitySprite(d.abilityName);
                if (ability == null) {
                    Console.WriteLine("No ability found for " + d.name);
                }
            }
        }
    }
}