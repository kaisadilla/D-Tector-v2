using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaisa.Digivice.App {
    public abstract class DigiviceApp : MonoBehaviour {
        [Header("UI Elements")]
        [SerializeField]
        protected Image screenDisplay;

        protected GameManager gm;
        protected AudioManager audioMgr;

        //AppLoader
        public static DigiviceApp LoadApp(GameObject appPrefab, GameManager gm) {
            GameObject appGO = Instantiate(appPrefab, gm.RootParent);
            DigiviceApp app = appGO.GetComponent<DigiviceApp>();
            app.Initialize(gm);
            return app;
        }

        public virtual void Initialize(GameManager gm) {
            this.gm = gm;
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
    }
}