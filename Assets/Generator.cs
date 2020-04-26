using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kaisa.Digivice;

public class Generator {
    public byte[] GenerateRandom(int width, int height) {
        byte[] allBytes = new byte[width * height];
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                allBytes[x + (y * width)] = (byte)Random.Range(0, 2);
            }
        }
        return allBytes;
    }

    public Menu DoStuff() {
        Menu menu = "Main menu";
        return menu["parent"]["child"].SetOrder(0);
        //return menu["parent"]["child"];
    }
}
