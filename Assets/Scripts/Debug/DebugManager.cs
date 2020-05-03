using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static System.Environment;

namespace Kaisa.Digivice {
    public class DebugManager : MonoBehaviour {
        [SerializeField]
        private GameObject debugConsole;
        [SerializeField]
        private Text consoleOutput;
        [SerializeField]
        private InputField input;

        private GameManager gm;
        private SavedGame loadedGame;
        public void Initialize(GameManager gm, SavedGame loadedGame) {
            this.gm = gm;
            this.loadedGame = loadedGame;
        }
        public void Write(string output) {
            consoleOutput.text += output;
        }
        public void Write(object output) => Write(output.ToString());

        public void WriteLine(string output) {
            if (consoleOutput.cachedTextGenerator.lines.Count > 9) {
                consoleOutput.text = "";
                /*foreach(var s in consoleOutput.cachedTextGenerator.lines) {
                    consoleOutput.text = s.ToString();
                }*/
            }
            consoleOutput.text += output + "\n";
        }
        public void WriteLine(object output) => WriteLine(output.ToString());

        public void ConsumeInput() {
            WriteLine(AnalyzeCommand(input.text));
            input.text = "";
        }
        public string AnalyzeCommand(string command) {
            command = command.ToLower();
            if (command.StartsWith("/currentslot")) {
                return "Current slot: " + loadedGame.Slot;
            }
            if (command.StartsWith("/checkslot")) {
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    try {
                        return SavedGame.IsSlotUsed(int.Parse(args[1])).ToString();
                    }
                    catch {
                        return "Invalid number.";
                    }
                }
                return "Invalid number of parameters.";
            }
            if (command.StartsWith("/gamename")) {
                return "Game name: " + loadedGame.Name;
            }
            if (command.StartsWith("/gamecharacter")) {
                return "Character: " + loadedGame.Name;
            }
            if (command.StartsWith("/currentmap")) {
                return "Current map: " + loadedGame.CurrentMap;
            }
            if (command.StartsWith("/currentarea")) {
                return "Current area: " + loadedGame.CurrentArea;
            }
            if (command.StartsWith("/currentdistance")) {
                return "Current distance: " + loadedGame.CurrentDistance;
            }
            if (command.StartsWith("/totalsteps")) {
                return "Steps: " + loadedGame.Steps;
            }
            if (command.StartsWith("/stepstonextevent")) {
                return "Steps until the next event: " + loadedGame.StepsToNextEvent;
            }
            if (command.StartsWith("/playerexperience")) {
                return "Player experience: " + loadedGame.PlayerExperience;
            }
            if (command.StartsWith("/playerspiritpower")) {
                return "Player spirit power: " + loadedGame.SpiritPower;
            }
            if (command.StartsWith("/totalwins")) {
                return "Total wins: " + loadedGame.TotalWins;
            }
            if (command.StartsWith("/getdigimonlevel")) {
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    if (gm.DatabaseMgr.GetDigimon(args[1]) == null) return "Digimon not found.";
                    return "Digimon " + args[1] + " level: " + loadedGame.GetDigimonLevel(args[1]);
                }
                return "Invalid parameters. Expected (string)digimonName";
            }
            if (command.StartsWith("/isareacompleted")) {
                string[] args = command.Split(' ');
                if (args.Length == 3) {
                    try {
                        return "Area + " + args[1] + ", " + args[2] + " completed: " + loadedGame.GetAreaCompleted(int.Parse(args[1]), int.Parse(args[2]));
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
            if (command.StartsWith("/cheatsused")) {
                return $"Cheats used this game: {loadedGame.CheatsUsed}";
            }
            //Commands that modify the data of the game and trigger "CheatsUsed".
            if (command.StartsWith("/setspiritpower")) {
                loadedGame.CheatsUsed = true;
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    try {
                        loadedGame.SpiritPower = int.Parse(args[1]);
                        return "Current spirit power: " + loadedGame.SpiritPower;
                    }
                    catch {
                        return "Invalid number.";
                    }
                }
                return "Invalid parameters. Expected (int)amount";
            }
            if (command.StartsWith("/setdigimonlevel")) {
                loadedGame.CheatsUsed = true;
                string[] args = command.Split(' ');
                if (args.Length == 3) {
                    if (gm.DatabaseMgr.GetDigimon(args[1]) == null) return "Digimon not found.";
                    if (args[2].StartsWith("max")) {
                        int maxLevel = gm.DatabaseMgr.GetDigimon(args[1]).MaxExtraLevel + 1;
                        loadedGame.SetDigimonLevel(args[1], maxLevel);
                        return $"Digimon {args[1]} level set to: {maxLevel}";
                    }
                    else {
                        try {
                            loadedGame.SetDigimonLevel(args[1], int.Parse(args[2]));
                            return "Digimon " + args[1] + " level set to: " + loadedGame.GetDigimonLevel(args[1]);
                        }
                        catch {
                            return "Invalid number.";
                        }
                    }
                }
                return "Invalid parameters. Expected (string)digimonName, (int)level / (string)\"max\".";
            }
            if (command.StartsWith("/unlockalldigimon")) {
                loadedGame.CheatsUsed = true;
                UnlockAllDigimon();
                return "All Digimon have been unlocked.";
            }
            if (command.StartsWith("/lockalldigimon")) {
                loadedGame.CheatsUsed = true;
                LockAllDigimon();
                return "All Digimon have been locked.";
            }
            if (command.StartsWith("/setdigimoncodeunlocked")) {
                loadedGame.CheatsUsed = true;
                string[] args = command.Split(' ');
                if (args.Length == 3) {
                    if (gm.DatabaseMgr.GetDigimon(args[1]) == null) return "Digimon not found.";
                    if (args[2].ToLower() == "true") {
                        loadedGame.SetDigimonCodeUnlocked(args[1], true);
                        return "Code for " + args[1] + " unlocked set to: true.";
                    }
                    else if (args[2].ToLower() == "false") {
                        loadedGame.SetDigimonCodeUnlocked(args[1], false);
                        return "Code for " + args[1] + " unlocked set to: false.";
                    }
                    else {
                        return "Invalid value. Value can only be 'true' or 'false'";
                    }
                }
                return "Invalid parameters. Expected (string)digimonName, (true/false)unlocked.";
            }
            if (command.StartsWith("/setplayerexperience")) {
                loadedGame.CheatsUsed = true;
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    try {
                        loadedGame.PlayerExperience = int.Parse(args[1]);
                        return $"Player experience set to {args[1]}";
                    }
                    catch {
                        return "Invalid number.";
                    }
                }
                return "Invalid parameters. Expected (int)experience.";
            }

            return "Invalid command.";
        }
        private void UnlockAllDigimon() {
            foreach (Digimon d in gm.DatabaseMgr.Digimons) {
                if(d.name != Constants.DEFAULT_SPIRIT_DIGIMON) gm.logicMgr.SetDigimonUnlocked(d.name, true);
            }
        }
        private void LockAllDigimon() {
            foreach (Digimon d in gm.DatabaseMgr.Digimons) {
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
                        Digimon d = gm.DatabaseMgr.GetWeightedDigimon(level);
                        if (d == null) {
                            file.WriteLine($"Digimon not found.");
                        }
                        else {
                            int victoryExp = gm.logicMgr.GetExperienceGained(level, d.baseLevel);
                            int defeatExp = gm.logicMgr.GetExperienceGained(d.baseLevel, level);
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
                Digimon d = gm.DatabaseMgr.GetWeightedDigimon(level);
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
                Digimon d = gm.DatabaseMgr.GetDigimon(digimon);
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
            gm.DebugInitialize();
        }

        public void ShowDebug() {
            debugConsole.SetActive(!debugConsole.activeSelf);
        }
    }
}