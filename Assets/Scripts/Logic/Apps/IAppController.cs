using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kaisa.Digivice {
    /// <summary>
    /// An interface for any object that can open and control an app.
    /// </summary>
    public interface IAppController {
        void CloseLoadedApp(Screen newScreen = Screen.MainMenu);
    }
}