using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUnitSwap : UnitSwap {

    //right now button causes redisplay
    protected override void SwapUnit(UnitData newUnit) {
        List<UnitData> units1 = playerGroups[group1].units;
        List<UnitData> units2 = playerGroups[group2].units;

        int unitI = units1.IndexOf(unit);
        int newUnitI = units2.IndexOf(newUnit);
        //Debug.Log(unitI + newUnitI);

        units1[unitI] = newUnit;
        units2[newUnitI] = unit;
        unit.SetPlayerGroup(group2);
        newUnit.SetPlayerGroup(group1);

        Clear();
    }



}
