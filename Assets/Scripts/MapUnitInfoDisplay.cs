using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//unit info display for maps, uses units rather than unitdata
//Exists: HoverinfoDisplay, Attack info, Unit details
public class MapUnitInfoDisplay : UnitInfoDisplay {
    //non-map info displays don't need extra displays for the group
    public bool showGroup;
    public Image group;
    public TMP_Text groupText;
    
     //takes unit, used in map hover info since it needs a new bg
    public void DisplayUnitInfo(Unit unit, string hpString, Texture newBg) {
        bgImage.texture = newBg;
        DisplayUnitInfo(unit, hpString);


    }

    //used to set custom hp string, used in hoverinfo,
    
    //for unitinfodisplay, called in mapmanager
    //sets hpPreset to true
    //also used in attackmanager
    public void DisplayUnitInfo(Unit unit, string hpString) {
        hp.text = hpString;
        presetHp = true; //safety
        DisplayUnitInfo(unit.data);
        if (showGroup) {
            DisplayGroup(unit);
        }
    }

    public void DisplayGroup(Unit unit) {
        group.gameObject.SetActive(true); 
        group.sprite = unit.groupIcon;
        groupText.text = unit.group.ToString();
    }


    //function used for map hover info, takes in map object
    public void manageHoverInfo(GameObject curTileObj) {
        if (curTileObj == null) { Clear();}
        else {
            Unit tileUnit = curTileObj.GetComponent<Unit>();
            if (tileUnit != null) {
                DisplayUnitInfo(tileUnit, tileUnit.data.hp + "/" + tileUnit.data.maxHp, hoverTextures[tileUnit.team]);
            }
        }       
    }
}
