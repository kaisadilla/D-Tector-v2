using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class DebugManager : MonoBehaviour {
        [SerializeField]
        private Text output;
        [SerializeField]
        private InputField input;

        private GameManager gm;
        public void AssignManagers(GameManager gm) {
            this.gm = gm;
        }
        public void Write(string output) {
            this.output.text += output;
        }
        public void Write(object output) => Write(output.ToString());

        public void WriteLine(string output) {
            this.output.text += output + "\n";
        }
        public void WriteLine(object output) => WriteLine(output.ToString());

        public void ConsumeInput() {
            WriteLine(AnalyzeCommand(input.text));
            input.text = "";
        }
        public string AnalyzeCommand(string command) {
            command = command.ToLower();
            if (command.StartsWith("/currentslot")) {
                return "Current slot: " + gm.LoadedGame.Slot;
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
                return "Game name: " + gm.LoadedGame.Name;
            }
            if (command.StartsWith("/gamecharacter")) {
                return "Character: " + gm.LoadedGame.Name;
            }
            if (command.StartsWith("/currentmap")) {
                return "Current map: " + gm.LoadedGame.CurrentMap;
            }
            if (command.StartsWith("/currentarea")) {
                return "Current area: " + gm.LoadedGame.CurrentArea;
            }
            if (command.StartsWith("/currentdistance")) {
                return "Current distance: " + gm.LoadedGame.CurrentDistance;
            }
            if (command.StartsWith("/totalsteps")) {
                return "Steps: " + gm.LoadedGame.Steps;
            }
            if (command.StartsWith("/stepstonextevent")) {
                return "Steps until the next event: " + gm.LoadedGame.StepsToNextEvent;
            }
            if (command.StartsWith("/playerexperience")) {
                return "Player experience: " + gm.LoadedGame.PlayerExperience;
            }
            if (command.StartsWith("/playerspiritpower")) {
                return "Player spirit power: " + gm.LoadedGame.SpiritPower;
            }
            if (command.StartsWith("/totalwins")) {
                return "Total wins: " + gm.LoadedGame.TotalWins;
            }
            if (command.StartsWith("/getdigimonlevel")) {
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    if (gm.Database.GetDigimon(args[1]) == null) return "Digimon not found.";
                    return "Digimon " + args[1] + " level: " + gm.LoadedGame.GetDigimonLevel(args[1]);
                }
                return "Invalid parameters. Expected (string)digimonName";
            }
            if (command.StartsWith("/isareacompleted")) {
                string[] args = command.Split(' ');
                if (args.Length == 3) {
                    try {
                        return "Area + " + args[1] + ", " + args[2] + " completed: " + gm.LoadedGame.GetAreaCompleted(int.Parse(args[1]), int.Parse(args[2]));
                    }
                    catch {
                        return "Invalid parameter.";
                    }
                }
                return "Invalid parameters. Expected (int)map, (int)area";
            }

            if (command.StartsWith("/addspiritpower")) {
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    try {
                        gm.LoadedGame.AddSpiritPower(int.Parse(args[1]));
                        return "Current spirit power: " + gm.LoadedGame.SpiritPower;
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
                    if (gm.Database.GetDigimon(args[1]) == null) return "Digimon not found.";
                    try {
                        gm.LoadedGame.SetDigimonLevel(args[1], int.Parse(args[2]));
                        return "Digimon " + args[1] + " level set to: " + gm.LoadedGame.GetDigimonLevel(args[1]);
                    }
                    catch {
                        return "Invalid number.";
                    }
                }
                return "Invalid parameters. Expected (string)digimonName, (int)level.";
            }
            if (command.StartsWith("/unlockalldigimon")) {
                gm.Database.UnlockAllDigimon();
                return "All Digimon have been unlocked.";
            }
            if (command.StartsWith("/lockalldigimon")) {
                gm.Database.LockAllDigimon();
                return "All Digimon have been locked.";
            }
            if (command.StartsWith("/setdigimoncodeunlocked")) {
                string[] args = command.Split(' ');
                if (args.Length == 3) {
                    if (gm.Database.GetDigimon(args[1]) == null) return "Digimon not found.";
                    if (args[2].ToLower() == "true") {
                        gm.LoadedGame.SetDigimonCodeUnlocked(args[1], true);
                        return "Code for " + args[1] + " unlocked set to: true.";
                    }
                    else if (args[2].ToLower() == "false") {
                        gm.LoadedGame.SetDigimonCodeUnlocked(args[1], false);
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
    }
}