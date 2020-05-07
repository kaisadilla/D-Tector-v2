using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kaisa.Digivice {
    public class GameLoader : MonoBehaviour {
        [SerializeField] private GameObject pVisualGameSlot;

        [SerializeField] private Transform contentPane;
        [SerializeField] private ToggleGroup gameSlotGroup;

        [SerializeField] private GameObject blockPanel;
        [SerializeField] private GameObject promptDeleteGame;
        [SerializeField] private Text prDeleteGameText;
        [SerializeField] private GameObject promptCreateGame;
        [SerializeField] private InputField prCreateGameInput;

        private List<BriefSavedGame> allSavedGames;

        private int SelectedSlot {
            get {
                return gameSlotGroup.ActiveToggles().First().GetComponent<VisualGameSlot>().Slot;
            }
        }
        private BriefSavedGame SelectedSavedGame => SavedGame.GetBriefSavedGame(SelectedSlot);

        private void Awake() {
            if (SavedGame.CurrentlyLoadedSlot != -1) {
                SceneManager.LoadScene("DigiviceFrontier");
            }
        }

        void Start() {
            BuildSavedGameList();
        }

        private void BuildSavedGameList() {
            foreach (Transform child in contentPane) Destroy(child.gameObject);

            allSavedGames = SavedGame.GetAllSavedGames();

            foreach (BriefSavedGame sg in allSavedGames) {
                GameObject visualGameSlot = Instantiate(pVisualGameSlot, contentPane);
                visualGameSlot.GetComponent<VisualGameSlot>().SetData(sg, gameSlotGroup);
            }
        }

        public void LoadSelectedGame() {
            SavedGame.CurrentlyLoadedSlot = SelectedSlot;
            Debug.Log($"Loaded {SavedGame.CurrentlyLoadedSlot}");
            SceneManager.LoadScene("DigiviceFrontier");
        }

        public void PromptDeleteSelectedGame() {
            prDeleteGameText.text =
                $"Are you sure you want to permanently delete the game [{SelectedSavedGame.slot}]" +
                $" {SelectedSavedGame.name} ({SelectedSavedGame.character}, Lv. {SelectedSavedGame.level})?" +
                $"\nThis action can't be undone.";
            blockPanel.SetActive(true);
            promptDeleteGame.SetActive(true);
        }

        public void DeleteSelectedGame() {
            SavedGame.DeleteSlot(SelectedSlot);
            BuildSavedGameList();
            CloseDeleteSelectedGame();
        }
        
        public void CloseDeleteSelectedGame() {
            blockPanel.SetActive(false);
            promptDeleteGame.SetActive(false);
        }

        public void PromptNewGame() {
            blockPanel.SetActive(true);
            promptCreateGame.SetActive(true);
        }

        public void CloseNewGame() {
            blockPanel.SetActive(false);
            promptCreateGame.SetActive(false);
        }

        public void CreateNewGame() {
            if (prCreateGameInput.text == "") return;
            int newSlot = SavedGame.GetFirstEmptySlot();
            SavedGame.CurrentlyLoadedSlot = newSlot;
            SavedGame sg = SavedGame.CreateSavedGame(newSlot);
            sg.Name = prCreateGameInput.text;

            Debug.Log($"Created game at slot {newSlot}");
            SceneManager.LoadScene("DigiviceFrontier");
        }
    }
}