using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {
    public int baseVal;   
    private int weaponVal = 0; //weapon val not considered a modifier=

    //modifiers not included 
    public List<int> modifiers = new List<int>();

    public void SetWeaponVal(int newVal) {
        weaponVal = newVal;
    }

    //returns color for stat
    public Color GetStatColor() {
        int initVal = baseVal + weaponVal;
        int finalVal = initVal;
        modifiers.ForEach(x => finalVal += x);

        if (finalVal == initVal) return Color.white;
        else if (finalVal > initVal) return Color.cyan;
        else return Color.red;

    }
    public int GetValue() {
        int finalVal = baseVal + weaponVal;

        modifiers.ForEach(x => finalVal += x);
        if (finalVal < 0) return 0;
        return finalVal;
    }

    public void AddModifier(int modifier) {
        if (modifier != 0)
            modifiers.Add(modifier);
    }

    public void RemoveModifier(int modifier) {
        if (modifier != 0)
            modifiers.Remove(modifier);
    }

    public void ClearModifiers() {
        modifiers.Clear();
    }
}
