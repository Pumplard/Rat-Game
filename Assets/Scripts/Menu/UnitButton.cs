using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitButton : MonoBehaviour
{
    public UnitData unit;
    public UnitInfoDisplay infoDisplay; //can be null

    public void SetInfoDisplay() {
        if (unit != null)
            infoDisplay.DisplayUnitInfo(unit);
    }
}
