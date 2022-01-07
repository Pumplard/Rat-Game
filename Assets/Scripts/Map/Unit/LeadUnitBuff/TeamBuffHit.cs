using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBuffHit : LeadUnitBuff {
    public override void Apply(UnitList unitList) {
        foreach (GameObject untObj in unitList.mapUnits) {
            Unit unit = untObj.GetComponent<Unit>();
            unit.data.hit.AddModifier(10);
        }
    }
}
