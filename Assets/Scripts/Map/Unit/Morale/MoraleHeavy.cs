using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleHeavy : MoralePhase {
    protected override void RemoveModifier(Morale mrl) {
        switch (mrl) {
            case Morale.A:
                curUnit.data.def.RemoveModifier(4);
                break;
            case Morale.B:
                curUnit.data.def.RemoveModifier(2);
                break;
            case Morale.C:
                break;
            case Morale.D:
                curUnit.data.def.RemoveModifier(-2);
                break;
            case Morale.E:
                curUnit.data.def.RemoveModifier(-4);
                break;
        }
    }
    protected override void AddModifier(Morale mrl) {
        switch (mrl) {
            case Morale.A:
                curUnit.data.def.AddModifier(4);
                break;
            case Morale.B:
                curUnit.data.def.AddModifier(2);
                break;
            case Morale.C:
                break;
            case Morale.D:
                curUnit.data.def.AddModifier(-2);
                break;
            case Morale.E:
                curUnit.data.def.AddModifier(-4);
                break;
        }
    }

    protected override void RemoveModifierA(Morale mrl) {

    }

    protected override void AddModifierA(Morale mrl) {

    }
}
