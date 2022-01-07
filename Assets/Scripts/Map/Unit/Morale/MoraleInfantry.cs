using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoraleInfantry : MoraleScript {

    protected override void RemoveModifier(Morale mrl) {
        switch (mrl) {
            case Morale.A:
                curUnit.data.hit.RemoveModifier(20);
                break;
            case Morale.B:
                curUnit.data.hit.RemoveModifier(10);
                break;
            case Morale.C:
                break;
            case Morale.D:
                curUnit.data.hit.RemoveModifier(-10);
                break;
            case Morale.E:
                curUnit.data.hit.RemoveModifier(-20);
                break;
        }
    }

    protected override void AddModifier(Morale mrl) {
        switch (mrl) {
            case Morale.A:
                curUnit.data.hit.AddModifier(20);
                break;
            case Morale.B:
                curUnit.data.hit.AddModifier(10);
                break;
            case Morale.C:
                break;
            case Morale.D:
                curUnit.data.hit.AddModifier(-10);
                break;
            case Morale.E:
                curUnit.data.hit.AddModifier(-20);
                break;
        }
    }
}
