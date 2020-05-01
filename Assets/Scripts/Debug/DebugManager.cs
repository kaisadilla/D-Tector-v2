using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class DebugManager : MonoBehaviour {
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
            if(consoleOutput.cachedTextGenerator.lines.Count > 9) {
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

            if (command.StartsWith("/setspiritpower")) {
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
                string[] args = command.Split(' ');
                if (args.Length == 3) {
                    if (gm.DatabaseMgr.GetDigimon(args[1]) == null) return "Digimon not found.";
                    try {
                        loadedGame.SetDigimonLevel(args[1], int.Parse(args[2]));
                        return "Digimon " + args[1] + " level set to: " + loadedGame.GetDigimonLevel(args[1]);
                    }
                    catch {
                        return "Invalid number.";
                    }
                }
                return "Invalid parameters. Expected (string)digimonName, (int)level.";
            }
            if (command.StartsWith("/unlockalldigimon")) {
                UnlockAllDigimon();
                return "All Digimon have been unlocked.";
            }
            if (command.StartsWith("/lockalldigimon")) {
                LockAllDigimon();
                return "All Digimon have been locked.";
            }
            if (command.StartsWith("/setdigimoncodeunlocked")) {
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

            return "Invalid command.";
        }
        private void UnlockAllDigimon() {
            foreach (Digimon d in gm.DatabaseMgr.Digimons) {
                gm.logicMgr.SetDigimonUnlocked(d.name, true);
            }
        }
        private void LockAllDigimon() {
            foreach (Digimon d in gm.DatabaseMgr.Digimons) {
                gm.logicMgr.SetDigimonUnlocked(d.name, false);
            }
        }
    }
}