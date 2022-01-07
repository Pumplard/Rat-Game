using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MoneyManager : MonoBehaviour
{
    public int curAmount;
    public TMP_Text display;

    void Awake() {
        gameObject.SetActive(true);
        SetAmount(curAmount);
    }

    public void RemoveAmount(int amount) {
        SetAmount(curAmount - amount);
    }
    public void SetAmount(int amount) {
        curAmount = amount;
        display.text = "Dolloar: " + amount;
    }
}
