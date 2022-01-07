using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PewterTeamBuff : LeadUnitBuff {
    public override void Apply(UnitList unitList) {
        foreach (GameObject untObj in unitList.mapUnits) {
            Unit unit = untObj.GetComponent<Unit>();
            unit.data.def.AddModifier(7);
            unit.data.atk.AddModifier(-5);
        }
    }
}
