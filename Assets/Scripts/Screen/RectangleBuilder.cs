using Kaisa.Digivice;
using Kaisa.Digivice.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RectangleBuilder : MonoBehaviour, IScreenElement {
    [SerializeField] private Image elementImage;
    [SerializeField] private Color screenColor;
    private float timePassed = 0f;
    private bool isBlack = true;
    private int flickPeriod = 0;

    //IScreenElement methods:
    public void Dispose() => Destroy(gameObject);
    public void SetActive(bool active) => gameObject.SetActive(active);
    public void SetName(string name) => gameObject.name = name;
    public void SetPosition(int x, int y) {
        gameObject.PlaceInPosition(x, y);
    }
    public void SetPosition(Vector2Int pos) => SetPosition(pos.x, pos.y);
    public void SetSize(int width, int height) {
        width *= Constants.PixelSize;
        height *= Constants.PixelSize;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }

    //Specific methods:
    /// <summary>
    /// Sets the speed (in ms) at which this rectangle flicks. A flick speed of 0 means that it will not flick.
    /// </summary>
    public void SetFlickPeriod(int flickPeriod) {
        this.flickPeriod = flickPeriod;
    }

    //MonoBehaviour methods:
    private void Update() {
        if (flickPeriod == 0) {
            if(isBlack == false) {
                elementImage.color = Color.black;
            }
        }
        else {
            timePassed += Time.deltaTime * 1000;
            if(timePassed >= flickPeriod) {
                timePassed = 0f;

                if(isBlack) {
                    elementImage.color = screenColor;
                    isBlack = false;
                }
                else {
                    elementImage.color = Color.black;
                    isBlack = true;
                }
            }
        }
    }
}
