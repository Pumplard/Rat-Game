using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour
{
    public Image unitPortrait;
    public UnitButton button;

    public void SetUnit(UnitData unit) {
        button.unit = unit;
    }

    public void SetPortrait(Sprite newImage) {
        if (unitPortrait != null)
            unitPortrait.sprite = newImage;
        else
            Debug.Log("Null unitPortrait template");
    }
}
