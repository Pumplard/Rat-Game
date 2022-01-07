using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//MoraleScript for phase based morale system
//Current children: Cavalry
public abstract class MoralePhase : MoraleScript {

    //sets the morale modifier based on if unit.changingPhase and unit.activePhase
    protected override void SetUnitModifiers(int prevMrl) {
        Morale prevMorale = GetCurMorale(prevMrl);
        Morale curMorale = GetCurMorale(curUnit.data.mrl);
        if (curUnit.changingPhase) {
            if (curUnit.activePhase) {
                //from active to inactive
                RemoveModifierA(prevMorale);
                AddModifier(curMorale);
            }
            else {
                //from inactive to active
                RemoveModifier(prevMorale);
                AddModifierA(curMorale);
            }
        }
        else /*if (curMorale != prevMorale)*/ {
            if (curUnit.activePhase) {
                RemoveModifierA(prevMorale);
                AddModifierA(curMorale);
            }
            else {
                RemoveModifier(prevMorale);
                AddModifier(curMorale);
            }
        }
    }

    //functions for modifiers on actve phase
    //moraleScript Remove/AddModifier are used for inactive phase
    protected abstract void RemoveModifierA(Morale mrl);

    protected abstract void AddModifierA(Morale mrl);
}
