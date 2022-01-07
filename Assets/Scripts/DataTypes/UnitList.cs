using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UnitList", menuName = "UnitList")]
[System.Serializable]
public class UnitList : ScriptableObject {
    //baseUnit only needed if list will be used in the map
    public GameObject baseUnit; //default gameobject that is instantiated and receives unitData, needs to have unit script attatched

    public LeadUnitData leadUnit; //commander unit, is always the last unit in the unitList, added only when map initiates
    public List<UnitData> units; //units before being initialized on map
    internal List<GameObject> mapUnits; //units on map, can be assumed all have "Unit" component

    //Can be null, assigns unitAI[i] to Unit[i]
    //must both be the same size as units or completely empty
    public List<AIType> unitAI;
    public List<Weapon> unitWeapons;

    public int team = 0;
    public int playerGroup = 0; //not mpa
    public int group = 0; //map
    public int unitTotal;
    public int maxUnits = 15;
    internal bool isActive = false; //false if all units are dead. set in init, set false in turnmanager.
    



    public bool hasLeadUnit {
        get {
            if (leadUnit != null)
                return true;
            else
                return false; 
        }
    }

    //makes copies of baseUnit w unitData to mapUnits then puts them on the map
    //called in mapgen
    public void Initialize() {
        RemoveNullUnits();
        mapUnits = new List<GameObject>();
        unitTotal = units.Count;
        isActive = true; 
    }

    //used in loadmap, resets to initial state, maintaining group number
    public void Reset() {
        leadUnit = null;
        mapUnits.Clear();
        ResetInitialUnits();
    }

    //resets list units, used in UnitShop
    public void ResetInitialUnits() {

        units.Clear();
        unitTotal = 0;
    }

    //removes null units from list on instantiated map in mapGen
    public void RemoveNullUnits() {
        units.RemoveAll(item => item == null);
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



    //returns object created from unitData in units by index (data not yet read)
    public GameObject CreateUnitObject(int i, Vector2 coords) {
        
        GameObject unitObject = Instantiate(baseUnit, coords, Quaternion.identity);
        
        Unit unitScript = unitObject.GetComponent<Unit>();
        
        UnitData data = Instantiate(units[i]);
        unitScript.data = data;
        return(unitObject);
    }

    //called in Turnmanager, also handles phase-based morale systems
    //also called on first acting group by mapManager
    public void OnTurnStart() {
        unitTotal = mapUnits.Count;
        foreach (GameObject unitObject in mapUnits) {
            Unit unitScript = unitObject.GetComponent<Unit>();
            if (unitScript != null) {
                unitScript.hasActed = false;
                if (unitScript.animator != null) 
                    unitScript.StartIdleAnimationR();

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
            unitScript.StartIdleAnimationR();
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

    //used in shop
    public void AddUnit(UnitData newUnit) {
        if (units == null)
            units = new List<UnitData>();
        units.Add(newUnit);
        unitTotal ++;

       newUnit.SetPlayerGroup(playerGroup);
    }

    //used in loadlevel, 
    public void AddUnits(List<UnitData> newUnits) {
        foreach (UnitData unit in newUnits) {
                if (unit != null)
                    AddUnit(unit);
            }
    }

    //called on player teams by mapManager on map end, adds mapUnits from mapUnitList to playerData and removes mapUnits
    //also removes status affects & restores maxhp
    public void ReturnMapUnits(UnitList mapList) {
        units = new List<UnitData>();
        foreach (GameObject unitObj in mapList.mapUnits) {
            Unit unit = unitObj.GetComponent<Unit>();
            if (unit != null && (unit.data.unitName != leadUnit.unitName)) {
                unit.data.RestoreStatus();
                AddUnit(unit.data);
            }
        }
        RemoveMapUnits();
    }

    //removes unit from list, called in mapManager on Removeunit
    public void RemoveUnit(GameObject toRemove) {
        mapUnits.Remove(toRemove);
        unitTotal --;
    }

    //removes map untis, called on enemy teams by mapManager on map end
    public void RemoveMapUnits() {
        for (int i = mapUnits.Count-1; i >= 0;i--) {
            RemoveUnit(mapUnits[i]);
        }
        mapUnits.Clear();
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

}

