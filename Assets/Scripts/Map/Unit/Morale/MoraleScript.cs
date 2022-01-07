using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Morale { A, B, C, D, E}
//MoraleScripts will be stored as singleton prefabs
//Right now, morale is initialized as 50 at start of map
//morale is an int, doesn't use "modifiers"
//right now, morale only appears on map
public abstract class MoraleScript : MonoBehaviour
{
    //current unit functions are being called on
    protected Unit curUnit;

    //gets current Morale state
    //switch statement range is slower?
    protected Morale GetCurMorale(int val) {
        if (val >= 80) return Morale.A;
        else if (val >= 60) return Morale.B;
        else if (val >= 40) return Morale.C;
        else if (val >= 20) return Morale.D;
        else return Morale.E;
    }


    //Sets unit
    //Updates unit morale count and applys/removes morale bonuses
    
    public void UpdateUnitMorale(Unit unit, int diff) {
        //if (diff == 0) 
        //    return;

        int prevMrl = unit.data.mrl;
        int newMrl = prevMrl + diff;
        if (newMrl < 0) newMrl = 0;
        if (newMrl > 100) newMrl = 100;
        unit.data.mrl = newMrl;

        curUnit = unit;
        SetUnitModifiers(prevMrl);
        curUnit = null;
    }
    
    //Calls addmodifier(curMrl) and removeModifier(prevMrl) on curUnit
    protected virtual void SetUnitModifiers(int prevMrl) {
        Morale prevMorale = GetCurMorale(prevMrl);
        Morale curMorale = GetCurMorale(curUnit.data.mrl);
        //if (curMorale != prevMorale) {
            RemoveModifier(prevMorale);
            AddModifier(curMorale);
        //}
    }

    //these scripts handle removing/adding correct modifiers
    protected abstract void RemoveModifier(Morale mrl);

    protected abstract void AddModifier(Morale mrl);
}
