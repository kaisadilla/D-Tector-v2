using Kaisa.Digivice;
using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RectangleBuilder : ScreenElement {
    [SerializeField] private Image rectangle;

    //Flick variables:
    private float timePassed = 0f;
    private bool isEnabled = true;
    private float flickPeriod = 0f;

    public override void InvertColors(bool val) {
        if (val) {
            rectangle.color = Constants.backgroundColor;
            background.color = Constants.activeColor;
        }
        else {
            rectangle.color = Constants.activeColor;
            background.color = Constants.backgroundColor;
        }
    }

    //Specific methods:
    /// <summary>
    /// Sets the speed (in seeconds) at which this rectangle flicks. A flick speed of 0 means that it will not flick.
    /// </summary>
    public void SetFlickPeriod(float flickPeriod, bool startEnabled = true) {
        this.flickPeriod = flickPeriod;
        rectangle.enabled = startEnabled;
        isEnabled = startEnabled;
    }
    public float GetFlickPeriod() {
        return flickPeriod;
    }

    public void ResetFlick(bool startEnabled = true) {
        timePassed = 0f;
        rectangle.enabled = startEnabled;
        isEnabled = startEnabled;
    }

    private void Update() {
        if (flickPeriod == 0) {
            if(isEnabled == false) {
                rectangle.enabled = true;
            }
        }
        else {
            timePassed += Time.deltaTime;
            if(timePassed >= flickPeriod) {
                timePassed = 0f;

                if(isEnabled) {
                    rectangle.enabled = false;
                    isEnabled = false;
                }
                else {
                    rectangle.enabled = true;
                    isEnabled = true;
                }
            }
        }
    }
}
