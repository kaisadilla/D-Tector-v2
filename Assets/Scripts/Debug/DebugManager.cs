using System;
using System.Linq;
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
            if (command.StartsWith("/currentSlot")) {
                return "Current slot: " + gm.LoadedGame.Slot;
            }
            if (command.StartsWith("/checkSlot")) {
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
            if (command.StartsWith("/gameName")) {
                return "Game name: " + gm.LoadedGame.Name;
            }
            if (command.StartsWith("/gameCharacter")) {
                return "Character: " + gm.LoadedGame.Name;
            }
            if (command.StartsWith("/currentMap")) {
                return "Current map: " + gm.LoadedGame.CurrentMap;
            }
            if (command.StartsWith("/currentArea")) {
                return "Current area: " + gm.LoadedGame.CurrentArea;
            }
            if (command.StartsWith("/currentDistance")) {
                return "Current distance: " + gm.LoadedGame.CurrentDistance;
            }
            if (command.StartsWith("/totalSteps")) {
                return "Steps: " + gm.LoadedGame.Steps;
            }
            if (command.StartsWith("/stepsToNextEvent")) {
                return "Steps until the next event: " + gm.LoadedGame.StepsToNextEvent;
            }
            if (command.StartsWith("/playerExperience")) {
                return "Player experience: " + gm.LoadedGame.PlayerExperience;
            }
            if (command.StartsWith("/playerSpiritPower")) {
                return "Player spirit power: " + gm.LoadedGame.SpiritPower;
            }
            if (command.StartsWith("/totalWins")) {
                return "Total wins: " + gm.LoadedGame.TotalWins;
            }
            if (command.StartsWith("/getDigimonLevel")) {
                string[] args = command.Split(' ');
                if (args.Length == 2) {
                    if (gm.Database.GetDigimon(args[1]) == null) return "Digimon not found.";
                    return "Digimon " + args[1] + " level: " + gm.LoadedGame.GetDigimonLevel(args[1]);
                }
                return "Invalid parameters. Expected (string)digimonName";
            }
            if (command.StartsWith("/isAreaCompleted")) {
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

            if (command.StartsWith("/addSpiritPower")) {
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
            if (command.StartsWith("/setDigimonLevel")) {
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
                return "Invalid parameters. Expected (string)digimonName, (int)level";
            }
            if (command.StartsWith("/unlockAllDigimon")) {
                gm.Database.UnlockAllDigimon();
                return "All Digimon have been unlocked.";
            }
            if (command.StartsWith("/lockAllDigimon")) {
                gm.Database.LockAllDigimon();
                return "All Digimon have been locked.";
            }

            return "Invalid command.";
        }
    }
}