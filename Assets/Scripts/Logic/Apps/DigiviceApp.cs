using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.App {
    public abstract class DigiviceApp : MonoBehaviour {
        protected string[] appArgs;
        protected IAppController controller;
        [Header("UI Elements")]
        [SerializeField]
        protected Image screenDisplay;
        protected Transform Parent => screenDisplay.transform;

        protected GameManager gm;
        protected AudioManager audioMgr;

        //AppLoader
        public static DigiviceApp LoadApp(GameObject appPrefab, GameManager gm, IAppController controller, params string[] appArgs) {
            GameObject appGO = Instantiate(appPrefab, gm.RootParent);
            DigiviceApp app = appGO.GetComponent<DigiviceApp>();
            app.Initialize(gm, appArgs, controller);
            return app;
        }

        public virtual void Initialize(GameManager gm, string[] appArgs, IAppController controller) {
            this.gm = gm;
            this.appArgs = appArgs;
            this.controller = controller;
            audioMgr = gm.audioMgr;
            StartApp();
        }
        public virtual void Dispose() => Destroy(gameObject);

        public virtual void InputA() { }
        public virtual void InputB() { }
        public virtual void InputLeft() { }
        public virtual void InputRight() { }
        public virtual void InputADown() { }
        public virtual void InputBDown() { }
        public virtual void InputLeftDown() { }
        public virtual void InputRightDown() { }
        public virtual void InputAUp() { }
        public virtual void InputBUp() { }
        public virtual void InputLeftUp() { }
        public virtual void InputRightUp() { }

        protected Coroutine navigationCoroutine;
        protected virtual IEnumerator AutoNavigateDir(Direction dir) { yield return null; }
        protected void StartNavigation(Direction dir) {
            if (navigationCoroutine != null) StopNavigation();
            navigationCoroutine = StartCoroutine(AutoNavigateDir(dir));
        }
        protected void StopNavigation() {
            if (navigationCoroutine != null) StopCoroutine(navigationCoroutine);
        }

        protected abstract void StartApp();
        protected virtual void CloseApp(Screen goToMenu = Screen.MainMenu) {
            controller.FinalizeApp(goToMenu);
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