using Kaisa.Digivice;
using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RectangleBuilder : ScreenElement {
    [SerializeField] private Image rectangle;
    [SerializeField] private Color screenColor;

    //Flick variables:
    private float timePassed = 0f;
    private bool isBlack = true;
    private int flickPeriod = 0;

    //Specific methods:
    /// <summary>
    /// Sets the speed (in ms) at which this rectangle flicks. A flick speed of 0 means that it will not flick.
    /// </summary>
    public void SetFlickPeriod(int flickPeriod) {
        this.flickPeriod = flickPeriod;
    }

    private void Update() {
        if (flickPeriod == 0) {
            if(isBlack == false) {
                rectangle.color = Color.black;
            }
        }
        else {
            timePassed += Time.deltaTime * 1000;
            if(timePassed >= flickPeriod) {
                timePassed = 0f;

                if(isBlack) {
                    rectangle.color = screenColor;
                    isBlack = false;
                }
                else {
                    rectangle.color = Color.black;
                    isBlack = true;
                }
            }
        }
    }
}
