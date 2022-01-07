using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UnitList", menuName = "UnitList")]
[System.Serializable]
public class UnitList : ScriptableObject {
    public GameObject baseUnit; //default gameobject that is instantiated and receives unitData, needs to have unit script attatched

    public LeadUnitData leadUnit; //commander unit, is always the last unit in the unitList, added only when map initiates
    public List<UnitData> units; //units before being initialized on map
    internal List<GameObject> mapUnits; //units on map
    public int team = 0;
    public int playerGroup = 0;
    public int group = 0;
    public int unitTotal;
    public int maxUnits = 15;
    public int unitsActed = 0; //this is set everytime a unit acts
    internal bool isActive = false; 
    public bool allUnitsActed {
        get {
            return (unitTotal <= unitsActed);
        }
    }

    public bool hasLeadUnit {
        get {
            if (leadUnit != null)
                return true;
            else
                return false; 
        }
    }

    public void ResetInitialUnits() {
        units.Clear();
        unitTotal = 0;
        unitsActed = 0;
    }

    //use only for initial units
    public bool IsEmpty() {
        if (units.Count <= 0)
            return true;
        else
            return false;
    }
    //sets units, used when adding player units, except not anymore
    /*public void SetInitialUnits(List<GameObject> newUnits) {
        if (units == null)
            units = new List<GameObject>();
        ResetInitialUnits();
        if (newUnits != null) {
            foreach (GameObject newUnit in newUnits) {
            units.Add(newUnit);
            }   
        }
    }*/

    //makes copies of baseUnit w unitData to mapUnits then puts them on the map
    public void Initialize() {
        mapUnits = new List<GameObject>();
        unitTotal = units.Count;
        unitsActed = 0;
        isActive = true; 
    }

    //returns object created from unitData in units by index (data not yet read)
    public GameObject CreateUnitObject(int i, Vector2 coords) {
        GameObject unitObject = Instantiate(baseUnit, coords, Quaternion.identity);
        
        Unit unitScript = unitObject.GetComponent<Unit>();
        
        UnitData data = Instantiate(units[i]);
        unitObject.GetComponent<Transform>().localScale *= data.scale;//temp
        unitScript.data = data;
        return(unitObject);
    }

    //called in Turnmanager, also handles phase-based morale systems
    //also called on first acting group by mapManager
    public void OnTurnStart() {
        unitTotal = mapUnits.Count;
        unitsActed = 0;
        foreach (GameObject unitObject in mapUnits) {
            Unit unitScript = unitObject.GetComponent<Unit>();
            if (unitScript != null) {
                unitScript.hasActed = false;
                if (unitScript.animator != null) 
                    unitScript.StartIdleAnimation();

                //handles phase based morale scripts
                unitScript.activePhase = false;
                HandlePhaseMorale(unitScript);
                unitScript.activePhase = true;    
            }
        }
    }

    //able to end
    //called in Turnmanager
    //also called on turn one on all inactive groups in  mapmanager
    public void OnTurnEnd() {
        foreach (GameObject unitObject in mapUnits) {
            Unit unitScript = unitObject.GetComponent<Unit>();
            unitScript.hasActed = true;
            //handles phase based morale scripts
            unitScript.activePhase = true;
            HandlePhaseMorale(unitScript);
            unitScript.activePhase = false;
        }
    }


    //handles phase based morale scripts, called on turn start and end
    private void HandlePhaseMorale(Unit unit) {
        unit.changingPhase = true;
        unit.data.moraleScript.UpdateUnitMorale(unit,0);
        unit.changingPhase = false;
    }
    public void AddAction() {
        unitsActed += 1;
    }

    public void AddUnit(UnitData newUnit) {
        if (units == null)
            units = new List<UnitData>();
        units.Add(newUnit);
        unitTotal ++;

       newUnit.SetPlayerGroup(playerGroup);
    }

    //removes unit from list, called in mapManager on Removeunit
    public void RemoveUnit(GameObject toRemove) {
        mapUnits.Remove(toRemove);
        unitTotal --;
    }

    //returns map unit data to units and removes them, called on player teams by mapManager on map end
    public void ReturnMapUnits(){
        units = new List<UnitData>();
        foreach (GameObject unitObj in mapUnits) {
            Unit unit = unitObj.GetComponent<Unit>();
            if (unit != null && unit.unitName != leadUnit.unitName) {
                units.Add(unit.data);
            }
        }
        RemoveMapUnits();
    }

    //removes map untis, called on enemy teams by mapManager on map end
    public void RemoveMapUnits() {
        for (int i = mapUnits.Count-1; i >= 0;i--) {
            RemoveUnit(mapUnits[i]);
        }
    }

    //called in dropdown
    public void SetLeadUnit(LeadUnitData newLead) {
        leadUnit = newLead;
    }
    //adds lead unit to unit list, used when generating map
    public void AddLeadUnit() {
        if (hasLeadUnit) {
            units.Add(leadUnit);
            unitTotal ++;
        }
    }

    //used in "end" selection, used to force turn end on next turnmanager.CompleteAction()
    public void TurnEndCondition() {
        unitsActed = unitTotal;
    }
}

