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

        private string SelectedFilePath {
            get {
                return gameSlotGroup.ActiveToggles().First().GetComponent<VisualGameSlot>().FilePath;
            }
        }
        private BriefSavedGame SelectedSavedGame => SavedGame.GetBriefSavedGame(SelectedFilePath);

        private void Awake() {
            if (!SavedGame.IsConfigurationInitialized) {
                SavedGame.ConfigVolume = 1f;
                SavedGame.ConfigLocalization = 0;
                SavedGame.ConfigActiveColor = Color.black;
                SavedGame.ConfigBackgroundColor = new Color32(129, 147, 118, 255);
                SavedGame.IsConfigurationInitialized = true;
            }
            Preferences.ApplyPreferences();

            if (SavedGame.CurrentlyLoadedFilePath != "") {
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
            SavedGame.CurrentlyLoadedFilePath = SelectedFilePath;
            Debug.Log($"Loaded {SavedGame.CurrentlyLoadedFilePath}");
            SceneManager.LoadScene("DigiviceFrontier");
        }

        public void PromptDeleteSelectedGame() {
            prDeleteGameText.text =
                $"Are you sure you want to permanently delete the game" +
                $" {SelectedSavedGame.name} ({SelectedSavedGame.character}, Lv. {SelectedSavedGame.level})?" +
                $"\nThis action can't be undone.";
            blockPanel.SetActive(true);
            promptDeleteGame.SetActive(true);
        }

        public void DeleteSelectedGame() {
            Debug.Log("Attempting to delete " + SelectedFilePath);
            SavedGame.DeleteSavedGame(SelectedFilePath);
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
            string path = SavedGame.CreateSavedGame(prCreateGameInput.text);

            Debug.Log($"Created game at slot {path}");
            SceneManager.LoadScene("DigiviceFrontier");
        }
    }
}