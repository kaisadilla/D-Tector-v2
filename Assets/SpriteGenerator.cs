using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteGenerator : MonoBehaviour {
    Generator g = new Generator();
    private void Start () {
        Debug.Log(g.DoStuff().Name);
    }
}
