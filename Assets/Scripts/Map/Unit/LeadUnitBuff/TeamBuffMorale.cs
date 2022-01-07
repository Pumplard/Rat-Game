using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBuffMorale : LeadUnitBuff {
    public override void Apply(UnitList unitList) {
        foreach (GameObject untObj in unitList.mapUnits) {
            Unit unit = untObj.GetComponent<Unit>();
            unit.data.mrl += 20;
        }
    }
}
