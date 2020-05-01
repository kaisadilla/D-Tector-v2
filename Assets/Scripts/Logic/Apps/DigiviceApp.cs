using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.App {
    public abstract class DigiviceApp : MonoBehaviour {
        protected string[] appArgs;
        [Header("UI Elements")]
        [SerializeField]
        protected Image screenDisplay;
        protected Transform Parent => screenDisplay.transform;

        protected GameManager gm;
        protected AudioManager audioMgr;

        //AppLoader
        public static DigiviceApp LoadApp(GameObject appPrefab, GameManager gm, params string[] appArgs) {
            GameObject appGO = Instantiate(appPrefab, gm.RootParent);
            DigiviceApp app = appGO.GetComponent<DigiviceApp>();
            app.Initialize(gm, appArgs);
            return app;
        }

        public virtual void Initialize(GameManager gm, string[] appArgs) {
            this.gm = gm;
            this.appArgs = appArgs;
            audioMgr = gm.audioMgr;
            StartApp();
        }
        public virtual void Dispose() => Destroy(gameObject);

        public abstract void InputA();
        public abstract void InputB();
        public abstract void InputLeft();
        public abstract void InputRight();

        protected abstract void StartApp();
        protected virtual void CloseApp(Screen goToMenu = Screen.MainMenu) {
            gm.logicMgr.FinalizeApp(goToMenu);
        }

        protected void SetScreen(Sprite sprite) => screenDisplay.sprite = sprite;

        /// <summary>
        /// Destroys all children gameObjects of this app.
        /// </summary>
        protected void ClearScreen() {
            foreach (Transform child in screenDisplay.transform) {
                Destroy(child.gameObject);
            }
        }
    }
}