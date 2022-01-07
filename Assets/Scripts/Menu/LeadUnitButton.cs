using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//unitbutton for lead units, sets lead unit playergroup (since they are not bought) activates dropdown
public class LeadUnitButton : UnitButton
{
    public MenuUnitDisplay display;
    internal RectTransform rTransform;
    public int playerGroup; //set in teamdisplaymanager

    void Awake() {
        rTransform = GetComponent<RectTransform>();
    }
    public void DisplayLeaders() {
        unit.SetPlayerGroup(playerGroup);
        display.SetUnitList(unit.playerGroup);
        display.DisplayGroup(new Vector2(-120 + (playerGroup*100),-25));
    }

    public void LeadUnitSwap() {
        LeadUnitData leadUnit= (LeadUnitData)unit;
        display.LeadUnitSwap(leadUnit);
    }
}
