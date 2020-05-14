using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.Apps {
    public abstract class DigiviceApp : MonoBehaviour {
        protected IAppController controller;
        [Header("UI Elements")]
        [SerializeField]
        protected Image screenDisplay;
        protected Transform Parent => screenDisplay.transform;

        protected GameManager gm;
        protected AudioManager audioMgr;
        private void Awake() {
            screenDisplay.color = Preferences.ActiveColor;
            screenDisplay.sprite = Constants.EMPTY_SPRITE;
        }

        public virtual void Setup(GameManager gm, IAppController controller) {
            this.gm = gm;
            this.controller = controller;
            audioMgr = gm.audioMgr;
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

        public abstract void StartApp();
        protected virtual void CloseApp(Screen gotoMenu = Screen.MainMenu) {
            controller.CloseLoadedApp(gotoMenu);
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