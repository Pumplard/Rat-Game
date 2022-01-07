using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleCavalry : MoralePhase {
    protected override void RemoveModifier(Morale mrl) {
        switch (mrl) {
            case Morale.A:
                curUnit.data.avo.RemoveModifier(20);
                break;
            case Morale.B:
                curUnit.data.avo.RemoveModifier(10);
                break;
            case Morale.C:
                break;
            case Morale.D:
                curUnit.data.avo.RemoveModifier(-10);
                break;
            case Morale.E:
                curUnit.data.avo.RemoveModifier(-20);
                break;
        }
    }

    protected override void AddModifier(Morale mrl) {
        switch (mrl) {
            case Morale.A:
                curUnit.data.avo.AddModifier(20);
                break;
            case Morale.B:
                curUnit.data.avo.AddModifier(10);
                break;
            case Morale.C:
                break;
            case Morale.D:
                curUnit.data.avo.AddModifier(-10);
                break;
            case Morale.E:
                curUnit.data.avo.AddModifier(-20);
                break;
        }
    }

    protected override void RemoveModifierA(Morale mrl) {
        switch (mrl) {
            case Morale.A:
                curUnit.data.atk.RemoveModifier(4);
                break;
            case Morale.B:
                curUnit.data.atk.RemoveModifier(2);
                break;
            case Morale.C:
                break;
            case Morale.D:
                curUnit.data.atk.RemoveModifier(-2);
                break;
            case Morale.E:
                curUnit.data.atk.RemoveModifier(-4);
                break;
        }
    }

    protected override void AddModifierA(Morale mrl) {
        switch (mrl) {
            case Morale.A:
                curUnit.data.atk.AddModifier(4);
                break;
            case Morale.B:
                curUnit.data.atk.AddModifier(2);
                break;
            case Morale.C:
                break;
            case Morale.D:
                curUnit.data.atk.AddModifier(-2);
                break;
            case Morale.E:
                curUnit.data.atk.AddModifier(-4);
                break;
        }
    }
}
