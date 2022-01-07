using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//scripts lead unit has for applying buffs
public abstract class LeadUnitBuff : MonoBehaviour {
    //called in mapGen, applies to mapUnits
    public abstract void Apply(UnitList unitList);
    
}
