using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitSwap : GroupEditor {

    public int group1;
    public int group2;

    protected UnitData unit;


    public void AddUnit(UnitButton newUnitB) {
        UnitData unitData = newUnitB.unit;
        //Unit unitData = unitO.GetComponent<Unit>();
        //should make them cancel when both units are the same
        if (unit == null) {
            unit = unitData;
            group1 = unitData.playerGroup;
        } 
        else {
            group2 = unitData.playerGroup;  
            SwapUnit(unitData);
        }
    }

    //cleared every swap and when gui closed
    public void Clear() {
        unit = null;
    }

    protected abstract void SwapUnit(UnitData newUnit);
    //protected abstract void SwapUnit(GameObject newUnit);
}
