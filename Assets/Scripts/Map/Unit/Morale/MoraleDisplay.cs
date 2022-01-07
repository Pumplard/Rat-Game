using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoraleDisplay : MonoBehaviour
{
    public Sprite[] moraleSprites = new Sprite[4];
    public Image display;

    //displays current morale
    public void Display(int val) {
        if (val >= 80) 
            display.sprite = moraleSprites[0];
        else if (val >= 60) 
            display.sprite = moraleSprites[1];
        else if (val >= 40) 
            display.sprite = moraleSprites[2];
        else if (val >= 20)    
            display.sprite = moraleSprites[3];
        else 
            display.sprite = moraleSprites[4];
    }
}
